using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class ThietLapNgonNguController : AdminControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Thiết lập ngôn ngữ";
            //Set permission tạm thời
            vm.PermitedCreate = 1;
            vm.PermitedEdit = 1;
            vm.PermitedView = 1;
            vm.PermitedDelete = 1;
            return View(vm);
        }
    }
}
