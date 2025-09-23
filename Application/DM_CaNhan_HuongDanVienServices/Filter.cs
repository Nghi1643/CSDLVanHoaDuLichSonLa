using Dapper;
using Domain.Core;
using Domain.DM_CaNhan_HuongDanVienModel;
using Domain.DM_CaNhan_VanDongVienModel;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DM_CaNhan_HuongDanVienServices
{
    public class Filter
    {
        public class Query : IRequest<Result<IEnumerable<DM_CaNhan_HuongDanVienViewModel>>>
        {
            public string TuKhoa;
            public Guid? ToChucID;
            public int? GioiTinhID;
            public bool? TrangThai;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<DM_CaNhan_HuongDanVienViewModel>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<DM_CaNhan_HuongDanVienViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.TuKhoa);
                        parameters.Add("@ToChucID", request.ToChucID);
                        parameters.Add("@GioiTinhID", request.GioiTinhID);
                        parameters.Add("@TrangThai", request.TrangThai);
                        parameters.Add("@MaNgoNgu", request.MaNgonNgu);
                        var result = await connection.QueryAsync<DM_CaNhan_HuongDanVienViewModel>("spu_DM_CaNhan_HuongDanVien_Filter", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<DM_CaNhan_HuongDanVienViewModel>>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<IEnumerable<DM_CaNhan_HuongDanVienViewModel>>.Failure(ex.Message);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
        }
    }
}
