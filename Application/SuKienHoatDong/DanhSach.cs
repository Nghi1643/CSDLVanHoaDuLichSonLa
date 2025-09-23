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

namespace Application.SuKienHoatDong
{
    public class DanhSach
    {
        /// <summary>
        /// Danh sách danh mục chung(list địa điểm, list chức danh,...) theo loại danh mục (địa điểm, chức danh,...)
        /// </summary>
        public class Query : IRequest<Result<List<SuKienHoatDongDTO>>>
        {
            public SuKienHoatDongRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<SuKienHoatDongDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<SuKienHoatDongDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TrangThai", request.Data.TrangThai);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@DonViToChucID", request.Data.DonViToChucID);
                        parameters.Add("@SuKienID", request.Data.SuKienID);

                        var queryResult = await connettion.QueryAsync<SuKienHoatDongDTO>("spu_DM_SuKienHoatDong_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<SuKienHoatDongDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<SuKienHoatDongDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
