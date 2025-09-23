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

namespace Application.NgonNgu
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<Domain.DanhMuc.NgonNgu>>
        {
            public Guid NgonNguID { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Domain.DanhMuc.NgonNgu>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<Domain.DanhMuc.NgonNgu>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@NgonNguID", request.NgonNguID);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<Domain.DanhMuc.NgonNgu>("spu_DM_NgonNgu_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<Domain.DanhMuc.NgonNgu>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<Domain.DanhMuc.NgonNgu>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
