namespace Domain.CaNhan
{

    public class CaNhan_NoiDung
    {
        public Guid CaNhanID { get; set; }
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
        public bool IsDeleted { get; set; }
    }

}
