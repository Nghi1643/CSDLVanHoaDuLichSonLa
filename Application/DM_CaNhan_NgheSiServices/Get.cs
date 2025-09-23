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
using Domain.DM_CaNhan_HuongDanVienModel;
using Domain.DM_CaNhan_NgheSiModel;

namespace Application.DM_CaNhan_NgheSiServices
{
    public class Get
    {
        public class Query : IRequest<Result<DM_CaNhan_NgheSiViewModel>>
        {
            public Guid CaNhanID;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<DM_CaNhan_NgheSiViewModel>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_CaNhan_NgheSiViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_NgheSiViewModel>("spu_DM_CaNhan_NgheSi_Get", parameters, commandType: CommandType.StoredProcedure);
                        return Result<DM_CaNhan_NgheSiViewModel>.Success(result);
                    }catch (Exception ex)
                    {
                        return Result<DM_CaNhan_NgheSiViewModel>.Failure(ex.Message);
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
