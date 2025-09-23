using Domain.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class TacPham : BaseEntity
    {
        public Guid TacPhamID { get; set; }
        public string MaDinhDanh { get; set; }
        public int? LinhVucID { get; set; }
        public int? TheLoaiID { get; set; }
        public int? NamCongBo { get; set; }
        public string HinhAnh { get; set; }
        public int ThuTu { get; set; }
    }

    public class TacPhamDTO : TacPham
    {
        public string TenTheLoai { get; set; }
        public string TenTacPham { get; set; }
        public string TacGia { get; set; }
        public List<TacPham_NoiDung> BanDich { get; set; }
    }

    public class TacPhamRequest
    {
        public string TuKhoa { get; set; }
        public int? TheLoaiID { get; set; }
        public int? LinhVucID { get; set; }
        public bool? SuDung { get; set; }
        public string MaNgonNgu { get; set; }
    }
    
    public class TacPhamAdd : BaseEntity
    {
        public Guid? TacPhamID { get; set; }
        public string MaDinhDanh { get; set; }
        public int? LinhVucID { get; set; }
        public int? TheLoaiID { get; set; }
        public int? NamCongBo { get; set; }
        public string HinhAnh { get; set; }
        public int ThuTu { get; set; }
    }

    public class TacPhamJson
    {
        public string TacPham { get; set; }
        public string TacPham_NoiDung { get; set; }
        public IFormFile HinhAnh { get; set; }
    }
}
