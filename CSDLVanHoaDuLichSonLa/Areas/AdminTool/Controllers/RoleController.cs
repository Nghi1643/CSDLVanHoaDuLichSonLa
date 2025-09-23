using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSDLVanHoaDuLichSonLa.Models;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    public class RoleController : AdminControllerBase
    {
        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            ViewPermissionViewModel permission = await getPermission();
            if (permission.PermitedView == 0)
                return View("Error");
            ViewBag.PermitedCreate = permission.PermitedCreate;
            ViewBag.PermitedEdit = permission.PermitedEdit;
            ViewBag.PermitedDelete = permission.PermitedDelete;
            ViewBag.PageTitle = "Quản lý vai trò";
            return View("DanhSachVaiTro");
        }

        //[Authorize]
        //public IActionResult DanhSachVaiTro()
        //{
        //    //ViewPermissionViewModel permission = await getPermission();
        //    //if (permission.PermitedView == 0)
        //    //    return View("Error");
        //    //ViewBag.PermitedEdit = permission.PermitedEdit;
        //    //ViewBag.PermitedDelete = permission.PermitedDelete;
        //    //ViewBag.PermitedApprove = permission.PermitedApprove;
        //    ViewBag.PageTitle = "Phân quyền vai trò";
        //    return View("DanhSachVaiTro");
        //}
    }
}
