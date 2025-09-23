using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Domain.DiaDiem;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.DiaDiem
{
    public class GetMulti
    {
        public class Query : IRequest<Result<IEnumerable<DiaDiemDTO>>>
        {
            public string ListID;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<DiaDiemDTO>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<DiaDiemDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@ListDiaDiemID", request.ListID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryAsync<DiaDiemDTO>("spu_DM_DiaDiem_GetMulti", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<DiaDiemDTO>>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<IEnumerable<DiaDiemDTO>>.Failure(ex.Message);
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
