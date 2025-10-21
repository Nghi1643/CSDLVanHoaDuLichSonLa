using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TourDuLich
{
    public class TourDuLich
    {
        public Guid TourID { get; set; }           
        public Guid? ToChucID { get; set; }
        public string? MaDinhDanh { get; set; }
        public Guid? TheLoaiID { get; set; }
        public int? LoaiHinhID { get; set; }
        public int? PhanKhucID { get; set; }
        public Guid? DiemDiID { get; set; }
        public Guid? DiemDenID { get; set; }
        public DateTime? NgayKhoiHanh { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public short? SoKhachToiThieu { get; set; }
        public short? SoKhachToiDa { get; set; }
        public long? GiaTour { get; set; }
        public long? GiaNguoi { get; set; }
        public string? MaNgonNgu { get; set; }="";
        public int? PhuongTienID { get; set; }
        public int? SoLuotDanhGia { get; set; }
        public int? DiemDanhGia { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class TourDuLichDTO:TourDuLich
    {
        public string TenTour { get; set; }
        public string MoTa { get; set; }
        public string ThoiGian { get; set; }
        public string TraiNghiem { get; set; }
        public string GoiY { get; set; }
        //
        public string TenToChuc { get; set; }
        public string TenTheLoai { get; set; }
        public string TenLoaiHinh { get; set; }
        public string TenPhanKhuc { get; set; }
        public string TenDiemDi { get; set; }
        public string TenDiemDen { get; set; }
        public string TenNgonNgu { get; set; }
        public string TenPhuongTien { get; set; }

    }
    public class TourDuLichRequest
    {
        public string TuKhoa { get; set; }="";
        public Guid? ToChucID { get; set; }
        public Guid? TheLoaiID { get; set; }
        public int? LoaiHinhID { get; set; }
        public int? PhanKhucID { get; set; }
        public int? PhuongTienID { get; set; }
        public string MaNgonNgu { get; set; }="";
        public bool? TrangThai { get; set; }
    }
    public class TourDuLichAdd
    {
        public string TourDuLich { get; set; }
        public string TourDuLich_NoiDung { get; set; }
    }
}
