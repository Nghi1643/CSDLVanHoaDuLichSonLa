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
    public class AddRelationDiaDiem
    {
        public class Command : IRequest<Result<DM_SuKien_DiaDiem>>
        {
            public DM_SuKien_DiaDiem Entity;
        }

        public class Handler : IRequestHandler<Command, Result<DM_SuKien_DiaDiem>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_SuKien_DiaDiem>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@DiaDiemID", request.Entity.DiaDiemID);
                            parameters.Add("@SuKienID", request.Entity.SuKienID);
                            var result = await connection.QueryFirstOrDefaultAsync<DM_SuKien_DiaDiem>("spu_DM_SuKien_DiaDiem_Add", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            return Result<DM_SuKien_DiaDiem>.Success(result);
                        }
                        catch (Exception ex) {
                            transaction.Rollback();
                            return Result<DM_SuKien_DiaDiem>.Failure(ex.Message);
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
