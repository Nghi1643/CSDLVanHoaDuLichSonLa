using Dapper;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BaoChi.ViPham
{
    public class DanhSachBanDich
    {
        /// <summary>
        /// Danh sách các bản dịch của một danh mục chung(địa điểm, nghề nghiệp,...)
        /// </summary>
        public class Query : IRequest<Result<List<ViPham_NoiDung>>>
        {
            public Guid? MaViPhamID { get; set; }
            public string MaNgonNgu { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ViPham_NoiDung>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<ViPham_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaViPhamID", request.MaViPhamID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var queryResult = await connettion.QueryAsync<ViPham_NoiDung>("spu_BC_ViPham_NoiDung_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<ViPham_NoiDung>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<ViPham_NoiDung>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
