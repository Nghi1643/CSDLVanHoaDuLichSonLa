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
namespace Application.TourDuLich.TheLoai
{
    public class DanhSach 
    {
        public class Query : IRequest<Result<List<TourDuLich_TheLoaiDTO>>>
        {
            public string MaNgonNgu { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<TourDuLich_TheLoaiDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<TourDuLich_TheLoaiDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync(cancellationToken);
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                    var queryResult = await connection.QueryAsync<TourDuLich_TheLoaiDTO>(
                        "spu_DL_TourDuLich_TheLoai_Gets",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<List<TourDuLich_TheLoaiDTO>>.Success(queryResult?.ToList());
                }
                catch (Exception ex)
                {
                    return Result<List<TourDuLich_TheLoaiDTO>>.Failure(ex.Message);
                }
            }
        }
    }
  
}
