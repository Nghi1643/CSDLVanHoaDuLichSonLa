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

namespace Application.DM_CaNhan_VanDongVienServices
{
    public class Get
    {
        public class Query : IRequest<Result<DM_CaNhan_VanDongVienViewModel>>
        {
            public string MaNgonNgu;
            public Guid CaNhanID;
        }

        public class Handler : IRequestHandler<Query, Result<DM_CaNhan_VanDongVienViewModel>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_CaNhan_VanDongVienViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var result = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_VanDongVienViewModel>("spu_DM_CaNhan_VanDongVien_Get", parameters, commandType: CommandType.StoredProcedure);
                        return Result<DM_CaNhan_VanDongVienViewModel>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<DM_CaNhan_VanDongVienViewModel>.Failure(ex.Message);
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
