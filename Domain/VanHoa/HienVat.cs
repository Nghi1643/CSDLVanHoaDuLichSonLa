using Domain.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class HienVat : BaseEntity
    {
        public Guid HienVatID { get; set; }
        public int? LoaiHienVatID { get; set; }
        public string MaHienVat { get; set; }
        public int? ChatLieuID { get; set; }
        public int? PhuongThucTangMuaID { get; set; }
        public Guid? BaoTangID { get; set; }
        public int? TrangThaiID { get; set; }
        public string TenHienVat { get; set; }
        public string ThuTu { get; set; }
        public string AnhDaiDien { get; set; }
    }

    public class HienVatDTO : HienVat
    {
        public string TenLoaiHienVat { get; set; }
        public string TenChatLieu { get; set; }
        public string TenPhuongThucTangMuaID { get; set; }
        public string TenTrangThai { get; set; }
        public string TenBaoTang { get; set; }
        public string SoLuong { get; set; }
        public List<HienVat_NoiDung> BanDich { get; set; }
    }
    public class HienVatRequest
    {
        public string TuKhoa { get; set; }
        public int? LoaiHienVatID { get; set; }
        public int? ChatLieuID { get; set; }
        public int? PhuongThucTangMuaID { get; set; }
        public Guid? BaoTangID { get; set; }
        public int? TrangThaiID { get; set; }
        public bool? SuDung { get; set; }
        public string MaNgonNgu { get; set; }
    }
    public class HienVatAdd : HienVat
    {
        public new Guid? HienVatID { get; set; }
    }
    public class HienVatJson
    {
        public string HienVat { get; set; }
        public string HienVat_NoiDung { get; set; }
        public IFormFile AnhDaiDien { get; set; }
    }
}
