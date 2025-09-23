using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DM_CaNhan_VanDongVienModel
{
    public class DM_CaNhan
    {
        public Guid CaNhanID { get; set; }
        public string MaDinhDanh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? GioiTinh { get; set; }
        public int? DanTocID { get; set; }
        public Guid? TinhID { get; set; }
        public Guid? XaID { get; set; }
        public string DienThoai {  get; set; }
        public string HopThu { get; set; }
        public string AnhChanDung { get; set; }
        public int? TrangThaiID { get; set; }
    }

    public class DM_CaNhan_VaiTroXaHoi
    {
        public Guid CaNhanID { get; set; }
        public byte VaiTroID { get; set; }
    }

    public class DM_CaNhan_NoiDung
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

    public class DM_CaNhanRequestHandleInfo : DM_CaNhan_NoiDung
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
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public int? TrangThaiID { get; set; }
        public string TheChat { get; set; }
        public string ThanhTich { get; set; }
        public string AnhChanDungDetail { get; set; }

        public string ToChucID { get; set; }
    }

    public class DM_CaNhan_TrongTaiRequestHandleInfo : DM_CaNhan_NoiDung
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
        public int? TrangThaiID { get; set; }
        public int? CapBacID { get; set; }
        public byte NamKinhNghiem { get; set; }
        public string AnhChanDungDetail { get; set; }
        public string ToChucID { get; set;}
    }

    public class DM_CaNhan_HuanLuyenVienRequestHandleInfo : DM_CaNhan_NoiDung
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
        public int? TrangThaiID { get; set; }
        public string AnhChanDungDetail { get; set; }
        public string ToChucID { get; set; }
    }

    public class DM_CaNhan_CanBoTheTheoRequestHandleInfo : DM_CaNhan_NoiDung
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
        public string AnhChanDungDetail { get; set; }
        public int? TrangThaiID { get; set; }
        public string ToChucID { get; set; }
    }

    public class DM_CaNhan_VanDongVien_NoiDung
    {
        public Guid? CaNhanID { get; set; }
        public byte VaiTroID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TheChat { get; set; }
        public string ThanhTich { get; set; }
        public string GhiChu { get; set; }
    }

    public class DM_CaNhan_MonTheThao
    {
        public Guid? CaNhanID { get; set; }
        public byte VaiTroID { get; set; }
        public Guid MonTheThaoID { get; set; }
        public bool MonTheThaoChinh { get; set; }
    }

    public class DM_CaNhan_MonTheThaoList : DM_CaNhan_MonTheThao
    {
        public string TenMon { get; set; }
    }

    public class DM_CaNhan_MonTheThaoAddRequest
    {
        public Guid MonTheThaoID { get; set; }
        public bool MonTheThaoChinh { get; set; }
    }

    public class DM_CaNhanVDV_AddRequest
    {
        public string RequestVanDongVien { get; set; }
        public IFormFile File { get; set; }

        public string RequestMonTheThao { get; set; }
    }

    public class DM_CaNhanVDV_UpdateRequest
    {
        public string RequestVanDongVien { get; set; }
        public IFormFile File { get; set; }
    }

    public class DM_CaNhanTrongTai_AddRequest
    {
        public string RequestTrongTai { get; set; }
        public IFormFile File { get; set; }

        public string RequestMonTheThao { get; set; }
    }

    public class DM_CaNhanHuanLuyenVien_AddRequest
    {
        public string RequestHuanLuyenVien { get; set; }
        public IFormFile File { get; set; }

        public string RequestMonTheThao { get; set; }
    }

    public class DM_CaNhanHuanLuyenVien_UpdateRequest
    {
        public string RequestHuanLuyenVien { get; set; }
        public IFormFile File { get; set; }
    }

    public class DM_CaNhanCanBoTheThao_Request
    {
        public string RequestCanBoTheThao { get; set; }
        public IFormFile File { get; set; }
    }

    public class DM_CaNhan_VanDongVienViewModel : DM_CaNhan_NoiDung
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
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string ThanhTich { get; set; }
        public string TheChat { get; set; }
        public string TenTinh { get; set; }
        public string TenXa { get; set; }
        public byte SoLuongMon { get; set; }
    }

    public class DM_CaNhan_TrongTai_UpdateRequest
    {
        public string RequestTrongTai { get; set; }
        public IFormFile File { get; set; }
    }

    public class DM_CaNhan_TrongTaiViewModel : DM_CaNhan_NoiDung
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
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string TenTinh { get; set; }
        public string TenXa { get; set; }
        public byte NamKinhNghiem { get; set; }
        public string TenCapBac { get; set; }
        public byte SoLuongMon { get; set; }
    }

    public class DM_CaNhan_HuanLuyenVienViewModel : DM_CaNhan_NoiDung
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
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string TenTinh { get; set; }
        public string TenXa { get; set; }
        public byte SoLuongMon { get; set; }
    }

    public class DM_CaNhan_CanBoTheThaoViewModel : DM_CaNhan_NoiDung
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
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string TenTinh { get; set; }
        public string TenXa { get; set; }
    }


    public class DM_CaNhan_ToChuc
    {
        public Guid CaNhanID { get; set; }
        public Guid ToChucID { get; set; }
    }
}
