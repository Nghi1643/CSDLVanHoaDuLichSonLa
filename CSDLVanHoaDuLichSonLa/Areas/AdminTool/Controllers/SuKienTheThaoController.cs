using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class SuKienTheThaoController : AdminControllerBase
    {
        public async Task<IActionResult> SuKien()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý sự kiện văn hóa";
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
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa sự kiện văn hóa" : "Thêm mới sự kiện văn hóa";
            return View();
        }

        public IActionResult Edit(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.SuKienId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa sự kiện văn hóa" : "Thêm mới sự kiện văn hóa";
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.SuKienId = id;
            ViewData["Title"] = "Chi tiết sự kiện văn hóa";
            return View();
        }
    }
}
