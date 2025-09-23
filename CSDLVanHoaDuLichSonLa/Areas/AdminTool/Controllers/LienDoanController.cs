using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class LienDoanController : AdminControllerBase
    {
        public async Task<IActionResult> LienDoan()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý liên đoàn thể thao";
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
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa liên đoàn" : "Thêm mới liên đoàn";
            
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.CoQuanId = id;
            ViewData["Title"] = "Chi tiết liên đoàn";
            
            return View();
        }
    }
}
