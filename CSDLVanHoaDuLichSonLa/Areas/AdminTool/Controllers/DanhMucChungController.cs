using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Authorize]
    public class DanhMucChungController : AdminControllerBase
    {
        public async Task<IActionResult> CapDo()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Cấp độ sự kiện/hoạt động";

            if (vm == null || vm.PermitedView == 0) { 
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 4;
            return View(vm);
        }

        public async Task<IActionResult> TanSuatHoatDong()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Tần xuất hoạt động";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 6;
            return View(vm);
        }

        public async Task<IActionResult> ChucDanh()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Chức danh nghề nghiệp";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 2;
            return View(vm);
        }

        public async Task<IActionResult> DiaDiem()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Địa điểm";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 8;
            return View(vm);
        }

        public async Task<IActionResult> LinhVucHoatDong()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Lĩnh vực hoạt động";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 9;
            return View(vm);
        }
        public async Task<IActionResult> NgoaiNgu()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Ngoại ngữ";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 10;
            return View(vm);
        }

        public async Task<IActionResult> DanToc()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Dân tộc";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 1;
            return View(vm);
        }

        public async Task<IActionResult> QuocTich()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Quốc tịch";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 11;
            return View(vm);
        }

        public async Task<IActionResult> LoaiNhanSu()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại nhân sự";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 12;
            return View(vm);
        }

        public async Task<IActionResult> LoaiToChuc()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại tổ chức";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 13;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhMonTheThao()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình môn thể thao";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 14;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhCongTrinhTheThao()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình công trình thể thao";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 15;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhDiSanVanHoa()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình di sản văn hoá";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 16;
            return View(vm);
        }

        public async Task<IActionResult> CapXepHang()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Cấp xếp hạng";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 17;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhDiTich()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình di tích";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 18;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhSuKien()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình sự kiện";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 19;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhNgheThuatBieuDien()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình nghệ thuật biểu diễn";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 20;
            return View(vm);
        }

        public async Task<IActionResult> ChuyenMon()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Chuyên môn";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 21;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhBaoChiXuatBan()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình báo chí xuất bản";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 22;
            return View(vm);
        }

        public async Task<IActionResult> CoQuanChuQuan()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Cơ quan chủ quản";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 23;
            return View(vm);
        }
        public async Task<IActionResult> LinhVucXuatBan()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Lĩnh vực xuất bản";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 24;
            return View(vm);
        }

        public async Task<IActionResult> TheLoaiAnPham()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Lĩnh vực xuất bản";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 25;
            return View(vm);
        }
        public async Task<IActionResult> LoaiDiaDiemDuLich()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại địa điểm du lịch";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 26;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhDichVu()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình dịch vụ";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 27;
            return View(vm);
        }

        public async Task<IActionResult> LoaiLuHanh()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại lữ hành";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 28;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhLuuTru()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình lưu trú";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 29;
            return View(vm);
        }

        public async Task<IActionResult> LoaiTourDuLich()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại tour du lịch";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 30;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhDoanhNghiep()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình doanh nghiệp";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 31;
            return View(vm);
        }

        public async Task<IActionResult> LoaiHinhLaoDong()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại hình lao động";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 32;
            return View(vm);
        }

        public async Task<IActionResult> LoaiChuQuan()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại chủ quản";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 34;
            return View(vm);
        }

        public async Task<IActionResult> LoaiChatLieu()
        {
            var vm = await getPermission();
            ViewData["Title"] = "Loại chất liệu";

            if (vm == null || vm.PermitedView == 0)
            {
                return View("Error");
            }
            ViewBag.LoaiDanhMuc = 35;
            return View(vm);
        }
    }
}
