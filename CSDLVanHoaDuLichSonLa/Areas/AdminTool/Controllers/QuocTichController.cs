using System.Security.Claims;
using CSDLVanHoaDuLichSonLa.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers

{
    [Authorize]
    public class QuocTichController : AdminControllerBase
    {
        public async Task<IActionResult> QuocTich()
        {            
            var vm = await getPermission();
            ViewData["Title"] = "Quốc tịch";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            return View(vm);
        }
    }
}
