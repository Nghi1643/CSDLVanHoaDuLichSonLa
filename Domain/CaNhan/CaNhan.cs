using Microsoft.AspNetCore.Http;

namespace Domain.CaNhan
{
    public class VanBan 
    {
        public Guid CaNhanID { get; set; }
        public string MaDinhDanh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? GioiTinhID { get; set; }
        public int? DanTocID { get; set; }
        public Guid? TinhID { get; set; }
        public Guid? XaID { get; set; }
        public string DienThoai { get; set; }
        public string HopThu { get; set; }
        public string AnhChanDung { get; set; }
        public int? TrangThaiID { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CaNhan_BaoChi_Add
    {
        public string CaNhan { get; set; }
        public string CaNhan_NoiDung { get; set; }
        public string CaNhan_BaoChi { get; set; }
        public string CaNhan_BaoChi_NoiDung { get; set; }
        public IFormFile FileDinhKem { get; set; }
    }
    public class CaNhanDTO
    {
        public Guid CaNhanID { get; set; }
        public string MaDinhDanh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? GioiTinhID { get; set; }
        public int? DanTocID { get; set; }
        public Guid? TinhID { get; set; }
        public Guid? XaID { get; set; }
        public string DienThoai { get; set; }
        public string HopThu { get; set; }
        public string AnhChanDung { get; set; }
        public int? TrangThaiID { get; set; }
        public bool IsDeleted { get; set; }

        //NOI DUNG DTO
        public string MaNgonNgu { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string NoiLamViec { get; set; }
        public string MoTa { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int? NguoiCapNhat { get; set; }
        public int? NguoiHieuChinh { get; set; }    

    }
    public class CaNhanRequest
    {
        public Guid? CaNhanID { get; set; }
        public int? VaiTroID { get; set; }
        public string TuKhoa { get; set; }
        public string IsDeleted { get; set; }
        public int? TrangThaiID { get; set; }
        public string MaNgonNgu { get; set; }
    }

}
