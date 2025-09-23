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
    public class AddRelation
    {
        public class Command : IRequest<Result<DM_CaNhan_MonTheThao>>
        {
            public DM_CaNhan_MonTheThao Entity;
        }

        public class Handler : IRequestHandler<Command, Result<DM_CaNhan_MonTheThao>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<DM_CaNhan_MonTheThao>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.Entity.CaNhanID);
                        parameters.Add("@VaiTroID", request.Entity.VaiTroID);
                        parameters.Add("@MonTheThaoID", request.Entity.MonTheThaoID);
                        parameters.Add("@MonTheThaoChinh", request.Entity.MonTheThaoChinh);
                        var result = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_MonTheThao>("spu_DM_CaNhan_MonTheThao_Add", parameters, commandType: CommandType.StoredProcedure);
                        return Result<DM_CaNhan_MonTheThao>.Success(result);
                    }catch(Exception ex)
                    {
                        return Result<DM_CaNhan_MonTheThao>.Failure(ex.Message);
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
