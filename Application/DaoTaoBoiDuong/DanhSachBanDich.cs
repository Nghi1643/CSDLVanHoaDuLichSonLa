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
    public class DanhSachBanDich
    {
        public class Query : IRequest<Result<List<DaoTaoBoiDuong_NoiDung>>>
        {
            public Guid? DaoTaoID { get; set; }
            public string MaNgonNgu { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<DaoTaoBoiDuong_NoiDung>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<Result<List<DaoTaoBoiDuong_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@DaoTaoID", request.DaoTaoID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);

                        var queryResult = await connection.QueryAsync<DaoTaoBoiDuong_NoiDung>(
                            "spu_DM_DaoTaoBoiDuong_NoiDung_GetFilter",
                            parameters,
                            commandType: System.Data.CommandType.StoredProcedure
                        );

                        return Result<List<DaoTaoBoiDuong_NoiDung>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<DaoTaoBoiDuong_NoiDung>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}