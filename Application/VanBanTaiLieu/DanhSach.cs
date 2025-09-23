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
using Domain.ToChuc;
using Domain.SuKienHoatDong;
using Domain.CaNhan;
using Domain.DanhMuc;

namespace Application.VanBanTaiLieu
{
    public class DanhSach
    {
        /// <summary>
        /// Danh sách danh mục chung(list địa điểm, list chức danh,...) theo loại danh mục (địa điểm, chức danh,...)
        /// </summary>
        public class Query : IRequest<Result<List<VanBanTaiLieuDTO>>>
        {
            public VanBanTaiLieuRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<VanBanTaiLieuDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<VanBanTaiLieuDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@DoiTuongSoHuuID", request.Data.DoiTuongSoHuuID);
                        var queryResult = await connettion.QueryAsync<VanBanTaiLieuDTO>("spu_DM_VanBanTaiLieu_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<VanBanTaiLieuDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<VanBanTaiLieuDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
