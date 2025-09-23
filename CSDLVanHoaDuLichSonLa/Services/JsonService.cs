using Azure.Core;
using CSDLVanHoaDuLichSonLa.Services;
using Dapper;
using Domain;
using Domain.ResponseDtos;
using Newtonsoft.Json;
using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.Data.SqlClient;
using static Dapper.SqlMapper;

namespace CSDLVanHoaDuLichSonLa.Services
{
    public class JsonService : IJsonService
    {
        private readonly IConfiguration _configuration;

        public JsonService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void GetData()
        {
            
        }

        public void RecreateData()
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    var result = connection.Query<MenuItemWithRoles>(new CommandDefinition("spu_DanhSachCayMenu", parameters: null, commandType: System.Data.CommandType.StoredProcedure)).ToList();

                    JsonSerializerSettings _options = new() { NullValueHandling = NullValueHandling.Ignore };
                    List<MenuItemWithRoles> subItems = new List<MenuItemWithRoles>();

                    List<AdminNavItemModel> navItems = new List<AdminNavItemModel>();

                    //foreach (var item in result)
                    //{
                    //    if (item.ParentId.HasValue)
                    //    {
                    //        subItems.Add(item);
                    //    }
                    //    else
                    //    {
                    //        navItems.Add(new AdminNavItemModel
                    //        {
                    //            Id = item.Id,
                    //            AreaName = item.AreaName,
                    //            ControllerName = item.ControllerName,
                    //            ActionName = item.ActionName,
                    //            Title = item.Title,
                    //            ListRoles = string.IsNullOrEmpty(item.ListRole) ? new List<string>() : item.ListRole.Split(",").ToList(),
                    //            ListChilds = new List<AdminNavSubItemModel>(),
                    //            IsLeaf = item.IsLeaf,
                    //            Icon = item.Icon,
                    //            IsAuthorize = false
                    //        });
                    //    }
                    //}

                    //foreach (var item in navItems)
                    //{
                    //    // menu cấp 1
                    //    foreach (var subItem in subItems)
                    //    {
                    //        if (subItem.ParentId.Value == item.Id)
                    //        {
                    //            item.ListChilds.Add(new AdminNavSubItemModel
                    //            {
                    //                Id = subItem.Id,

                    //                AreaName = subItem.AreaName,
                    //                ControllerName = subItem.ControllerName,
                    //                ActionName = subItem.ActionName,
                    //                Title = subItem.Title,
                    //                IsLeaf = true,
                    //                ListRoles = subItem.ListRole?.Split(",").ToList() ?? [],
                    //                IsAuthorize = false
                    //            });
                    //        }
                    //    }

                    //    // menu cấp 2
                    //    foreach (var subItemCap2 in item.ListChilds)
                    //    {
                    //        foreach (var subItem in subItems)
                    //        {
                    //            if (subItem.ParentId.Value == subItemCap2.Id)
                    //            {
                    //                item.ListChilds.Add(new AdminNavSubItemModel
                    //                {
                    //                    Id = subItem.Id,
                    //                    AreaName = subItem.AreaName,
                    //                    ControllerName = subItem.ControllerName,
                    //                    ActionName = subItem.ActionName,
                    //                    Title = subItem.Title,
                    //                    IsLeaf = true,
                    //                    ListRoles = subItem.ListRole?.Split(",").ToList() ?? [],
                    //                    IsAuthorize = false
                    //                });
                    //            }
                    //        }
                    //    }

                    //}




                    //object abc = new
                    //{
                    //    Items = navItems
                    //};

                    //object aa = new
                    //{
                    //    Navs = abc
                    //};

                    var allItems = result.Select(item => new AdminNavItemModel
                    {
                        Id = item.Id,
                        ParentId = item.ParentId,
                        AreaName = item.AreaName,
                        ControllerName = item.ControllerName,
                        ActionName = item.ActionName,
                        Title = item.Title,
                        Icon = item.Icon,
                        IsLeaf = item.IsLeaf,
                        IsShow = item.IsShow,
                        ListRoles = string.IsNullOrEmpty(item.ListRole) ? new List<string>() : item.ListRole.Split(",").ToList(),
                    }).ToList();

                    var navTree = BuildMenuTree(allItems);
                    
                    object abc = new
                    {
                        Items = navTree
                    };

                    object aa = new
                    {
                        Navs = abc
                    };
                    var jsonString = JsonConvert.SerializeObject(aa, _options);
                    File.WriteAllText("nav.json", jsonString);
                }
            }
            catch (Exception)
            {

            }
        }

        public void UpdateData(AdminNavModel vm, AdminNavItemModel itemModel, string RoleName)
        {
            foreach(var item in vm.Items)
            {
                if (item.Id == itemModel.Id)
                {
                    if (itemModel.IsLeaf)
                    {
                        item.ListRoles.Add(RoleName);
                    }
                    else
                    {
                        foreach (var itemSub in item.ListChilds)
                        {
                            if (itemSub.Id == itemModel.ListChilds[0].Id)
                            {
                                itemSub.ListRoles.Add(RoleName);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        private List<AdminNavItemModel> BuildMenuTree(List<AdminNavItemModel> allItems, long? parentId = null)
        {
            return allItems
                .Where(x => x.ParentId == parentId)
                .Select(x => new AdminNavItemModel
                {
                    Id = x.Id,
                    AreaName = x.AreaName,
                    ControllerName = x.ControllerName,
                    ActionName = x.ActionName,
                    Title = x.Title,
                    Icon = x.Icon,
                    IsLeaf = x.IsLeaf,
                    IsShow = x.IsShow,
                    ListRoles = x.ListRoles,
                    IsAuthorize = false,
                    ListChilds = BuildMenuTree(allItems, x.Id)
                })
                .ToList();
        }
    }
}
