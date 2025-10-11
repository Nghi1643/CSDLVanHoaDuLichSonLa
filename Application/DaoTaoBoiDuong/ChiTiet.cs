using Dapper;
using Domain.Core;
using Domain.DaoTaoBoiDuong;
using Domain.ThongTinXucTien;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.DaoTaoBoiDuong
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<DaoTaoBoiDuongDTO>>
        {
            public Guid DaoTaoID { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<DaoTaoBoiDuongDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config) => _config = config;

            public async Task<Result<DaoTaoBoiDuongDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    var parameters = new DynamicParameters();
                    parameters.Add("@DaoTaoID", request.DaoTaoID);
                    var queryResult = await connection.QueryFirstOrDefaultAsync<DaoTaoBoiDuongDTO>(
                        "spu_DM_DaoTaoBoiDuong_Get",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<DaoTaoBoiDuongDTO>.Success(queryResult);
                }
                catch (Exception ex)
                {
                    return Result<DaoTaoBoiDuongDTO>.Failure(ex.Message);
                }
            }
        }
    }
}
