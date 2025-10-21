using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TourDuLich
{
    public class TourDuLich_LichTrinh
    {
        public int LichTrinhID { get; set; }
        public Guid? TourID { get; set; }
        public Guid? DiaDiemID { get; set; }
        public DateTime? ThoiDiem { get; set; }
        public byte? ThuTu { get; set; }
    }
    public class TourDuLich_LichTrinhDTO : TourDuLich_LichTrinh
    {
        public string HoatDong { get; set; }
        public string SoGio { get; set; }
        public string TenDiaDiem { get; set; }
        public string DiaChi { get; set; }
    }
    public class TourDuLich_LichTrinhAdd
    {
        public string TourDuLich_LichTrinh { get; set; }
        public string TourDuLich_LichTrinh_NoiDung { get; set; }

    }
    //public class TourDuLich_LichTrinhAdd1
    //{
    //    public TourDuLich_LichTrinh TourDuLich_LichTrinh { get; set; } = new TourDuLich_LichTrinh();
    //    public TourDuLich_LichTrinh_NoiDung TourDuLich_LichTrinh_NoiDung { get; set; }= new TourDuLich_LichTrinh_NoiDung();
    //}
}
