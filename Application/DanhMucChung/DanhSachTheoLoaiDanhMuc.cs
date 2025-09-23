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
using Domain.DanhMuc;
using Microsoft.Data.SqlClient;

namespace Application.DanhMucChung
{
    public class DanhSachTheoLoaiDanhMuc
    {
        /// <summary>
        /// Danh sách danh mục chung(list địa điểm, list chức danh,...) theo loại danh mục (địa điểm, chức danh,...)
        /// </summary>
        public class Query : IRequest<Result<List<DanhMucChungDTO>>>
        {
            public DanhMucChungRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<DanhMucChungDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<DanhMucChungDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@LoaiDanhMucID", request.Data.LoaiDanhMucID);
                        parameters.Add("@TrangThai", request.Data.TrangThai);
                        var queryResult = await connettion.QueryAsync<DanhMucChungDTO>("spu_DM_DanhMucChung_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<DanhMucChungDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<DanhMucChungDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
