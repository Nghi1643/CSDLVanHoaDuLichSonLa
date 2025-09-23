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

namespace Application.VanHoa.HienVat
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<HienVatDTO>>
        {
            public Guid HienVatID { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<HienVatDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<HienVatDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@HienVatID", request.HienVatID);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<HienVatDTO>("spu_VH_HienVat_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        if (queryResult != null)
                        {
                            DynamicParameters parametersNoiDung = new DynamicParameters();
                            parametersNoiDung.Add("@HienVatID", request.HienVatID);
                            var queryResultNoiDung = await connettion.QueryAsync<HienVat_NoiDung>("spu_VH_HienVat_NoiDung_Get", parametersNoiDung, commandType: System.Data.CommandType.StoredProcedure);
                            queryResult.BanDich = queryResultNoiDung?.ToList();
                        }
                        return Result<HienVatDTO>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<HienVatDTO>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
