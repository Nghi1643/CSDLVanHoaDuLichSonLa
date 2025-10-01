using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoSoKinhDoanhDichVuDuLich
{
    public class CoSoAnUong
    {
        public Guid CoSoAnUongID { get; set; }
        public Guid DiaDiemID { get; set; }
        public int LoaiDichVuAnUongID { get; set; }
        public int? SoLuotDanhGia { get; set; }
        public int? DiemDanhGia { get; set; }
        public int? SucChua { get; set; }
        public bool? TiecGiaDinh { get; set; }
        public bool? TiecDongNguoi { get; set; }
        public bool? CoPhongRieng { get; set; }
        public bool? CoVuiChoiTreEm { get; set; }
        public bool? ChuanDuLich { get; set; }

        public string DienThoai { get; set; }
        public string HopThu { get; set; }
        public string LienKet { get; set; }
        public string NgayNghiHangTuan { get; set; }

        public TimeSpan? GioMoCua { get; set; }
        public TimeSpan? GioDongCua { get; set; }

        //tên trương khác với BaseEntity 
        public bool? TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public int NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int? NguoiCapNhat { get; set; }

    }
    public class CoSoAnUongDTO : CoSoAnUong
    {
        public string TenLoaiDichVuAnUong { get; set; }
        public string TenDiaDiem { get; set; }
        //
        public string TenCoSo { get; set; }
        public string MoTa { get; set; }
        public string DiaChi { get; set; }
        public string NguoiDaiDien { get; set; }
    }
    public class CoSoAnUongRequest
    {
        //public Guid? AnUongID { get; set; }
        public string TuKhoa { get; set; }
        public int? LoaiDichVuAnUongID { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class CoSoAnUongAdd
    {
        public string CoSoAnUong { get; set; }
        public string CoSoAnUong_NoiDung { get; set; }
    }
}
