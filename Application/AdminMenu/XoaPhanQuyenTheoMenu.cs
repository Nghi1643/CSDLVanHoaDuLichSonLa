using Dapper;
using Domain.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using Persistence;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminMenu
{
    public class XoaPhanQuyenTheoMenu
    {
        public class Command : IRequest<Result<int>>
        {
            public int MenuId { get; set; }
            public string RoleName { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly IConfiguration _configuration;
            public Handler(DataContext dataContext, IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@pMenuId", request.MenuId);
                        parameters.Add("@pRoleName", request.RoleName);
                        var updateResult = await connettion.ExecuteScalarAsync<int>("spu_MenuPermission_Delete", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        if (updateResult <= 0)
                        {
                            throw new Exception("Cập nhật không thành công");
                        }
                        return Result<int>.Success(updateResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<int>.Failure(ex.Message);
                    }
                    finally
                    {
                        await connettion.CloseAsync();
                    }
                }
            }
        }
    }
}
