using Dapper;
using Domain.Core;
using Domain.ThongTinXucTien;
using Domain.TourDuLich;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Application.TourDuLich.LichTrinh
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<TourDuLich_LichTrinhDTO>>
        {
            public int LichTrinhID { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<TourDuLich_LichTrinhDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config) => _config = config;

            public async Task<Result<TourDuLich_LichTrinhDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    var parameters = new DynamicParameters();
                    parameters.Add("@LichTrinhID", request.LichTrinhID);
                    var queryResult = await connection.QueryFirstOrDefaultAsync<TourDuLich_LichTrinhDTO>(
                        "spu_DL_TourDuLich_LichTrinh_Get", 
                        parameters, 
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<TourDuLich_LichTrinhDTO>.Success(queryResult);
                }
                catch (Exception ex)
                {
                    return Result<TourDuLich_LichTrinhDTO>.Failure(ex.Message);
                }
            }
        }
    }
}
