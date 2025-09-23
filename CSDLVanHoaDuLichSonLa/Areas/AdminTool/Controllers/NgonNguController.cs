using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class NgonNguController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Ngôn ngữ";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            return View(vm);
        }
    }
}
