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

namespace Application.TourDuLich.TourDuLich
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<TourDuLichDTO>>
        {
            public Guid TourID { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<TourDuLichDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config) => _config = config;

            public async Task<Result<TourDuLichDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    var parameters = new DynamicParameters();
                    parameters.Add("@TourID", request.TourID);
                    var queryResult = await connection.QueryFirstOrDefaultAsync<TourDuLichDTO>(
                        "spu_DL_TourDuLich_Get", 
                        parameters, 
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<TourDuLichDTO>.Success(queryResult);
                }
                catch (Exception ex)
                {
                    return Result<TourDuLichDTO>.Failure(ex.Message);
                }
            }
        }
    }
}
