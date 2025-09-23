using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSDLVanHoaDuLichSonLa.Models;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    public class MenuController : AdminControllerBase
    {
        [Authorize]
        public IActionResult Index()
        {
            //ViewPermissionViewModel permission = await getPermission();
            //if (permission.PermitedView == 0)
            //    return View("Error");

            //ViewBag.PermitedEdit = permission.PermitedEdit;
            //ViewBag.PermitedDelete = permission.PermitedDelete;
            //ViewBag.PermitedApprove = permission.PermitedApprove;

            ViewBag.PermitedEdit = 1;
            ViewBag.PermitedDelete = 1;
            ViewBag.PermitedApprove = 1;

            return View();
        }

        [Authorize]
        public IActionResult DanhSach()
        {
            ViewBag.PageTitle = "Quản lý menu";
            return View();
        }
    }
}
