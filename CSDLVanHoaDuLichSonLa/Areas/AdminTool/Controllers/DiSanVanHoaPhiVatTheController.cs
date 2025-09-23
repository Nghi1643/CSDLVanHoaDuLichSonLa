using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class DiSanVanHoaPhiVatTheController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View("DiSanVanHoaPhiVatThe", vm);
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
            ViewData["diSanID"] = id;
            return View("AddEdit");
        }
    }
}
