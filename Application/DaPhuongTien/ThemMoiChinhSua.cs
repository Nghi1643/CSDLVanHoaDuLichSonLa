using Dapper;
using Domain.BaoChi;
using Domain.Core;
using Domain.VanHoa;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.DaPhuongTien
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.DanhMuc.DaPhuongTien>>
        {
            public Domain.DanhMuc.DaPhuongTien Data { get; set; }
            public List<Domain.DanhMuc.DaPhuongTien_NoiDung> NoiDungBanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.DanhMuc.DaPhuongTien>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;


            public Handler(IConfiguration config, IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _config = config;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Domain.DanhMuc.DaPhuongTien>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // lấy userId
                            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var parameters = new DynamicParameters();
                            parameters.Add("@DaPhuongTienID", request.Data.DaPhuongTienID);
                            parameters.Add("@DoiTuongSoHuuID", request.Data.DoiTuongSoHuuID);
                            parameters.Add("@LoaiDoiTuong", request.Data.LoaiDoiTuong);
                            parameters.Add("@LoaiMedia", request.Data.LoaiMedia);
                            parameters.Add("@TheLoaiID", request.Data.TheLoaiID);
                            parameters.Add("@DuongDanFile", request.Data.DuongDanFile);
                            parameters.Add("@TacGia", request.Data.TacGia);
                            parameters.Add("@ThuTuHienThi", request.Data.ThuTuHienThi);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@SuDung", request.Data.SuDung);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.DaPhuongTien>(
                                "spu_DM_DaPhuongTien_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@DaPhuongTienID", result.DaPhuongTienID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TieuDe", noiDung.TieuDe);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.DaPhuongTien_NoiDung>(
                                        "spu_DM_DaPhuongTien_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.DanhMuc.DaPhuongTien>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.DanhMuc.DaPhuongTien>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
