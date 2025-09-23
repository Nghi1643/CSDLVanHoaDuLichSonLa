using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Domain.DM_SuKienModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.DM_SuKienServices
{
    public class AddRelationToChuc
    {
        public class Command : IRequest<Result<DM_SuKien_ToChuc>>
        {
            public DM_SuKien_ToChuc Entity;
        }

        public class Handler : IRequestHandler<Command, Result<DM_SuKien_ToChuc>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_SuKien_ToChuc>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@SuKienID", request.Entity.SuKienID);
                            parameters.Add("@ToChucID", request.Entity.ToChucID);
                            parameters.Add("@VaiTroID", request.Entity.VaiTroID);
                            parameters.Add("@XepHangID", request.Entity.XepHangID);
                            var result = await connection.QueryFirstOrDefaultAsync<DM_SuKien_ToChuc>("spu_DM_SuKien_ToChuc_Add", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            return Result<DM_SuKien_ToChuc>.Success(result);
                        }
                        catch (Exception ex) {
                            transaction.Rollback();
                            return Result<DM_SuKien_ToChuc>.Failure(ex.Message);
                        }
                        finally
                        {
                            transaction.Commit();
                            await connection.CloseAsync();
                        }
                    }
                }
            }
        }
    }
}
