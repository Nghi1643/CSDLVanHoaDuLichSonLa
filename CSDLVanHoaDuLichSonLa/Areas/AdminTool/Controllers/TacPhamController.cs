using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class TacPhamNgheThuatController : AdminControllerBase
    {
        // view index của tác phẩm nghệ thuật thuộc văn hoá
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View("TacPhamNgheThuat", vm);
        }
     
        public async Task<IActionResult> ThemMoi()
        {
            var vm = await getPermission();

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View("AddEdit");
        }
        public async Task<IActionResult> ChinhSua(string id)
        {
            var vm = await getPermission();

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }

            if (string.IsNullOrEmpty(id))
            {
                return View("Error");
            }

            ViewData["TacPhamID"] = id;
            return View("AddEdit");
        }
    }
}
