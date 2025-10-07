using Dapper;
using Domain.Core;
using Domain.ThongTinXucTien;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ThongTinXucTien
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<ThongTinXucTienDTO>>>
        {
            public ThongTinXucTienRequest Data { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<ThongTinXucTienDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<ThongTinXucTienDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    var parameters = new DynamicParameters();
                    parameters.Add("@TuKhoa", request.Data.TuKhoa);
                    parameters.Add("@HinhThucID", request.Data.HinhThucID);
                    parameters.Add("@TruyenThongID", request.Data.TruyenThongID);
                    parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                    parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                    var queryResult = await connection.QueryAsync<ThongTinXucTienDTO>(
                        "spu_DL_ThongTinXucTien_GetFilter", 
                        parameters, 
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<List<ThongTinXucTienDTO>>.Success(queryResult?.ToList());
                }
                catch (Exception ex)
                {
                    return Result<List<ThongTinXucTienDTO>>.Failure(ex.Message);
                }
            }
        }
    }
}
