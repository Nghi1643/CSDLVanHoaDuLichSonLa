using Dapper;
using Domain.BaoChi;
using Domain.Core;
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

namespace Application.BaoChi.ViPham
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.BaoChi.ViPham>>
        {
            public Domain.BaoChi.ViPham ViPham { get; set; }
            public List<ViPham_NoiDung> NoiDungBanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.BaoChi.ViPham>>
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

            public async Task<Result<Domain.BaoChi.ViPham>> Handle(Command request, CancellationToken cancellationToken)
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
                            parameters.Add("@MaViPham", request.ViPham.MaViPham);
                            parameters.Add("@NgayPhatHien", request.ViPham.NgayPhatHien);
                            parameters.Add("@HinhThucXuLyID", request.ViPham.HinhThucXuLyID);
                            parameters.Add("@DonViViPhamID", request.ViPham.DonViViPhamID);
                            parameters.Add("@LoaiDonVi", request.ViPham.LoaiDonVi);
                            parameters.Add("@TrangThaiXuly", request.ViPham.TrangThaiXuly);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.BaoChi.ViPham>(
                                "spu_BC_ViPham_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@MaViPhamID", result.MaViPham);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@NoiDungViPham", noiDung.NoiDungViPham);
                                    parametersNoiDung.Add("@MoTaViPham", noiDung.MoTaViPham);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.DanhMucChung_NoiDung>(
                                        "spu_BC_ViPham_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.BaoChi.ViPham>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.BaoChi.ViPham>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
