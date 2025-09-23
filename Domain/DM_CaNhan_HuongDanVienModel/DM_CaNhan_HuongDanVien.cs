using Domain.DM_CaNhan_BaoChiModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DM_CaNhan_HuongDanVienModel
{
    public class DM_CaNhan_HuongDanVien
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
    }

    public class DM_CaNhan_HuongDanVien_NoiDung
    {
        public Guid? CaNhanID { get; set; }
        public string MaNgonNgu { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string NoiLamViec { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int? NguoiCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int NguoiHieuChinh { get; set; }
        public bool? IsDeleted { get; set; }
        public string GhiChu { get; set; }
    }

    public class DM_CaNhan_HuongDanVien_RequestInfo : DM_CaNhan_HuongDanVien_NoiDung
    {
        public string MaDinhDanh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? GioiTinhID { get; set; }
        public int? DanTocID { get; set; }
        public Guid? TinhID { get; set; }
        public Guid? XaID { get; set; }
        public string DienThoai { get; set; }
        public string HopThu { get; set; }
        public string AnhChanDung { get; set; }
        public string AnhChanDungDetail { get; set; }
        public int? TrangThaiID { get; set; }
        public string SoThe { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public int? LoaiTheID { get; set; }
        public string NamKinhNghiem { get; set; }
        public string NoiCapID { get; set; }
        public string ToChucID { get; set; }
    }

    public class DM_CaNhan_NoiDungHuongDanVien
    {
        public Guid CaNhanID { get; set; }
        public byte VaiTroID { get; set; }
        public string MaNgonNgu { get; set; }
        public string SoThe { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public int? LoaiTheID { get; set; }
        public string NamKinhNghiem { get; set; }
        public string NoiCapID { get; set; }
    }

    public class DM_CaNhan_HuongDanVienViewModel : DM_CaNhan_HuongDanVien_NoiDung
    {
        public string MaDinhDanh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? GioiTinh { get; set; }
        public int? DanTocID { get; set; }
        public Guid? TinhID { get; set; }
        public Guid? XaID { get; set; }
        public string DienThoai { get; set; }
        public string HopThu { get; set; }
        public string AnhChanDung { get; set; }
        public string SoThe { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public byte? NamKinhNghiem { get; set; }
        public Guid? NoiCap { get; set; }
        public int? LoaiTheID { get; set; }
        public string TenTinh { get; set; }
        public string TenXa { get; set; }
        public string TenLoaiThe { get; set; }
        public string TenNoiCap { get; set; }

    }

    public class DM_CaNhanHuongDanVienRequestInfo
    {
        public string RequestHuongDanVien { get; set; }
        public IFormFile File { get; set; }
    }
}
