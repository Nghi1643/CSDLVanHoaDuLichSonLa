using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Application.DM_CaNhan_VanDongVienServices
{
    public class DeleteRelation
    {
        public class Command : IRequest<Result<int>>
        {
            public Guid CaNhanID;
            public Guid MonTheThaoID;
        }

        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.CaNhanID);
                        parameters.Add("@MonTheThaoID", request.MonTheThaoID);
                        var result = await connection.ExecuteAsync("spu_DM_CaNhan_MonTheThao_Delete", parameters, commandType: CommandType.StoredProcedure);
                        return Result<int>.Success(result);
                    }
                    catch (Exception ex) {
                        return Result<int>.Failure(ex.Message);
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
