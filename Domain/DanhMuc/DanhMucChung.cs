using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class DanhMucChung
    {
        public int DanhMucID { get; set; }
        public short? LoaiDanhMucID { get; set; }
        public string MaSo { get; set; }
        public int? ThuTu { get; set; }
        public bool? TrangThai { get; set; }
        public string Ten { get; set; }
    }

    public class DanhMucChungDTO
    {
        // Dữ liệu thuộc DanhMucChung
        public int DanhMucID { get; set; }
        public short? LoaiDanhMucID { get; set; }
        public string MaSo { get; set; }
        public int? ThuTu { get; set; }
        public bool? TrangThai { get; set; }

        // Dữ liệu thuộc DanhMucChung_NoiDung
        public int DanhMuc_NoiDungID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenDanhMuc { get; set; }
        public string MoTaDanhMuc { get; set; }

        //Dữ liệu thuộc LinhVuc
        public short? LinhVucID { get; set; }
        public short? LinhVucCapChaID { get; set; }
        public string Ten { get; set; }
        
        //Dữ liệu thuộc NgonNgu
       public string TenNgonNgu { get; set; }

    }

    public class DanhMucChungRequest
    {
        public short? LoaiDanhMucID { get; set; }
        public string TenDanhMuc { get;set; }
        public string TuKhoa { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? TrangThai { get; set; }
    }

    public class DanhMucChungAdd {
        public int? DanhMucID { get; set; }
        public short? LoaiDanhMucID{ get; set; }
        public string MaSo { get; set; }
        public int? ThuTu { get; set; }
        public bool? TrangThai { get; set; }
        public string Ten { get; set; }
        public List<DanhMucChung_NoiDung> DanhMucChung_NoiDungs { get; set; }
    }
}
