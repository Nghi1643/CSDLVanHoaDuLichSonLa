using Dapper;
using Domain.Core;
using Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Domain.BaoChi;

namespace Application.BaoChi.XuatBanAnPham
{
    /// <summary>
    /// Danh sách xuất bản ấn phẩm
    /// </summary>
    public class DanhSach
    {
        public class Query : IRequest<Result<List<XuatBanAnPhamDTO>>>
        {
            public XuatBanAnPhamRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<XuatBanAnPhamDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<XuatBanAnPhamDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TheLoaiID", request.Data.TheLoaiID);
                        parameters.Add("@NhaXuatBanID", request.Data.NhaXuatBanID);
                        parameters.Add("@TrangThai", request.Data.TrangThai);

                        var queryResult = await connettion.QueryAsync<XuatBanAnPhamDTO>("spu_BC_XuatBanAnPham_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<XuatBanAnPhamDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<XuatBanAnPhamDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
