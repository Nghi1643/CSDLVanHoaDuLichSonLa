using System.Security.Claims;
using CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers;
using CSDLVanHoaDuLichSonLa.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class CapXaController : AdminControllerBase
    {
        public async Task<IActionResult> CapXa()
        {            
            var vm = await getPermission();
            ViewData["Title"] = "Danh mục cấp xã";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            return View(vm);
        }
    }
}
