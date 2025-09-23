using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Domain.DM_CaNhan_BaoChiModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using Domain.DM_CaNhan_VanDongVienModel;

namespace Application.DM_CaNhan_CanBoTheThaoServices
{
    public class Filter
    {
        public class Query : IRequest<Result<IEnumerable<DM_CaNhan_CanBoTheThaoViewModel>>>
        {
            public string TuKhoa;
            public Guid? ToChucID;
            public int? GioiTinhID;
            public bool? TrangThai;
            public string MaNgonNgu;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<DM_CaNhan_CanBoTheThaoViewModel>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<DM_CaNhan_CanBoTheThaoViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
                        var result = await connection.QueryAsync<DM_CaNhan_CanBoTheThaoViewModel>("spu_DM_CaNhan_CanBoTheThao_Filter", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<DM_CaNhan_CanBoTheThaoViewModel>>.Success(result);
                    }catch (Exception ex)
                    {
                        return Result<IEnumerable<DM_CaNhan_CanBoTheThaoViewModel>>.Failure(ex.Message);
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
