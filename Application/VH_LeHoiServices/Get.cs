using Dapper;
using Domain.Core;
using Domain.VH_LeHoiModel;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.VH_LeHoiServices
{
    public class Get
    {
        public class Query : IRequest<Result<VH_LeHoiViewModel>>
        {
            public string MaNgonNgu;
            public Guid LeHoiID;
        }

        public class Handler : IRequestHandler<Query, Result<VH_LeHoiViewModel>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<VH_LeHoiViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        parameters.Add("@LeHoiID", request.LeHoiID);
                        var result = await connection.QueryFirstOrDefaultAsync<VH_LeHoiViewModel>("spu_VH_LeHoi_Get", parameters, commandType: CommandType.StoredProcedure);
                        return Result<VH_LeHoiViewModel>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<VH_LeHoiViewModel>.Failure(ex.Message);
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
