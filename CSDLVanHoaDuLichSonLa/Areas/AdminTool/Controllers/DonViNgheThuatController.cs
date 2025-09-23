using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class DonViNgheThuatController : AdminControllerBase
    {
        public async Task<IActionResult> DonViNgheThuat()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý đơn vị nghệ thuật";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.CoQuanId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa đơn vị nghệ thuật" : "Thêm mới đơn vị nghệ thuật";
            
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.CoQuanId = id;
            ViewData["Title"] = "Chi tiết đơn vị nghệ thuật";
            
            return View();
        }
    }
}
