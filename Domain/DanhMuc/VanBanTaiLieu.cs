using Domain.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class VanBanTaiLieu
    {
        public Guid VanBanID { get; set; }
        public Guid DoiTuongSoHuuID { get; set; }
        public string SoKyHieu { get; set; }
        public string TrichYeu { get; set; }
        public Guid? CoQuanBanHanhID { get; set; }
        public string NguoiKy { get; set; }
        public string NoiDung { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public string BangThamChieu { get; set; }
    }

    public class VanBanTaiLieuRequest
    {
        public Guid? DoiTuongSoHuuID { get; set; }
    }
    public class VanBanTaiLieuDTO : VanBanTaiLieu
    {
        public string DuongDanFile { get; set; }
        public string TenCoQuanBanHanh { get; set; }
    }

    public class VanBanTaiLieuAdd
    {
        public Guid? VanBanID { get; set; }
        public Guid DoiTuongSoHuuID { get; set; }
        public string SoKyHieu { get; set; }
        public string TrichYeu { get; set; }
        public Guid? CoQuanBanHanhID { get; set; }
        public string NguoiKy { get; set; }
        public string NoiDung { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public string BangThamChieu { get; set; }
        public string DuongDanFile{ get; set; }
        public List<IFormFile> File { get; set; } = new List<IFormFile>();
    }
}
