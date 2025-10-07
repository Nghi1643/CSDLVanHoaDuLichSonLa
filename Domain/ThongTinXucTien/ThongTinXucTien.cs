using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ThongTinXucTien
{
    public class ThongTinXucTien//không có các trường trong baseEntity
    {
        public Guid XucTienID { get; set; }
        public short? LinhVucID { get; set; }
        public Guid? ToChucID { get; set; }
        public int? DoiTuongID { get; set; }
        public int? HinhThucID { get; set; }
        public int? TruyenThongID { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? TrangThaiID { get; set; }
    }
    public class ThongTinXucTienDTO:ThongTinXucTien
    {
        public string TenLinhVuc { get; set; }
        public string TenToChuc { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenHinhThuc { get; set; }
        public string TenTruyenThong { get; set; }
        public string TenTrangThai { get; set; }

        //
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
    }
    public class ThongTinXucTienRequest
    {
        //public Guid? XucTienID { get; set; }
        public string TuKhoa { get; set; }
        public short? LinhVucID { get; set; }//không dùng
        public Guid? ToChucID { get; set; }//không dùng
        public int? DoiTuongID { get; set; }//không dùng
        public int? HinhThucID { get; set; }
        public int? TruyenThongID { get; set; }
        public string MaNgonNgu { get; set; }
        public int? TrangThaiID { get; set; }
    }
    public class ThongTinXucTienAdd
    {
        public string ThongTinXucTien_NoiDung { get; set; }
        public string ThongTinXucTien { get; set; }
    }
}
