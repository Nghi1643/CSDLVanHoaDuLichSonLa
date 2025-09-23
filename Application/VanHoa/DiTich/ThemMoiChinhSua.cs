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

namespace Application.VanHoa.DiTich
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.VanHoa.DiTich>>
        {
            public Domain.VanHoa.DiTich Data { get; set; }
            public List<Domain.VanHoa.DiTich_NoiDung> NoiDungBanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.VanHoa.DiTich>>
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

            public async Task<Result<Domain.VanHoa.DiTich>> Handle(Command request, CancellationToken cancellationToken)
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
                            parameters.Add("@DiTichID", request.Data.DiTichID);
                            parameters.Add("@TenDiTich", request.Data.TenDiTich);
                            parameters.Add("@MaDinhDanh", request.Data.MaDinhDanh);
                            parameters.Add("@XepHangID", request.Data.XepHangID);
                            parameters.Add("@LoaiHinhID", request.Data.LoaiHinhID);
                            parameters.Add("@NgayCongNhan", request.Data.NgayCongNhan);
                            parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                            parameters.Add("@ToChucQuanLyID", request.Data.ToChucQuanLyID);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);
                            parameters.Add("@SuDung", request.Data.SuDung);
                            parameters.Add("@AnhDaiDien", request.Data.AnhDaiDien);
                            parameters.Add("@ThuTu", request.Data.ThuTu);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.VanHoa.DiTich>(
                                "spu_VH_DiTich_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@DiTichID", result.DiTichID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TenDiTich", noiDung.TenDiTich);
                                    parametersNoiDung.Add("@TenGoiKhac", noiDung.TenGoiKhac);
                                    parametersNoiDung.Add("@NienDai", noiDung.NienDai);
                                    parametersNoiDung.Add("@MoTaDiTich", noiDung.MoTaDiTich);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<DiTich_NoiDung>(
                                        "spu_VH_DiTich_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                return Result<Domain.VanHoa.DiTich>.Failure("Mã định danh đã tồn tại");
                            }

                                transaction.Commit(); // ok
                            return Result<Domain.VanHoa.DiTich>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.VanHoa.DiTich>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
