using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Domain.VH_LeHoiModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.VH_LeHoiServices
{
    public class GetListContent
    {
        public class Query : IRequest<Result<IEnumerable<VH_LeHoi_NoiDung>>>
        {
            public Guid LeHoiID;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<VH_LeHoi_NoiDung>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<VH_LeHoi_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@LeHoiID", request.LeHoiID);
                        var result = await connection.QueryAsync<VH_LeHoi_NoiDung>("spu_VH_LeHoi_NoiDung_GetListByLeHoiID", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<VH_LeHoi_NoiDung>>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<IEnumerable<VH_LeHoi_NoiDung>>.Failure(ex.Message);
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
