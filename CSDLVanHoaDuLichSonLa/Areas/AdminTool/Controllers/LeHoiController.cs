using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class LeHoiController : AdminControllerBase
    {
        public async Task<IActionResult> LeHoi()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý lễ hội";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.LeHoiId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa lễ hội" : "Thêm mới lễ hội";
            return View();
        }

        public IActionResult Edit(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.LeHoiId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa lễ hội" : "Thêm mới lễ hội";
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.LeHoiId = id;
            ViewData["Title"] = "Chi tiết lễ hội";
            return View();
        }
    }
}
