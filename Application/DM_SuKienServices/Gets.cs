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
    public class Gets
    {
        public class Query : IRequest<Result<IEnumerable<DM_SuKienViewModel>>>
        {
            public string MaNgonNgu;
            public int LinhVucID;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<DM_SuKienViewModel>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<DM_SuKienViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction()) {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                            parameters.Add("@LinhVucID", request.LinhVucID);
                            var result = await connection.QueryAsync<DM_SuKienViewModel>("spu_DM_SuKien_Gets", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            return Result<IEnumerable<DM_SuKienViewModel>>.Success(result);
                        }
                        catch (Exception ex) {
                            return Result<IEnumerable<DM_SuKienViewModel>>.Failure(ex.Message);
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
