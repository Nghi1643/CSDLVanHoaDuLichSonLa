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
    public class Get
    {
        public class Query : IRequest<Result<DM_SuKienViewModel>>
        {
            public Guid SuKienID;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<DM_SuKienViewModel>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_SuKienViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction()) {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@SuKienID", request.SuKienID);
                            parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                            var result = await connection.QueryFirstOrDefaultAsync<DM_SuKienViewModel>("spu_DM_SuKien_Get", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            return Result<DM_SuKienViewModel>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Result<DM_SuKienViewModel>.Failure(ex.Message);
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
