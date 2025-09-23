using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class BaoChiController : AdminControllerBase
    {
        public async Task<IActionResult> CoQuanBaoChi()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Cơ quan báo chí";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.CoQuanId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa cơ quan báo chí" : "Thêm mới cơ quan báo chí";
            
            return View();
        }
        
        public IActionResult Edit(Guid? id = null)
        {
            ViewPermissionViewModel vma = new ViewPermissionViewModel
            {
                PermitedView = 1,
                PermitedEdit = 1,
                PermitedDelete = 1,
                PermitedApprove = 1,
                PermitedCreate = 1,
            };

            ViewBag.IsEdit = id.HasValue;
            ViewBag.CoQuanId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa cơ quan báo chí" : "Thêm mới cơ quan báo chí";
            
            return View(vma);
        }

        public IActionResult Details(Guid id)
        {
            ViewBag.CoQuanId = id;
            ViewData["Title"] = "Chi tiết cơ quan báo chí";

            return View();
        }

        public IActionResult AnPhamKenhPhatSong(string id="")
        {
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.ToChucID = id;
            }
            else
            {
                ViewBag.ToChucID = "a2bb7e5c-d698-4003-997d-629ba92df32d";
            }

            ViewPermissionViewModel vma = new ViewPermissionViewModel
                {
                    PermitedView = 1,
                    PermitedEdit = 1,
                    PermitedDelete = 1,
                    PermitedApprove = 1,
                    PermitedCreate = 1,
                };
            ViewData["Title"] = "Quản lý ẩn phẩm/kênh phát sóng";

            return View(vma);
        }
    }
}
