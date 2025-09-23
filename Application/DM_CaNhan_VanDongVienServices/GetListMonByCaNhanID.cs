using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.DM_CaNhan_VanDongVienServices
{
    public class GetListMonByCaNhanID
    {
        public class Query : IRequest<Result<IEnumerable<DM_CaNhan_MonTheThaoList>>>
        {
            public Guid CaNhanID;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<DM_CaNhan_MonTheThaoList>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<DM_CaNhan_MonTheThaoList>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryAsync<DM_CaNhan_MonTheThaoList>("spu_DM_CaNhan_MonTheThao_GetListByCaNhanID", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<DM_CaNhan_MonTheThaoList>>.Success(result);
                    }catch (Exception ex)
                    {
                        return Result<IEnumerable<DM_CaNhan_MonTheThaoList>>.Failure(ex.Message);
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
