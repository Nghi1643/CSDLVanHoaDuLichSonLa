using Dapper;
using Domain.Core;
using Domain.ThongTinXucTien;
using Domain.TourDuLich;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Application.TourDuLich.TheLoai
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<TourDuLich_TheLoaiDTO>>
        {
            public Guid TheLoaiID { get; set; }
            public string MaNgonNgu { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<TourDuLich_TheLoaiDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config) => _config = config;

            public async Task<Result<TourDuLich_TheLoaiDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    var parameters = new DynamicParameters();
                    parameters.Add("@TheLoaiID", request.TheLoaiID);
                    parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                    var queryResult = await connection.QueryFirstOrDefaultAsync<TourDuLich_TheLoaiDTO>(
                        "spu_DL_TourDuLich_TheLoai_Get", 
                        parameters, 
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<TourDuLich_TheLoaiDTO>.Success(queryResult);
                }
                catch (Exception ex)
                {
                    return Result<TourDuLich_TheLoaiDTO>.Failure(ex.Message);
                }
            }
        }
    }
}
