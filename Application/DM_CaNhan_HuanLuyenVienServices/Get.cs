using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.DM_CaNhan_HuanLuyenVienServices
{
    public class Get
    {
        public class Query : IRequest<Result<DM_CaNhan_HuanLuyenVienViewModel>>
        {
            public string MaNgonNgu;
            public Guid CaNhanID;
        }

        public class Handler : IRequestHandler<Query, Result<DM_CaNhan_HuanLuyenVienViewModel>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_CaNhan_HuanLuyenVienViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        var result = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_HuanLuyenVienViewModel>("spu_DM_CaNhan_HuanLuyenVien_Get", parameters, commandType: CommandType.StoredProcedure);
                        return Result<DM_CaNhan_HuanLuyenVienViewModel>.Success(result);
                    }catch (Exception ex )
                    {
                        return Result<DM_CaNhan_HuanLuyenVienViewModel>.Failure(ex.Message);
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
