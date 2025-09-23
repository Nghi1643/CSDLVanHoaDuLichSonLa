using Dapper;
using Domain.BaoChi;
using Domain.Core;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.BaoChi.GiaiThuong
{
    /// <summary>
    /// Danh sách giải thưởng (lọc)
    /// Gọi SP: spu_BC_GiaiThuong_GetFilter
    /// </summary>
    public class DanhSach
    {
        public class Query : IRequest<Result<List<GiaiThuongDTO>>>
        {
            public GiaiThuongRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<GiaiThuongDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<GiaiThuongDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@GiaiThuongID", request.Data.GiaiThuongID);
                        parameters.Add("@NamTraoGiai", request.Data.NamTraoGiai);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TrangThai", request.Data.TrangThai);

                        var queryResult = await connection.QueryAsync<GiaiThuongDTO>("spu_BC_GiaiThuong_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        return Result<List<GiaiThuongDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<GiaiThuongDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
