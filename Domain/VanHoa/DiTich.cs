using Domain.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class DiTich : BaseEntity
    {
        public Guid DiTichID { get; set; }
        public Guid? DiaDiemID { get; set; }
        //public byte? LinhVucID { get; set; }
        public string AnhDaiDien { get; set; }
        public string TenDiTich { get; set; }
        public string MaDinhDanh { get; set; }
        public int? XepHangID { get; set; }
        public int? LoaiHinhID { get; set; }
        public DateTime? NgayCongNhan { get; set; }
        public int? TrangThaiID { get; set; }
        public Guid? ToChucQuanLyID { get; set; }
        public int? ThuTu { get; set; }
    }

    public class DiTichDTO : DiTich
    {
        public string TenCapXepHang { get; set; }
        public string TenLoaiHinh { get; set; }
        public string TenTrangThai { get; set; }
        public string TenToChucQuanLy { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public string TenNguoiHieuChinh { get; set; }
        public List<DiTich_NoiDung> BanDich { get; set; }
    }

    public class DiTichRequest
    {
        public string TuKhoa { get; set; }
        public int? CapXepHangID { get; set; }
        public int? LoaiHinhID { get; set; }
        public int? TrangThaiID { get; set; }
        public Guid? ToChucQuanLyID { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? SuDung { get; set; }
    }
    public class DiTichAdd
    {
        public string DiTich { get; set; }
        public string DiTich_NoiDung { get; set; }
        public IFormFile AnhDaiDien { get; set; }
    }
}
