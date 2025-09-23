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

namespace Application.BaoChi.ViPham
{
    /// <summary>
    /// Danh sách xuất bản ấn phẩm
    /// </summary>
    public class DanhSach
    {
        public class Query : IRequest<Result<List<ViPhamDTO>>>
        {
            public ViPhamRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ViPhamDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<ViPhamDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@DonViViPhamID", request.Data.DonViViPhamID);
                        parameters.Add("@TrangThaiXuly", request.Data.TrangThaiXuly);
                        parameters.Add("@MaViPham", request.Data.MaViPham);

                        var queryResult = await connettion.QueryAsync<ViPhamDTO>("spu_BC_ViPham_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<ViPhamDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<ViPhamDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
