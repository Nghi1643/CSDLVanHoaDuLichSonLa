using Dapper;
using Domain.Core;
using Domain.DaoTaoBoiDuong;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DaoTaoBoiDuong
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<DaoTaoBoiDuongDTO>>>
        {
            public DaoTaoBoiDuongRequest Data { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<DaoTaoBoiDuongDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<DaoTaoBoiDuongDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    var parameters = new DynamicParameters();
                    parameters.Add("@TuKhoa", request.Data.TuKhoa);
                    parameters.Add("@LinhVucID", request.Data.LinhVucID);
                    parameters.Add("@ToChucID", request.Data.ToChucID);
                    parameters.Add("@DiaDiemID", request.Data.DiaDiemID);
                    parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                    parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                    var queryResult = await connection.QueryAsync<DaoTaoBoiDuongDTO>(
                        "spu_DM_DaoTaoBoiDuong_GetFilter",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure);
                    return Result<List<DaoTaoBoiDuongDTO>>.Success(queryResult?.ToList());
                }
                catch (Exception ex)
                {
                    return Result<List<DaoTaoBoiDuongDTO>>.Failure(ex.Message);
                }
            }
        }
    }
}
