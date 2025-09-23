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

namespace Application.DM_CaNhan_TrongTaiServices
{
    public class Gets
    {
        public class Query : IRequest<Result<IEnumerable<DM_CaNhan_TrongTaiViewModel>>>
        {
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<DM_CaNhan_TrongTaiViewModel>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<DM_CaNhan_TrongTaiViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryAsync<DM_CaNhan_TrongTaiViewModel>("spu_DM_CaNhan_TrongTai_Gets", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<DM_CaNhan_TrongTaiViewModel>>.Success(result);
                    }catch(Exception ex)
                    {
                        return Result<IEnumerable<DM_CaNhan_TrongTaiViewModel>>.Failure(ex.Message);
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
