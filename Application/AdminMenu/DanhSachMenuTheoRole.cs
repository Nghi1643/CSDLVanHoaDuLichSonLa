using Dapper;
using Domain;
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

namespace Application.AdminMenu
{
    public class DanhSachMenuTheoRole
    {
        public class Query : IRequest<Result<List<MenuItemCompact>>>
        {
            public string RoleName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<MenuItemCompact>>>
        {
            private readonly IConfiguration _configuration;
            public Handler(DataContext dataContext, IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<List<MenuItemCompact>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@pRole", request.RoleName);
                        var queryResult = await connettion.QueryAsync<MenuItemCompact>("spu_Menu_Query_ByRole", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        var result = queryResult.ToList();

                        List<MenuItemCompact> subLst = new List<MenuItemCompact>();

                        foreach (MenuItemCompact item in result)
                        {
                            if (item.PermissionId.HasValue)
                            {
                                subLst.Add(new MenuItemCompact
                                {
                                    Id = item.PermissionId.Value + 4000,
                                    ParentId = item.Id,
                                    Title = "Cho phép thêm mới",
                                    IsLeaf = true,
                                    HasPermission = item.PermitedCreate.Value,
                                    PermissionId = item.PermissionId.Value,
                                    PermissionType = 4,
                                    IsPermission = true
                                });
                                subLst.Add(new MenuItemCompact
                                {
                                    Id = item.PermissionId.Value + 1000,
                                    ParentId = item.Id,
                                    Title = "Cho phép chỉnh sửa",
                                    IsLeaf = true,
                                    HasPermission = item.PermitedEdit.Value,
                                    PermissionId = item.PermissionId.Value,
                                    PermissionType = 1,
                                    IsPermission = true
                                });
                                subLst.Add(new MenuItemCompact
                                {
                                    Id = item.PermissionId.Value + 2000,
                                    ParentId = item.Id,
                                    Title = "Cho phép xóa",
                                    IsLeaf = true,
                                    HasPermission = item.PermitedDelete.Value,
                                    PermissionId = item.PermissionId.Value,
                                    PermissionType = 2,
                                    IsPermission = true
                                });
                                subLst.Add(new MenuItemCompact
                                {
                                    Id = item.PermissionId.Value + 3000,
                                    ParentId = item.Id,
                                    Title = "Cho phép duyệt",
                                    IsLeaf = true,
                                    HasPermission = item.PermitedApprove.Value,
                                    PermissionId = item.PermissionId.Value,
                                    PermissionType = 3,
                                    IsPermission = true
                                });
                                //item.IsLeaf = false;
                            }
                        }
                        result.AddRange(subLst);

                        return Result<List<MenuItemCompact>>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<List<MenuItemCompact>>.Failure(ex.Message);
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
