using Domain.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class XuatBanAnPham : BaseEntity
    {
        public Guid? XuatBanAnPhamID { get; set; }
        public string MaDinhDanhXBAP { get; set; }
        public string TenAnPham { get; set; }
        public string MaQuocTe { get; set; }
        public Guid? NhaXuatBanID { get; set; } // Bảng DM_ToChuc
        public int? NamXuatBan { get; set; }
        public int? TheLoaiID { get; set; }
        public int? SoLanTaiBan { get; set; }
        public int? SoLuongIn { get; set; }
        public int? GiaBia { get; set; }
        public string DuongDanHinhAnhBia { get; set; }
    }

    public class XuatBanAnPhamDTO : XuatBanAnPham
    {
        public string TenTheLoai { get; set; }
        public string TenNhaXuatBan { get; set; }
    }

    public class XuatBanAnPhamRequest
    {
        public Guid? XuatBanAnPhamID { get; set; }
        public string TuKhoa { get; set; }
        public Guid? NhaXuatBanID { get; set; }
        public int? TheLoaiID { get; set; }
        public string MaNgonNgu { get; set; } 
        public bool? TrangThai { get; set; }
    }

    public class XuatBanAnPhamAdd
    {
        public string XuatBanAnPham_NoiDung { get; set; }
        public string XuatBanAnPham { get; set; }
        public IFormFile FileDinhKem { get; set; }
    }

}
