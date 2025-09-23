using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class MonTheThaoController : AdminControllerBase
    {
        public async Task<IActionResult> MonTheThao()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý môn thể thao";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }
    }
}
