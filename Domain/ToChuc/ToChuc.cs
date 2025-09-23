using Domain.BaoChi;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ToChuc
{
    public class ToChuc
    {
        public Guid ToChucID { get; set; }
        public int? LoaiToChucID { get; set; }
        public int? LinhVucID { get; set; }
        public string MaDinhDanh { get; set; }
        public int? TrangThaiID { get; set; }
        public string HopThu { get; set; }
        public string TuKhoa { get; set; }
        public string DienThoai { get; set; }
        public string Website { get; set; }
        public DateTime? NgayThanhLap { get; set; }
        public string SoGiayPhepHoatDong { get; set; }
        public int? CoQuanChuQuanID { get; set; }
        public int? PhamViHoatDongID { get; set; }
        public double? ToaDoX { get; set; }
        public double? ToaDoY { get; set; }
        public int LoaiHinhID { get; set; }
    }
    public class ToChucDTO
    {
        //TO CHUC
        public Guid ToChucID { get; set; }
        public int? LoaiToChucID { get; set; }
        public int? LinhVucID { get; set; }
        public string MaDinhDanh { get; set; }
        public int? TrangThaiID { get; set; }
        public string HopThu { get; set; }
        public string TuKhoa { get; set; }
        public string DienThoai { get; set; }
        public string Website { get; set; }
        public DateTime? NgayThanhLap { get; set; }
        public string SoGiayPhepHoatDong { get; set; }
        public int? CoQuanChuQuanID { get; set; }
        public int? PhamViHoatDongID { get; set; }
        public double? ToaDoX { get; set; }
        public double? ToaDoY { get; set; }
        public int LoaiHinhID { get; set; }
        // TO CHUC _ NOI DUNG
        public int ID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenToChuc { get; set; }
        public string DiaChi { get; set; }
        public string GioiThieu { get; set; }

        public string GhiChu { get; set; }
        // NGON NGU
        public string TenNgonNgu { get; set; }
        // TRINH DIEN
        public int SoLuongCanBo { get; set; }
        public int SoLuongAnPham { get; set; }
        public int SoLuongXuatBan { get; set; }
        public string CoQuanChuQuan { get; set; }
        public string PhamViHoatDong { get; set; }
        public string LoaiHinh { get; set; }
        public string QuyMo { get; set; }
        public Guid? TinhID { get; set; }
        public string TenTinh { get; set; }
        public Guid? XaID { get; set; }
        public string TenXa { get; set; }
        
    }
    public class ToChucRequest
    {
        public string MaNgonNgu { get; set; }
        public int? TrangThaiID { get; set; }
        public string TuKhoa { get; set; }
        public int LoaiToChucID { get; set; }

        public int? CoQuanChuQuanID { get; set; }
        public int? PhamViHoatDongID { get; set; }
        public int? LoaiHinhID { get; set; }
        public Guid? ToChucID { get; set; }

    }

    public class ToChucAdd
    {
        public Guid? ToChucID { get; set; }
        public int? LoaiToChucID { get; set; }
        public int? LinhVucID { get; set; }
        public string MaDinhDanh { get; set; }
        public int? TrangThaiID { get; set; }
        public string HopThu { get; set; }
        public string TuKhoa { get; set; }
        public string DienThoai { get; set; }
        public string Website { get; set; }
        public DateTime? NgayThanhLap { get; set; }
        public string SoGiayPhepHoatDong { get; set; }
        public int? CoQuanChuQuanID { get; set; }
        public int? PhamViHoatDongID { get; set; }
        public int? LoaiHinhID { get; set; }
        public double? ToaDoX { get; set; }
        public double? ToaDoY { get; set; }
        public Guid? TinhID { get; set; }
        public Guid? XaID { get; set; }
        public string QuyMo { get; set; }
        public List<ToChuc_NoiDung> ToChuc_NoiDungs { get; set; }
    }

    public class DM_ToChucBaseRequest : ToChuc_NoiDung
    {
        public int? LoaiToChucID { get; set; }
        public int? LinhVucID { get; set; }
        public string MaDinhDanh { get; set; }
        public int? TrangThaiID { get; set; }
        public string HopThu { get; set; }
        public string TuKhoa { get; set; }
        public string DienThoai { get; set; }
        public string Website { get; set; }
        public DateTime? NgayThanhLap { get; set; }
        public string SoGiayPhepHoatDong { get; set; }
        public int? CoQuanChuQuanID { get; set; }
        public int? PhamViHoatDongID { get; set; }
        public double? ToaDoX { get; set; }
        public double? ToaDoY { get; set; }
        public int LoaiHinhID { get; set; }

        public Guid? TinhID { get; set; }
        public Guid? XaID { get; set; }
        public string QuyMo { get; set; }
    }



    public class BC_AnPhamBaseRequest : BaoChiAnPham_NoiDung
    {

        public Guid? ToChucID { get; set; }

        public int? TanSuatID { get; set; }

        public int LinhVucChuyenSauID { get; set; }

        public int? SoLuong { get; set; }
        public int? NguoiCapNhat { get; set; }
        public int? NguoiHieuChinh { get; set; }
        public bool TrangThai { get; set; }
    }

    public class BC_NhaXuatBanRequest
    {
        public string RequestToChuc { get; set; } // JSON string của ToChuc
        public List<XuatBanAnPhamAdd> RequestXuatBan { get; set; } // được bind từ form fields index
    }

    public class BC_ToChucBaoChiRequest
    {
        public string ReqeustToChuc { get; set; }
        public string RequestAnPham { get; set; }
    }

    public class ToChuc_AnPham_Add
    {
        public string ToChucAdd { get; set; }
        public string XuatBanAnPham_NoiDung { get; set; }
        public string XuatBanAnPham { get; set; }
        public IFormFile FileDinhKem { get; set; }
    }
}
