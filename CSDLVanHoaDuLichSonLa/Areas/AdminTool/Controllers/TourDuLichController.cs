using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class TourDuLichController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý tour du lịch";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }
        public IActionResult Add()
        {
            ViewBag.Title = "Thêm mới tour du lịch";

            return View();
        }

        public IActionResult Edit(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.TourID = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa tour du lịch" : "Thêm mới tour du lịch";
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.TourID = id;
            ViewData["Title"] = "Chi tiết tour du lịch";
            return View();
        }
    }
}
