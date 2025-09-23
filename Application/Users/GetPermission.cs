using Dapper;
using Domain.Core;
using Domain.ResponseDtos;
using MediatR;
using Microsoft.Extensions.Configuration;
using Persistence;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class GetPermission
    {        

        public class Query : IRequest<Result<PermissionDto>>
        {
            public string AreaName { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public string RolesName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PermissionDto>>
        {
            private readonly IConfiguration _configuration;
            public Handler(DataContext dataContext, IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<Result<PermissionDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@pAreaName", request.AreaName);
                        parameters.Add("@pControllerName", request.ControllerName);
                        parameters.Add("@pActionName", request.ActionName);
                        parameters.Add("@pRoles", request.RolesName);
                        var queryResult = await connettion.QueryAsync<PermissionDto>("spu_User_GetPermission", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        PermissionDto result = new PermissionDto
                        {
                            PermitedEdit = false,
                            PermitedView = false,
                            PermitedApprove = false,
                            PermitedDelete = false,
                            PermitedCreate = false
                        };

                        foreach (var item in queryResult)
                        {
                            if (item.PermitedEdit.HasValue || item.PermitedDelete.HasValue || item.PermitedApprove.HasValue)
                                result.PermitedView = true;

                            if (item.PermitedEdit.HasValue && item.PermitedEdit.Value)
                                result.PermitedEdit = true;

                            if (item.PermitedDelete.HasValue && item.PermitedDelete.Value)
                                result.PermitedDelete = true;

                            if (item.PermitedApprove.HasValue && item.PermitedApprove.Value)
                                result.PermitedApprove = true;
                            if (item.PermitedCreate.HasValue && item.PermitedCreate.Value)
                                result.PermitedCreate = true;
                        }

                        if (result.PermitedDelete.Value || result.PermitedApprove.Value || result.PermitedEdit.Value || result.PermitedCreate.Value)
                            result.PermitedView = true;

                        return Result<PermissionDto>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<PermissionDto>.Failure(ex.Message);
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
