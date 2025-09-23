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
    public class CapTinhController : AdminControllerBase
    {
        public async Task<IActionResult> CapTinh()
        {            
            var vm = await getPermission();
            ViewData["Title"] = "Danh mục cấp tỉnh";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            return View(vm);
        }   
    }
}
