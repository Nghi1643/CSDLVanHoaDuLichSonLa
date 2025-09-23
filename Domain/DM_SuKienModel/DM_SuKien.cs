using Domain.DanhMuc;
using Microsoft.AspNetCore.Http;

namespace Domain.DM_SuKienModel
{
    public class DM_SuKien
    {
        public Guid SuKienID { get; set; }
        public short LinhVucID { get; set; }
        public string MaDinhDanh { get; set; }
        public string AnhDaiDien { get; set; }
        public int CapDoID { get; set; }
        public DateTime? BatDau { get; set; }
        public DateTime? KetThuc { get; set; }
        public int TrangThaiID { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int NguoiHieuChinh { get; set; }
        public int ThuTu { get; set; }
        public bool TrangThai { get; set; }
    }

    public class DM_SuKien_NoiDung
    {
        public Guid? SuKienID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenSuKien { get; set; }
        public string MoTa { get; set; }
        public string KetQua { get; set; }
        public string NoiDung { get; set; }
    }

    public class DM_SuKien_ToChuc
    {
        public int ID { get; set; }
        public Guid? SuKienID { get; set; }
        public Guid? ToChucID { get; set; }
        public int? VaiTroID { get; set; }
        public int? XepHangID { get; set; }
    }

    public class DM_SuKien_DiaDiem
    {
        public Guid DiaDiemID { get; set; }
        public Guid SuKienID { get; set; }
    }

    public class DM_SuKien_MonTheThao
    {
        public Guid? MonTheThaoID { get; set; }
        public Guid SuKienID { get; set; }
    }

    public class DM_SuKienModalAdd : DM_SuKien_NoiDung
    {
        public short LinhVucID { get; set; }
        public string MaDinhDanh { get; set; }
        public string AnhDaiDien { get; set; }
        public int CapDoID { get; set; }
        public DateTime? BatDau { get; set; }
        public DateTime? KetThuc { get; set; }
        public int TrangThaiID { get; set; }
        public int NguoiCapNhat { get; set; }
        public int ThuTu { get; set; }
        public string ListID { get; set; }
        public string ListIDDiaDiem { get; set; }
        public bool TrangThai { get; set; }
    }

    public class DM_SuKien_RequestAdd
    {
        public string EntitySuKien { get; set; }
        public IFormFile File { get; set; }
        public List<DaPhuongTienAdd> EntityDaPhuongTien { get; set; }
    }

    public class DM_SuKien_RequestUpdate
    {
        public string EntitySuKien { get; set; }
        public IFormFile File { get; set; }
    }

    public class DM_SuKienModalUpdate : DM_SuKien_NoiDung
    {
        public short LinhVucID { get; set; }
        public string MaDinhDanh { get; set; }
        public string AnhDaiDien { get; set; }
        public string AnhDaiDienDetail { get; set; }
        public int CapDoID { get; set; }
        public DateTime? BatDau { get; set; }
        public DateTime? KetThuc { get; set; }
        public int TrangThaiID { get; set; }
        public int NguoiHieuChinh { get; set; }
        public int ThuTu { get; set; }
        public string ListID { get; set; }
        public string ListIDDiaDiem { get; set; }
        public bool TrangThai { get; set; }
    }

    public class DM_SuKienRequestAdd
    {
        public string SuKienRequest { get; set; }
        public IFormFile File { get; set; }
    }

    public class DM_SuKienViewModel : DM_SuKien
    {
        public string MaNgonNgu { get; set; }
        public string TenSuKien { get; set; }
        public string MoTa { get; set; }
        public string KetQua { get; set; }
        public string TenTrangThai { get; set; }
        public string TenLinhVuc { get; set; }
        public string TenCapDo { get; set; }
        public string ListIDDiaDiem { get; set; }
    }
}
