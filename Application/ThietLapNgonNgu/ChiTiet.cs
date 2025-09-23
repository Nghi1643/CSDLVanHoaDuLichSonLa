using Dapper;
using Domain.Core;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ThietLapNgonNgu
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<Domain.ThietLapNgonNgu>>
        {
            public int ID { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Domain.ThietLapNgonNgu>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<Domain.ThietLapNgonNgu>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@ID", request.ID);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<Domain.ThietLapNgonNgu>("spu_CSDL_ThietLapNgonNgu_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<Domain.ThietLapNgonNgu>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<Domain.ThietLapNgonNgu>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
