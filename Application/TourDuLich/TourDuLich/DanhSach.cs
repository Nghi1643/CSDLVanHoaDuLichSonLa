using Dapper;
using Domain.Core;
using Domain.TourDuLich;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.TourDuLich.TourDuLich
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<TourDuLichDTO>>>
        {
            public TourDuLichRequest data { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<TourDuLichDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<TourDuLichDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync(cancellationToken);
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@TuKhoa", request.data.TuKhoa);
                    parameters.Add("@ToChucID", request.data.ToChucID);
                    parameters.Add("@TheLoaiID", request.data.TheLoaiID);
                    parameters.Add("@LoaiHinhID", request.data.LoaiHinhID);
                    parameters.Add("@PhanKhucID", request.data.PhanKhucID);
                    parameters.Add("@PhuongTienID", request.data.PhuongTienID);
                    parameters.Add("@MaNgonNgu", request.data.MaNgonNgu);
                    parameters.Add("@TrangThai", request.data.TrangThai);
                    var queryResult = await connection.QueryAsync<TourDuLichDTO>(
                        "spu_DL_TourDuLich_GetFilter",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<List<TourDuLichDTO>>.Success(queryResult?.ToList());
                }
                catch (Exception ex)
                {
                    return Result<List<TourDuLichDTO>>.Failure(ex.Message);
                }
            }
        }
    }
    
}
