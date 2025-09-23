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
    /// Danh sách bản dịch (nội dung) của một giải thưởng
    /// SP: spu_BC_GiaiThuong_NoiDung_GetFilter
    /// </summary>
    public class DanhSachBanDich
    {
        public class Query : IRequest<Result<List<GiaiThuong_NoiDung>>>
        {
            public Guid? GiaiThuongID { get; set; }
            public string MaNgonNgu { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<GiaiThuong_NoiDung>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<GiaiThuong_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@GiaiThuongID", request.GiaiThuongID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var queryResult = await connection.QueryAsync<GiaiThuong_NoiDung>("spu_BC_GiaiThuong_NoiDung_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        return Result<List<GiaiThuong_NoiDung>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<GiaiThuong_NoiDung>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
