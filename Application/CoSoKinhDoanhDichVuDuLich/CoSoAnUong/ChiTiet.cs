using Domain.Core;
using Domain.CoSoKinhDoanhDichVuDuLich;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
namespace Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong
{
    public class ChiTiet
    {
        public class Query: IRequest<Result<CoSoAnUongDTO>>
        {
            public Guid CoSoAnUongID { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<CoSoAnUongDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<CoSoAnUongDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@CoSoAnUongID", request.CoSoAnUongID);
                    var queryResult = await connection.QueryFirstOrDefaultAsync<CoSoAnUongDTO>("spu_DL_CoSoAnUong_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);
                    return Result<CoSoAnUongDTO>.Success(queryResult); 
                }
                catch (Exception ex)
                {
                    return Result<CoSoAnUongDTO>.Failure(ex.Message);
                }
            }
        }
    }
}
