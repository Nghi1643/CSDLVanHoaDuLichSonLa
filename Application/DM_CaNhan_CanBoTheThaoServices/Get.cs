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

namespace Application.DM_CaNhan_CanBoTheThaoServices
{
    public class Get
    {
        public class Query : IRequest<Result<DM_CaNhan_CanBoTheThaoViewModel>>
        {
            public string MaNgonNgu;
            public Guid CaNhanID;
        }

        public class Handler : IRequestHandler<Query, Result<DM_CaNhan_CanBoTheThaoViewModel>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_CaNhan_CanBoTheThaoViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        var result = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_CanBoTheThaoViewModel>("spu_DM_CaNhan_CanBoTheThao_Get", parameters, commandType: CommandType.StoredProcedure);
                        return Result<DM_CaNhan_CanBoTheThaoViewModel>.Success(result);
                    }catch (Exception ex )
                    {
                        return Result<DM_CaNhan_CanBoTheThaoViewModel>.Failure(ex.Message);
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
