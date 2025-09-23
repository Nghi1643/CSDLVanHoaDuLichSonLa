using CSDLVanHoaDuLichSonLa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    [Authorize]
    public class NhanSuController : AdminControllerBase
    {
        //Vận động viên
        public async Task<IActionResult> VanDongVien()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý vận động viên";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add_VanDongVien(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa vận động viên" : "Thêm mới vận động viên";
            return View();
        }

        public IActionResult Edit_VanDongVien(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa vận động viên" : "Thêm mới vận động viên";
            return View();
        }

        public IActionResult Details_VanDongVien(Guid id)
        {
            ViewBag.NhanSuId = id;
            ViewData["Title"] = "Chi tiết vận động viên";
            return View();
        }

        //Huấn luyện viên
        public async Task<IActionResult> HuanLuyenVien()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý huấn luyện viên";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add_HuanLuyenVien(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa huấn luyện viên" : "Thêm mới huấn luyện viên";
            return View();
        }

        public IActionResult Edit_HuanLuyenVien(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa huấn luyện viên" : "Thêm mới huấn luyện viên";
            return View();
        }

        public IActionResult Details_HuanLuyenVien(Guid id)
        {
            ViewBag.NhanSuId = id;
            ViewData["Title"] = "Chi tiết huấn luyện viên";
            return View();
        }

        // Trọng tài/giám sát
        public async Task<IActionResult> TrongTai()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý trọng tài/giám sát";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add_TrongTai(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa trọng tài/giám sát" : "Thêm mới trọng tài/giám sát";
            return View();
        }

        public IActionResult Edit_TrongTai(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa trọng tài/giám sát" : "Thêm mới trọng tài/giám sát";
            return View();
        }

        public IActionResult Details_TrongTai(Guid id)
        {
            ViewBag.NhanSuId = id;
            ViewData["Title"] = "Chi tiết trọng tài/giám sát";
            return View();
        }

        // cán bộ quản lý
        public async Task<IActionResult> CanBoTheThao()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quản lý cán bộ quản lý";
            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            return View(vm);
        }

        public IActionResult Add_CanBoTheThao(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa cán bộ quản lý" : "Thêm mới cán bộ quản lý";
            return View();
        }

        public IActionResult Edit_CanBoTheThao(Guid? id = null)
        {
            ViewBag.IsEdit = id.HasValue;
            ViewBag.NhanSuId = id;
            ViewData["Title"] = id.HasValue ? "Chỉnh sửa cán bộ quản lý" : "Thêm mới cán bộ quản lý";
            return View();
        }

        public IActionResult Details_CanBoTheThao(Guid id)
        {
            ViewBag.NhanSuId = id;
            ViewData["Title"] = "Chi tiết cán bộ quản lý";
            return View();
        }
    }
}
