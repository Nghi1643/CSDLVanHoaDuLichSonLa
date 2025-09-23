using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Domain.TT_MonTheThaoModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.TT_MonTheThaoServices
{
    public class GetListForCaNhan
    {
        public class Query : IRequest<Result<IEnumerable<TT_MonTheThaoViewModel>>>
        {
            public Guid CaNhanID;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<TT_MonTheThaoViewModel>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<TT_MonTheThaoViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryAsync<TT_MonTheThaoViewModel>("spu_TT_MonTheThao_GetListForCaNhan", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<TT_MonTheThaoViewModel>>.Success(result);
                    }catch(Exception ex)
                    {
                        return Result<IEnumerable<TT_MonTheThaoViewModel>>.Failure(ex.Message);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
        }
    }
}
