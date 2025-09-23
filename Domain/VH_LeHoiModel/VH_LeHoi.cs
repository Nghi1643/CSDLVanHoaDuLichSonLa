using Domain.DanhMuc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VH_LeHoiModel
{
    public class VH_LeHoi
    {
        public Guid LeHoiID { get; set; }
        public string TenLeHoi { get; set; }
        public string MaDinhDanhLeHoi { get; set; }
        public int? TheLoaiID { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get;set; }
        public int? TanSuatID { get; set; }
        public int? CapDoID { get; set; }
        public int? CapQuanLyID { get; set; }
        public int? SoLuongThamGia { get;set; }
        public Guid? DonViToChucID { get; set; }
        public int? SoNgayDienRa { get; set; }
        public int? NamVHPHTQG { get; set; }
        public int? DanTocID { get; set; }
        public bool LaTieuBieu { get; set; }
        public bool LaCungDinh { get; set; }
        public bool TrangThaiToChuc { get; set; }
        public bool TrangThai { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int? NguoiCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int? NguoiHieuChinh { get;set; }
        public string ThoiGianDienRa { get; set; }
    }

    public class VH_LeHoi_RequestAdd : VH_LeHoi_NoiDung_Request
    {
        public string TenLeHoi { get; set; }
        public string MaDinhDanhLeHoi { get; set; }
        public int? TheLoaiID { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? TanSuatID { get; set; }
        public int? CapDoID { get; set; }
        public int? CapQuanLyID { get; set; }
        public int? SoLuongThamGia { get; set; }
        public Guid? DonViToChucID { get; set; }
        public int? SoNgayDienRa { get; set; }
        public int? NamVHPHTQG { get; set; }
        public int? DanTocID { get; set; }
        public bool LaTieuBieu { get; set; }
        public bool LaCungDinh { get; set; }
        public bool TrangThaiToChuc { get; set; }
        public bool TrangThai { get; set; }
        public int? NguoiCapNhat { get; set; }
        public string ListIDDiaDiem { get; set; }
        public string ThoiGianDienRa { get; set; }

    }

    public class VH_DiaDiemRequest : Domain.DiaDiem.DiaDiem
    {
        public string MaNgonNgu { get; set; }
        public string TenDiaDiemChild { get; set; }
        public string MoTa { get; set; }
        public string DiaChi { get; set; }
        public string HeToaDo { get; set; }
        public string ThoiGianHoatDong { get; set; }
        public string SucChua { get; set; }
    }

    public class VH_LeHoi_RequestMulti
    {
        public string EntityLeHoi { get; set; }
        public List<DaPhuongTienAdd> EntityDaPhuongTien { get; set; }
    }

    public class VH_LeHoi_RequestUpdate : VH_LeHoi_NoiDung_Request
    {
        public string TenLeHoi { get; set; }
        public string MaDinhDanhLeHoi { get; set; }
        public int? TheLoaiID { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? TanSuatID { get; set; }
        public int? CapDoID { get; set; }
        public int? CapQuanLyID { get; set; }
        public int? SoLuongThamGia { get; set; }
        public Guid? DonViToChucID { get; set; }
        public int? SoNgayDienRa { get; set; }
        public int? NamVHPHTQG { get; set; }
        public int? DanTocID { get; set; }
        public bool LaTieuBieu { get; set; }
        public bool LaCungDinh { get; set; }
        public bool TrangThaiToChuc { get; set; }
        public bool TrangThai { get; set; }
        public int? NguoiHieuChinh { get; set; }
        public string ThoiGianDienRa { get; set; }
        public string ListIDDiaDiem { get; set; }
    }

    public class VH_LeHoiViewModel : VH_LeHoi
    {
        public Guid LeHoiNoiDungID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenLeHoiChild { get; set; }
        public string NhanVatPhungTho { get; set; }
        public string NoiDung { get; set; }
        public string QuiDinh { get; set; }
        public string DanhGia { get; set; }
        public string DonViPhoiHop { get; set; }
        public string DiaDiemChiTiet { get; set; }
        public string TheLoai { get; set; }
        public string TanSuat { get; set; }
        public string CapDo { get; set; }
        public string CapQuanLy { get; set; }
        public string TenToChuc { get; set; }
        public string ListIDDiaDiem { get; set; }
    }

    public class VH_LeHoi_DiaDiem
    {
        public Guid LeHoiID { get; set; }
        public Guid DiaDiemID { get; set; }
    }
}
