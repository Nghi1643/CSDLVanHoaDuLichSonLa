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
namespace Application.DaoTaoBoiDuong
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.DaoTaoBoiDuong.DaoTaoBoiDuong>>
        {
            public Domain.DaoTaoBoiDuong.DaoTaoBoiDuong DaoTaoBoiDuong { get; set; }
            public List<Domain.DaoTaoBoiDuong.DaoTaoBoiDuong_NoiDung> NoiDung { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Domain.DaoTaoBoiDuong.DaoTaoBoiDuong>>
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
            public async Task<Result<Domain.DaoTaoBoiDuong.DaoTaoBoiDuong>> Handle(Command request, CancellationToken cancellationToken)
            {

                await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync(cancellationToken);
                await using var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@DaoTaoID", request.DaoTaoBoiDuong.DaoTaoID);
                    parameters.Add("@LinhVucID", request.DaoTaoBoiDuong.LinhVucID);
                    parameters.Add("@ToChucID", request.DaoTaoBoiDuong.ToChucID);
                    parameters.Add("@DiaDiemID", request.DaoTaoBoiDuong.DiaDiemID);
                    parameters.Add("@BatDau", request.DaoTaoBoiDuong.BatDau);
                    parameters.Add("@KetThuc", request.DaoTaoBoiDuong.KetThuc);
                    parameters.Add("@TrangThaiID", request.DaoTaoBoiDuong.TrangThaiID);

                    var result = await connection.QueryFirstOrDefaultAsync<Domain.DaoTaoBoiDuong.DaoTaoBoiDuong>(
                        "spu_DM_DaoTaoBoiDuong_AddEdit",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure,
                         transaction: transaction);
                    if (result != null)
                    {
                        foreach (var noiDung in request.NoiDung)
                        {
                            var paramNoiDung = new DynamicParameters();
                            paramNoiDung.Add("@DaoTaoID", result.DaoTaoID);
                            paramNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                            paramNoiDung.Add("@NoiDungDaoTao", noiDung.NoiDungDaoTao);
                            paramNoiDung.Add("@DonVi", noiDung.DonVi);
                            await connection.QueryFirstOrDefaultAsync(
                                "spu_DM_DaoTaoBoiDuong_NoiDung_AddEdit",
                                paramNoiDung,
                                commandType: System.Data.CommandType.StoredProcedure,
                                 transaction: transaction);
                        }
                    }
                    transaction.Commit();
                    return Result<Domain.DaoTaoBoiDuong.DaoTaoBoiDuong>.Success(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<Domain.DaoTaoBoiDuong.DaoTaoBoiDuong>.Failure(ex.Message);
                }
            }
        }
    }
}
