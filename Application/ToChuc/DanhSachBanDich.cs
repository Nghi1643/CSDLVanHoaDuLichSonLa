using Dapper;
using Domain.Core;
using Domain.DanhMuc;
using Domain.ToChuc;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ToChuc
{
    public class DanhSachBanDich
    {
        /// <summary>
        /// Danh sách các bản dịch của một danh mục chung(địa điểm, nghề nghiệp,...)
        /// </summary>
        public class Query : IRequest<Result<List<ToChuc_NoiDung>>>
        {
            public Guid? ToChucID { get; set; }
            public string MaNgonNgu { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ToChuc_NoiDung>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<ToChuc_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@ToChucID", request.ToChucID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var queryResult = await connettion.QueryAsync<ToChuc_NoiDung>("spu_DM_ToChuc_NoiDung_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<ToChuc_NoiDung>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<ToChuc_NoiDung>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
