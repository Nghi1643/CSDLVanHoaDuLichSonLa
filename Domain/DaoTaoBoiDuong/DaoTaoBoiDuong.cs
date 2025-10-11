using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DaoTaoBoiDuong
{
    public class DaoTaoBoiDuong//không có các trường trong baseEntity
    {
        public Guid DaoTaoID { get; set; }
        public short LinhVucID { get; set; }
        public Guid? ToChucID { get; set; } 
        public Guid DiaDiemID { get; set; }
        public DateTime BatDau { get; set; }
        public DateTime KetThuc { get; set; }
        public int TrangThaiID { get; set; }
    }
    public class DaoTaoBoiDuongDTO : DaoTaoBoiDuong
    { 
        public string TenLinhVuc { get; set; }
        public string TenToChuc { get; set; }
        public string TenDiaDiem { get; set; }
        public string TenTrangThai { get; set; }
        //
        public string NoiDungDaoTao { get; set; }
        public string DonVi { get; set; }
    }
    public class DaoTaoBoiDuongRequest
    {
        public string TuKhoa { get; set; }
        public short? LinhVucID { get; set; }
        public Guid? ToChucID { get; set; }
        public Guid? DiaDiemID { get; set; }
        public string MaNgonNgu { get; set; }
        public int? TrangThaiID { get; set; }
    }
    public class DaoTaoBoiDuongAdd
    {
        public string DaoTaoBoiDuong_NoiDung { get; set; }
        public string DaoTaoBoiDuong { get; set; }
    }
}
