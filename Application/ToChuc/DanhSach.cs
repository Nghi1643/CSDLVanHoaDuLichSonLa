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

namespace Application.ToChuc
{
    public class DanhSach
    {
        /// <summary>
        /// Danh sách danh mục chung(list địa điểm, list chức danh,...) theo loại danh mục (địa điểm, chức danh,...)
        /// </summary>
        public class Query : IRequest<Result<List<ToChucDTO>>>
        {
            public ToChucRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ToChucDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<ToChucDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@LoaiToChucID", request.Data.LoaiToChucID);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                        parameters.Add("@CoQuanChuQuanID", request.Data.CoQuanChuQuanID);
                        parameters.Add("@PhamViHoatDongID", request.Data.PhamViHoatDongID);
                        parameters.Add("@LoaiHinhID", request.Data.LoaiHinhID);
                        parameters.Add("@ToChucID", request.Data.ToChucID);
                        var queryResult = await connettion.QueryAsync<ToChucDTO>("spu_DM_ToChuc_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<ToChucDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<ToChucDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
