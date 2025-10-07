using Dapper;
using Domain.Core;
using Domain.ThongTinXucTien;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
namespace Application.ThongTinXucTien
{
    public class ChiTiet
    {
        public class Query :IRequest<Result<ThongTinXucTienDTO>>
        {
            public Guid XucTienID { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<ThongTinXucTienDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)=>_config = config;
            
            public async Task<Result<ThongTinXucTienDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    var parameters = new DynamicParameters();
                    parameters.Add("@XucTienID", request.XucTienID);
                    var queryResult = await connection.QueryFirstOrDefaultAsync<ThongTinXucTienDTO>("spu_DL_ThongTinXucTien_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);
                    return Result<ThongTinXucTienDTO>.Success(queryResult);
                }
                catch (Exception ex)
                {
                    return Result<ThongTinXucTienDTO>.Failure(ex.Message);
                }
            }
        }
    }
}
