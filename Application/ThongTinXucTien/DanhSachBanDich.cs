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
    public class DanhSachBanDich
    {
        public class Query : IRequest<Result<List<ThongTinXucTien_NoiDung>>>
        {
            public Guid? XucTienID { get; set; }
            public string MaNgonNgu { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ThongTinXucTien_NoiDung>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<Result<List<ThongTinXucTien_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@XucTienID", request.XucTienID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);

                        var queryResult = await connection.QueryAsync<ThongTinXucTien_NoiDung>(
                            "spu_DL_ThongTinXucTien_NoiDung_GetFilter",
                            parameters,
                            commandType: System.Data.CommandType.StoredProcedure
                        );

                        return Result<List<ThongTinXucTien_NoiDung>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<ThongTinXucTien_NoiDung>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}