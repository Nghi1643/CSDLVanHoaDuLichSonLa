using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("AdminTool/DiSanVanHoaVatThe/[controller]/[action]")]
    public class HienVatController : AdminControllerBase
    {
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
            ViewData["hienVatID"] = id;
            return View("AddEdit");
        }
    }
}
