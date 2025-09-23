using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class GiaiThuongController : AdminControllerBase
    {
        public async Task<IActionResult> GiaiThuong()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý giải thưởng";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.GiaiThuongId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa giải thưởng" : "Thêm mới giải thưởng";
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.GiaiThuongId = id;
            ViewData["Title"] = "Chi tiết giải thưởng";
            return View();
        }
    }
}
