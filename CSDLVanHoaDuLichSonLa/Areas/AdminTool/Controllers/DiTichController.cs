using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("AdminTool/DiSanVanHoaVatThe/[controller]/[action]")]
    public class DiTichController : AdminControllerBase
    {
        public async Task<IActionResult> ThemMoi()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Thêm mới di tích lịch sử văn hoá";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View("AddEdit", vm);
        }

        public async Task<IActionResult> ChinhSua(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("Error");
            }

            ViewData["diTichID"] = id;

            var vm = await getPermission();
            ViewData["Title"] = "Chỉnh sửa di tích lịch sử văn hoá";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View("AddEdit", vm);
        }
    }
}
