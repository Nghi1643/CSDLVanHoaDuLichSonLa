using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class DiSanVanHoaVatTheController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý di sản văn hoá vật thể";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View("DiSanVanHoaVatThe", vm);
        }
    }
}
