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
    public class Get
    {
        public class Query : IRequest<Result<TT_MonTheThao>>
        {
            public string MaNgonNgu;
            public Guid MonTheThaoID;
        }

        public class Handler : IRequestHandler<Query, Result<TT_MonTheThao>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<TT_MonTheThao>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        parameters.Add("@MonTheThaoID", request.MonTheThaoID);
                        var result = await connection.QueryFirstOrDefaultAsync<TT_MonTheThao>("spu_TT_MonTheThao_Gets", parameters, commandType: CommandType.StoredProcedure);
                        return Result<TT_MonTheThao>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<TT_MonTheThao>.Failure(ex.Message);
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
