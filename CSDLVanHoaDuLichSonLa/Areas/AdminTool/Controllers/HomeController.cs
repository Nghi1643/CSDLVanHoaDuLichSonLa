using CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers;
using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    public class HomeController : AdminControllerBase
    {
        [Authorize]
        public IActionResult Index()
        {
            ViewBag.PageTitle = "Trang chủ";
            var claimUser = (ClaimsIdentity)User.Identity;
            ViewBag.orgUniqueCode = "";
            return View();
        }
        public IActionResult TableDemo()
        {
            return View();
        }

        public async Task<IActionResult> HuongDanHTMLAsync()
        {
            ViewPermissionViewModel permission = await getPermission();
            if (permission.PermitedView == 0)
                return View("Error");
            return View();
        }
    }
}
