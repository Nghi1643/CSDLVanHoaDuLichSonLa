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

namespace Application.VanHoa.DiTich
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<DiTichDTO>>
        {
            public Guid DiTichID { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<DiTichDTO>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<DiTichDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@DiTichID", request.DiTichID);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<DiTichDTO>("spu_VH_DiTich_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<DiTichDTO>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<DiTichDTO>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
