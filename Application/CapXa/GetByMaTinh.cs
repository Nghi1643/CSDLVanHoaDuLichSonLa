using Dapper;
using Domain.Core;
using Domain.DanhMuc;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CapXa
{
    public class GetByMaTInh
    {
        public class Query : IRequest<Result<IEnumerable<CapXaTrinhDien>>>
        {
            public string MaNgonNgu;
            public Guid MaTinh;
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<CapXaTrinhDien>>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<IEnumerable<CapXaTrinhDien>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        parameters.Add("@MaTinh", request.MaTinh);
                        var result = await connection.QueryAsync<CapXaTrinhDien>("spu_DiaPhuongCapXa_GetByMaTinh", parameters, commandType: CommandType.StoredProcedure);
                        return Result<IEnumerable<CapXaTrinhDien>>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<IEnumerable<CapXaTrinhDien>>.Failure(ex.Message);
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
