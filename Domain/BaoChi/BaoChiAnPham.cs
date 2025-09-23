using Domain.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class BaoChiAnPham : BaseEntity
    {
        public Guid BaoChiAnPhamID { get; set; }

        public string TenAnPham { get; set; }

        public Guid? ToChucID { get; set; }

        public int? TanSuatID { get; set; }

        public int LinhVucChuyenSauID { get; set; }

        public int? SoLuong { get; set; }
    }

    public class BaoChiAnPhamDTO : BaoChiAnPham
    {
        public string TenTanSuat { get; set; }
        public string TenCoQuanBaoChi { get; set; }
        public string TenLinhVucChuyenSau { get; set; }
        public string DoiTuongDocGia { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public string TenNguoiHieuChinh { get; set; }
        
    }

    public class BaoChiAnPhamRequest
    {
        public Guid? BaoChiAnPhamID { get; set; }
        public string TuKhoa { get; set; }
        public Guid? ToChucID { get; set; }
        public int? TanSuatID { get; set; }
        public int? LinhVucChuyenSauID { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? TrangThai { get; set; }
    }

    public class BaoChiAnPhamAdd
    {
        public string BaoChiAnPham_NoiDung { get; set; }
        public string BaoChiAnPham { get; set; }
    }
}
