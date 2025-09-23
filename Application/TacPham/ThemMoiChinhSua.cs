using Dapper;
using Domain.Core;
using Domain.DanhMuc;
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

namespace Application.TacPham
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.DanhMuc.TacPham>>
        {
            public TacPhamAdd Data { get; set; }
            public List<TacPham_NoiDungAdd> BanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.DanhMuc.TacPham>>
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

            public async Task<Result<Domain.DanhMuc.TacPham>> Handle(Command request, CancellationToken cancellationToken)
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
                            parameters.Add("@TacPhamID", request.Data.TacPhamID);
                            parameters.Add("@LinhVucID", request.Data.LinhVucID);
                            parameters.Add("@MaDinhDanh", request.Data.MaDinhDanh);
                            parameters.Add("@TheLoaiID", request.Data.TheLoaiID);
                            parameters.Add("@NamCongBo", request.Data.NamCongBo);
                            parameters.Add("@HinhAnh", request.Data.HinhAnh);
                            parameters.Add("@ThuTu", request.Data.ThuTu);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);
                            parameters.Add("@SuDung", request.Data.SuDung);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.TacPham>(
                                "spu_DM_TacPham_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.BanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@TacPhamID", result.TacPhamID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TenTacPham", noiDung.TenTacPham);
                                    parametersNoiDung.Add("@TacGia", noiDung.TacGia);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);
                                    parametersNoiDung.Add("@NoiDung", noiDung.NoiDung);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<TacPham_NoiDung>(
                                        "spu_DM_TacPham_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                return Result<Domain.DanhMuc.TacPham>.Failure("Mã định danh đã tồn tại");
                            }

                                transaction.Commit(); // ok
                            return Result<Domain.DanhMuc.TacPham>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // err
                            return Result<Domain.DanhMuc.TacPham>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
