using Dapper;
using Domain.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.ThongTinXucTien
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.ThongTinXucTien.ThongTinXucTien>>
        {
            public Domain.ThongTinXucTien.ThongTinXucTien ThongTinXucTien { get; set; }
            public List<Domain.ThongTinXucTien.ThongTinXucTien_NoiDung> NoiDung { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Domain.ThongTinXucTien.ThongTinXucTien>>
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
            public async Task<Result<Domain.ThongTinXucTien.ThongTinXucTien>> Handle(Command request, CancellationToken cancellationToken)
            {
                
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    await using var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@XucTienID", request.ThongTinXucTien.XucTienID);
                    parameters.Add("@LinhVucID", request.ThongTinXucTien.LinhVucID);
                    parameters.Add("@ToChucID", request.ThongTinXucTien.ToChucID);
                    parameters.Add("@DoiTuongID", request.ThongTinXucTien.DoiTuongID);
                    parameters.Add("@HinhThucID", request.ThongTinXucTien.HinhThucID);
                    parameters.Add("@TruyenThongID", request.ThongTinXucTien.TruyenThongID);
                    parameters.Add("@NgayBatDau", request.ThongTinXucTien.NgayBatDau);
                    parameters.Add("@NgayKetThuc", request.ThongTinXucTien.NgayKetThuc);
                    parameters.Add("@TrangThaiID", request.ThongTinXucTien.TrangThaiID);

                    var result = await connection.QueryFirstOrDefaultAsync<Domain.ThongTinXucTien.ThongTinXucTien>(
                        "spu_DL_ThongTinXucTien_AddEdit",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure,
                         transaction: transaction);
                    if (result != null)
                    {
                        foreach (var noiDung in request.NoiDung)
                        {
                            var paramNoiDung = new DynamicParameters();
                            paramNoiDung.Add("@XucTienID", result.XucTienID);
                            paramNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                            paramNoiDung.Add("@TieuDe", noiDung.TieuDe);
                            paramNoiDung.Add("@MoTa", noiDung.MoTa);
                            await connection.QueryFirstOrDefaultAsync(
                                "spu_DL_ThongTinXucTien_NoiDung_AddEdit",
                                paramNoiDung,
                                commandType: System.Data.CommandType.StoredProcedure,
                                 transaction: transaction);
                        }
                    }
                    transaction.Commit();
                    return Result<Domain.ThongTinXucTien.ThongTinXucTien>.Success(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<Domain.ThongTinXucTien.ThongTinXucTien>.Failure(ex.Message);
                }
            }
        }
    }
}
