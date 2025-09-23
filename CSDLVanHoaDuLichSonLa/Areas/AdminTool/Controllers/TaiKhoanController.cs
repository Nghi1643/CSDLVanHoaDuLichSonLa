using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSDLVanHoaDuLichSonLa.Models;
using System.Security.Claims;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    public class TaiKhoanController : AdminControllerBase
    {
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ViewPermissionViewModel permission = await getPermission();
            if (permission.PermitedView == 0)
                return View("Error");

            var claimUser = (ClaimsIdentity)User.Identity;
            if(claimUser != null)
            {
                string rolename = claimUser.FindFirst("RolesList").Value;
                string orgCode = claimUser.FindFirst("OrgUniqueCode").Value;

                var arr = rolename.Split(',');
                for(var i = 0; i < arr.Length; i++) {
                    if (arr[i] != null && (arr[i].ToUpper() == "HOST" || arr[i].ToUpper() == "SYSTEMADMIN"))
                    {
                        ViewBag.isAdmin = 1;
                        break;
                    }
                    else
                    {
                        ViewBag.isAdmin = 0;
                    }
                }
               
                ViewBag.orgUniqueCode = orgCode;
            }
            ViewBag.PageTitle = "Quản lý người dùng";
            return View(permission);
        }
    }
}
