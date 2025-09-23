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
    public class Gets
    {
        public class Query : IRequest<Result<IEnumerable<VH_LeHoiViewModel>>>
        {
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<VH_LeHoiViewModel>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<VH_LeHoiViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryAsync<VH_LeHoiViewModel>("spu_VH_LeHoi_Gets", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<VH_LeHoiViewModel>>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<IEnumerable<VH_LeHoiViewModel>>.Failure(ex.Message);
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
