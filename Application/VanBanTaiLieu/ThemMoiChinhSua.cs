using Dapper;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.VanBanTaiLieu
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.DanhMuc.VanBanTaiLieu>>
        {
            public VanBanTaiLieuAdd vbtl { get; set; }

        }

        public class Handler : IRequestHandler<Command, Result<Domain.DanhMuc.VanBanTaiLieu>>
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

            public async Task<Result<Domain.DanhMuc.VanBanTaiLieu>> Handle(Command request, CancellationToken cancellationToken)
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
                            parameters.Add("@VanBanID", request.vbtl.VanBanID);
                            parameters.Add("@DoiTuongSoHuuID", request.vbtl.DoiTuongSoHuuID);
                            parameters.Add("@BangThamChieu", request.vbtl.BangThamChieu);
                            parameters.Add("@SoKyHieu", request.vbtl.SoKyHieu);
                            parameters.Add("@TrichYeu", request.vbtl.TrichYeu);
                            parameters.Add("@CoQuanBanHanhID", request.vbtl.CoQuanBanHanhID);
                            parameters.Add("@NguoiKy", request.vbtl.NguoiKy);
                            parameters.Add("@NoiDung", request.vbtl.NoiDung);
                            parameters.Add("@NgayHieuLuc", request.vbtl.NgayHieuLuc);
                            parameters.Add("@NgayBanHanh", request.vbtl.NgayBanHanh);
                            parameters.Add("@NguoiCapNhat", userId);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.VanBanTaiLieu>(
                                "spu_DM_VanBanTaiLieu_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );
                            if (result != null)
                            {
                                var parametersNoiDung = new DynamicParameters();
                                parametersNoiDung.Add("@VanBanID", result.VanBanID);
                                parametersNoiDung.Add("@DuongDanFile", request.vbtl.DuongDanFile);

                                var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.VanBan_FileDinhKem>(
                                    "spu_DM_VanBan_FileDinhKem_AddEdit",
                                    parametersNoiDung,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: transaction
                                );
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.DanhMuc.VanBanTaiLieu>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.DanhMuc.VanBanTaiLieu>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
