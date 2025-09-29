using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class AmThucController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View("AmThuc", vm);
        }

        public async Task<IActionResult> ThemMoi()
        {
            return View("AddEdit");
        }
        public async Task<IActionResult> ChinhSua(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("Error");
            }
            ViewData["monAnUongID"] = id;
            return View("AddEdit");
        }
    }
}
