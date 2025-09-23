using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class XuatBanController : AdminControllerBase
    {
        public async Task<IActionResult> AnPhamXuatBan(string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.NhaXuatBanID = id;
            }
            var vm = await getPermission();
            ViewData["Title"] = "Ấn phẩm xuất bản";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.CoQuanId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa cơ quan báo chí" : "Thêm mới cơ quan báo chí";
            
            return View();
        }
        
        public IActionResult Edit(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.CoQuanId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa cơ quan báo chí" : "Thêm mới cơ quan báo chí";
            
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.CoQuanId = id;
            ViewData["Title"] = "Chi tiết cơ quan báo chí";

            return View();
        }
        
        public async Task<IActionResult> NhaXuatBan()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Nhà xuất bản";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public async Task<IActionResult> NhanSu()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Nhân sự";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }
    }
}
