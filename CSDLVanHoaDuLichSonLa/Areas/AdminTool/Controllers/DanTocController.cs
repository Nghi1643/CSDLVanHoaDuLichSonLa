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
    public class DanTocController : AdminControllerBase
    {
        public async Task<IActionResult> DanToc()
        {            
            var vm = await getPermission();
            ViewData["Title"] = "Dân tộc";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            return View(vm);
        }
    }
}
