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

namespace Application.VanHoa.PhiVatThe
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<DiSanPhiVatTheDTO>>
        {
            public Guid DiSanID { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<DiSanPhiVatTheDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<DiSanPhiVatTheDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@DiSanID", request.DiSanID);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<DiSanPhiVatTheDTO>("spu_VH_DiSanPhiVatThe_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        if (queryResult != null)
                        {
                            DynamicParameters parametersNoiDung = new DynamicParameters();
                            parametersNoiDung.Add("@DiSanID", request.DiSanID);
                            var queryResultNoiDung = await connettion.QueryAsync<DiSanPhiVatThe_NoiDung>("spu_VH_DiSanPhiVatThe_NoiDung_Get", parametersNoiDung, commandType: System.Data.CommandType.StoredProcedure);
                            queryResult.BanDich = queryResultNoiDung?.ToList();
                        }
                        return Result<DiSanPhiVatTheDTO>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<DiSanPhiVatTheDTO>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
