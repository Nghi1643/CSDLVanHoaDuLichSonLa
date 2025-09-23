using Dapper;
using Domain.Core;
using Domain.DanhMuc;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TacPham
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<TacPhamDTO>>
        {
            public Guid TacPhamID { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<TacPhamDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<TacPhamDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TacPhamID", request.TacPhamID);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<TacPhamDTO>("spu_DM_TacPham_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        if (queryResult != null)
                        {
                            DynamicParameters parametersNoiDung = new DynamicParameters();
                            parametersNoiDung.Add("@TacPhamID", request.TacPhamID);
                            var queryResultNoiDung = await connettion.QueryAsync<TacPham_NoiDung>("spu_DM_TacPham_NoiDung_Get", parametersNoiDung, commandType: System.Data.CommandType.StoredProcedure);
                            queryResult.BanDich = queryResultNoiDung?.ToList();
                        }
                        return Result<TacPhamDTO>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<TacPhamDTO>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
