using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.DM_SuKienServices
{
    public class Delete
    {
        public class Command : IRequest<Result<int>>
        {
            public Guid SuKienID;
        }

        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction()) {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@SuKienID", request.SuKienID);
                            var result = await connection.ExecuteAsync("spu_DM_SuKien_Delete", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            return Result<int>.Success(result);
                        }
                        catch (Exception ex) {
                            transaction.Rollback();
                            return Result<int>.Failure(ex.Message);
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
