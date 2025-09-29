using Dapper;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using Domain.VanHoa;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.VanHoa.VHAmThuc
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<AmThucDTO>>
        {
            public Guid MonAnUongID { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AmThucDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<AmThucDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MonAnUongID", request.MonAnUongID);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<AmThucDTO>("spu_VH_AmThuc_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        if (queryResult != null)
                        {
                            DynamicParameters parametersNoiDung = new DynamicParameters();
                            parametersNoiDung.Add("@MonAnUongID", request.MonAnUongID);
                            var queryResultNoiDung = await connettion.QueryAsync<AmThuc_NoiDung>("spu_VH_AmThuc_NoiDung_Get", parametersNoiDung, commandType: System.Data.CommandType.StoredProcedure);
                            queryResult.BanDich = queryResultNoiDung?.ToList();
                        }
                        return Result<AmThucDTO>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<AmThucDTO>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
