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
    public class DanhSachBanDich
    {
        public class Query : IRequest<Result<List<HienVat_NoiDung>>>
        {
            public Guid? HienVatID { get; set; }
            public string MaNgonNgu { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<HienVat_NoiDung>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<HienVat_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@HienVatID", request.HienVatID);
                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
                        var queryResult = await connettion.QueryAsync<HienVat_NoiDung>("spu_VH_HienVat_NoiDung_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<HienVat_NoiDung>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<HienVat_NoiDung>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
