using Domain.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class DaPhuongTien : BaseEntity
    {
        public Guid DaPhuongTienID { get; set; }
        public Guid DoiTuongSoHuuID { get; set; }
        public string LoaiDoiTuong { get; set; }
        public int LoaiMedia { get; set; }
        public int TheLoaiID { get; set; }
        public string DuongDanFile { get; set; }
        public string TacGia { get; set; }
        public int ThuTuHienThi { get; set; }
    }

    public class DaPhuongTienAdd
    {
        public string DaPhuongTien { get; set; }
        public string DaPhuongTien_NoiDung { get; set; }
        public IFormFile File { get; set; }
    }

    public class DaPhuongTienDTO : DaPhuongTien
    {
       public string TieuDe { get; set; }
       public string MoTa { get; set; }
    }
}
