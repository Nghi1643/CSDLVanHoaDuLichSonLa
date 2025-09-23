using Dapper;
using Domain.Core;
using Domain.TT_MonTheThaoModel;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TT_MonTheThaoServices
{
    public class Gets
    {
        public class Query : IRequest<Result<IEnumerable<TT_MonTheThaoViewModel>>>
        {
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
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryAsync<TT_MonTheThaoViewModel>("spu_TT_MonTheThao_Gets", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<TT_MonTheThaoViewModel>>.Success(result);
                    }
                    catch (Exception ex)
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
