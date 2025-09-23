using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class ThongTinDiaDiem
    {
        public Guid ThongTinDiaDiemID { get; set; }

        public Guid DoiTuongSoHuuID { get; set; }

        public string LoaiDoiTuong { get; set; }

        public string TenDiaDiem { get; set; }

        public string DiaChi { get; set; }

        public string SoNha { get; set; }

        public string Duong { get; set; }

        public string ThonBan { get; set; }

        public Guid XaPhuongID { get; set; }

        public Guid TinhThanhID { get; set; }

        public bool? CoHoTroNKT { get; set; }

        public bool? CoNVS { get; set; }

        public bool? CoBaiDoXe { get; set; }

        public bool? CoHuongDanVien { get; set; }

        public string GioMoCua { get; set; }

        public string GioDongCua { get; set; }

        public string GiaVe { get; set; }

        public string NguoiLienHe { get; set; }

        public string Website { get; set; }

        public string DienThoai { get; set; }

        public string Email { get; set; }

        public string MXH { get; set; }

        public string ToaDoX { get; set; }

        public string ToaDoY { get; set; }
    }
}
