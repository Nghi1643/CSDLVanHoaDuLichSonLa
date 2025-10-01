using Dapper;
using Domain.Core;
using Domain.CoSoKinhDoanhDichVuDuLich;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong
{
    /// <summary>
    /// Danh sach co so an uong
    /// </summary>
    public class DanhSach
    {
        public class Query : IRequest<Result<List<CoSoAnUongDTO>>>
        {
            public CoSoAnUongRequest Data { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<CoSoAnUongDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<CoSoAnUongDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);

                    var parameters = new DynamicParameters();
                    parameters.Add("@TuKhoa", request.Data.TuKhoa);
                    parameters.Add("@LoaiDichVuAnUongID", request.Data.LoaiDichVuAnUongID);
                    parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                    parameters.Add("@TrangThai", request.Data.TrangThai);

                    var queryResult = await connection.QueryAsync<CoSoAnUongDTO>("spu_DL_CoSoAnUong_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);
                    return Result<List<CoSoAnUongDTO>>.Success(queryResult?.ToList());
                }
                catch (Exception ex)
                {
                    return Result<List<CoSoAnUongDTO>>.Failure(ex.Message);
                }
            }
        }
    }
}
