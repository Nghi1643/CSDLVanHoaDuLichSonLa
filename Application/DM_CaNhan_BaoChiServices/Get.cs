using Dapper;
using Domain.Core;
using Domain.DM_CaNhan_BaoChiModel;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DM_CaNhan_BaoChiServices
{
    public class Get
    {
        public class Query : IRequest<Result<DM_CaNhan_BaoChiViewModel>>
        {
            public Guid CaNhanID;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<DM_CaNhan_BaoChiViewModel>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_CaNhan_BaoChiViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_BaoChiViewModel>("spu_DM_CaNhan_BaoChi_Get", parameters, commandType: CommandType.StoredProcedure);
                        return Result<DM_CaNhan_BaoChiViewModel>.Success(result);
                    }catch (Exception ex)
                    {
                        return Result<DM_CaNhan_BaoChiViewModel>.Failure(ex.Message);
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
