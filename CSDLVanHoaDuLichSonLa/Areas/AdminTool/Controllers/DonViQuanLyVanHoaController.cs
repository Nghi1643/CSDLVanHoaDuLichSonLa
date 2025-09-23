using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class DonViQuanLyVanHoaController : AdminControllerBase
    {
        public async Task<IActionResult> DonViQuanLyVanHoa()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý đơn vị quản lý văn hóa";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.CoQuanId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa đơn vị quản lý văn hóa" : "Thêm mới đơn vị quản lý văn hóa";
            
            return View();
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.CoQuanId = id;
            ViewData["Title"] = "Chi tiết đơn vị quản lý văn hóa";
            
            return View();
        }
    }
}
