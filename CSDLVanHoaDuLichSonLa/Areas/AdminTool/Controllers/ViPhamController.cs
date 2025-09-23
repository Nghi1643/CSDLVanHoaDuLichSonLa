using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class ViPhamController : AdminControllerBase
    {
        public async Task<IActionResult> ViPham()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý vi phạm và xử lý vi phạm";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.ViPhamId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa vi phạm" : "Thêm mới vi phạm";
            return View();
        }

        public IActionResult Edit(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.ViPhamId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa vi phạm" : "Thêm mới vi phạm";
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.ViPhamId = id;
            ViewData["Title"] = "Chi tiết vi phạm";
            return View();
        }
    }
}
