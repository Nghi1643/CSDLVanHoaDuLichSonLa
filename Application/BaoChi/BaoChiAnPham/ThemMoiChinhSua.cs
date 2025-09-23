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

namespace Application.BaoChi.BaoChiAnPham
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.BaoChi.BaoChiAnPham>>
        {
            public Domain.BaoChi.BaoChiAnPham BCAnPham { get; set; }
            public List<BaoChiAnPham_NoiDung> NoiDungBanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.BaoChi.BaoChiAnPham>>
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

            public async Task<Result<Domain.BaoChi.BaoChiAnPham>> Handle(Command request, CancellationToken cancellationToken)
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
                            parameters.Add("@BaoChiAnPhamID", request.BCAnPham.BaoChiAnPhamID);
                            parameters.Add("@TenAnPham", request.BCAnPham.TenAnPham);
                            parameters.Add("@ToChucID", request.BCAnPham.ToChucID);
                            parameters.Add("@TanSuatID", request.BCAnPham.TanSuatID);
                            parameters.Add("@LinhVucChuyenSauID", request.BCAnPham.LinhVucChuyenSauID);
                            parameters.Add("@SoLuong", request.BCAnPham.SoLuong);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);
                            parameters.Add("@TrangThai", request.BCAnPham.TrangThai);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.BaoChi.BaoChiAnPham>(
                                "spu_BC_BaoChiAnPham_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@BaoChiAnPhamID", result.BaoChiAnPhamID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TenAnPham", noiDung.TenAnPham);
                                    parametersNoiDung.Add("@DoiTuongDocGia", noiDung.DoiTuongDocGia);
                                    parametersNoiDung.Add("@LinhVucChuyenSau", noiDung.LinhVucChuyenSau);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.BaoChi.BaoChiAnPham_NoiDung>(
                                        "spu_BC_BaoChiAnPham_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.BaoChi.BaoChiAnPham>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.BaoChi.BaoChiAnPham>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
