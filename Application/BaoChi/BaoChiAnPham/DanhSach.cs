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

namespace Application.BaoChi.BaoChiAnPham
{
    /// <summary>
    /// Danh sách báo chí ấn phẩm 
    /// </summary>
    public class DanhSach
    {
        public class Query : IRequest<Result<List<BaoChiAnPhamDTO>>>
        {
            public BaoChiAnPhamRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<BaoChiAnPhamDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<BaoChiAnPhamDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@ToChucID", request.Data.ToChucID);
                        parameters.Add("@TanSuatID", request.Data.TanSuatID);
                        parameters.Add("@LinhVucChuyenSauID", request.Data.LinhVucChuyenSauID);
                        parameters.Add("@TrangThai", request.Data.TrangThai);

                        var queryResult = await connettion.QueryAsync<BaoChiAnPhamDTO>("spu_BC_BaoChiAnPham_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<BaoChiAnPhamDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<BaoChiAnPhamDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
