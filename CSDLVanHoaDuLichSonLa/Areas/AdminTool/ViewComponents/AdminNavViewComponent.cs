using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CSDLVanHoaDuLichSonLa.Models;
using System.Security.Claims;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.ViewComponents
{
    public class AdminNavViewComponent : ViewComponent
    {
        //private readonly IOptions<AdminNavModel> _adminNav;

        //public AdminNavViewComponent(IOptions<AdminNavModel> adminNav)
        //{
        //    _adminNav = adminNav;
        //}

        private readonly IOptionsSnapshot<AdminNavModel> _adminNav;

        public AdminNavViewComponent(IOptionsSnapshot<AdminNavModel> adminNav)
        {
            _adminNav = adminNav;
        }

        public IViewComponentResult Invoke()
        {
            var user = (ClaimsIdentity)User.Identity;

            string strArrRoles = user.FindFirst("RolesList")?.Value ?? "";
            string[] arrRoles = strArrRoles.Split(',');

            // Nếu user có role "Host" hoặc "SystemAdmin" => tất cả authorize
            bool isHost = arrRoles.Contains("Host") || arrRoles.Contains("SystemAdmin") ? true : false;

            AdminNavModel vm = (AdminNavModel)_adminNav.Value.Clone();

            foreach (var navItem in vm.Items)
            {
                if (isHost)
                {
                    navItem.IsAuthorize = true;
                    foreach (var subNav in navItem.ListChilds)
                    {
                        if (subNav.IsShow)
                        {
                            subNav.IsAuthorize = true;
                        }
                        foreach (var subSubNav in subNav.ListChilds)
                        {
                            if (subSubNav.IsShow)
                            {
                                subSubNav.IsAuthorize = true;
                            }
                        }
                    }
                    continue;
                }

                // Duyệt bình thường cho user không phải Host
                if (navItem.IsLeaf)
                {
                    foreach (var roleItem in arrRoles)
                    {
                        if (navItem.ListRoles.Contains(roleItem))
                        {
                            if (navItem.IsShow)
                            {
                                navItem.IsAuthorize = true;
                                break;
                            }
                        }
                    }
                    continue;
                }

                foreach (var subNav in navItem.ListChilds)
                {
                    foreach (var roleItem in arrRoles)
                    {
                        if (subNav.ListRoles.Contains(roleItem))
                        {
                            if (subNav.IsShow)
                            {
                                subNav.IsAuthorize = true;
                                navItem.IsAuthorize = true;
                                break;
                            }
                        }
                    }

                    foreach (var subSubNav in subNav.ListChilds)
                    {
                        foreach (var roleItem in arrRoles)
                        {
                            if (subSubNav.ListRoles.Contains(roleItem))
                            {
                                if (subSubNav.IsShow)
                                {
                                    subSubNav.IsAuthorize = true;
                                    subNav.IsAuthorize = true;
                                    navItem.IsAuthorize = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return View(vm);
        }
    }
}
