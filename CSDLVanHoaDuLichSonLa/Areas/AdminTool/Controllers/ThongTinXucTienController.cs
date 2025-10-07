using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    public class ThongTinXucTienController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();
            ViewBag.Title = "Quản lý thông tin xúc tiến quảng bá du lịch";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }
        public IActionResult Add()
        {
            ViewBag.Title = "Thêm mới thông tin xúc tiến quảng bá du lịch";

            return View();
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var vm = await getPermission();
            ViewBag.Title = "Chỉnh sửa thông tin xúc tiến quảng bá du lịch";
            ViewBag.IsEdit = true;
            ViewBag.XucTienID = id;

            if (vm == null || vm.PermitedEdit == 0)
            {
                return View("Error");
            }

            return View(vm);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await getPermission();
            ViewBag.Title = "Chi tiết thông tin xúc tiến quảng bá du lịch";
            ViewBag.XucTienID = id;

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }

            return View(vm);
        }
    }
}
