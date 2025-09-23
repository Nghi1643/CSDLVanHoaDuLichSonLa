using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class SuKienController : AdminControllerBase
    {
        public async Task<IActionResult> SuKien()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý sự kiện";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.SuKienId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa sự kiện" : "Thêm mới sự kiện";
            return View();
        }

        public IActionResult Edit(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.SuKienId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa sự kiện" : "Thêm mới sự kiện";
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.SuKienId = id;
            ViewData["Title"] = "Chi tiết sự kiện";
            return View();
        }
    }
}
