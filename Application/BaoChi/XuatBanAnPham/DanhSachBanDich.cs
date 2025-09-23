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

namespace Application.BaoChi.XuatBanAnPham
{
    public class DanhSachBanDich
    {
        /// <summary>
        /// Danh sách các bản dịch của một danh mục chung(địa điểm, nghề nghiệp,...)
        /// </summary>
        public class Query : IRequest<Result<List<XuatBanAnPham_NoiDung>>>
        {
            public Guid? XuatBanAnPhamID { get; set; }
            public string MaNgonNgu { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<XuatBanAnPham_NoiDung>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<XuatBanAnPham_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@XuatBanAnPhamID", request.XuatBanAnPhamID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var queryResult = await connettion.QueryAsync<XuatBanAnPham_NoiDung>("spu_BC_XuatBanAnPham_NoiDung_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<XuatBanAnPham_NoiDung>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<XuatBanAnPham_NoiDung>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
