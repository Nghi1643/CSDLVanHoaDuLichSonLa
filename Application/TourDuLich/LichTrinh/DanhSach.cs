using Dapper;
using Domain.Core;
using Domain.TourDuLich;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.TourDuLich.LichTrinh
{
    public class DanhSach : IRequest<Result<List<TourDuLich_LichTrinhDTO>>>
    {
        public class Query : IRequest<Result<List<TourDuLich_LichTrinhDTO>>>
        {
            public Guid TourID { get; set; }
            public string MaNgonNgu { get; set; }

        }
        public class Handler : IRequestHandler<Query, Result<List<TourDuLich_LichTrinhDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<TourDuLich_LichTrinhDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync(cancellationToken);
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@TourID", request.TourID);
                    parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                    var queryResult = await connection.QueryAsync<TourDuLich_LichTrinhDTO>(
                        "spu_DL_TourDuLich_LichTrinh_GetsByTour",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<List<TourDuLich_LichTrinhDTO>>.Success(queryResult?.ToList());
                }
                catch (Exception ex)
                {
                    return Result<List<TourDuLich_LichTrinhDTO>>.Failure(ex.Message);
                }
            }
        }
    }
}

