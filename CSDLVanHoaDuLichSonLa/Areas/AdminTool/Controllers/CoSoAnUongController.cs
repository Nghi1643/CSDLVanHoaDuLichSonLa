using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    public class CoSoAnUongController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý cơ sở ăn uống";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }
        public IActionResult Add()
        {
            ViewBag.Title = "Thêm mới cơ sở ăn uống";

            return View();
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var vm = await getPermission();
            ViewBag.Title = "Chỉnh sửa cơ sở ăn uống";
            ViewBag.IsEdit = true;
            ViewBag.CoSoAnUongId = id;

            if (vm == null || vm.PermitedEdit == 0)
            {
                return View("Error");
            }

            return View(vm);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await getPermission();
            ViewBag.Title = "Chi tiết cơ sở ăn uống";
            ViewBag.CoSoAnUongId = id;

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }

            return View(vm);
        }
    }
}
