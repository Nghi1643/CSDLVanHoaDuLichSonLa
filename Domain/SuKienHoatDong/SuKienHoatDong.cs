using Domain.BaoChi;
using Domain.SuKienHoatDong;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SuKienHoatDong

{
    public class DiaDiem
    {
        public Guid SuKienID { get; set; }
        public string MaSuKien { get; set; }
        public string TenSuKien { get; set; }
        public int? LinhVucID { get; set; }
        public string LoaiDoiTuong { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }       
        public string AnhDaiDien { get; set; }
        public Guid? DonViToChucID { get; set; }
        public int? SoKhachThamGia { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int? NguoiCapNhat { get; set; }
        public int? NguoiHieuChinh { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class SuKienHoatDongDTO
    {
        public Guid SuKienID { get; set; }
        public string MaSuKien { get; set; }
        public string TenSuKien { get; set; }
        public int? LinhVucID { get; set; }
        public string LoaiDoiTuong { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
      
        public string AnhDaiDien { get; set; }
        public Guid? DonViToChucID { get; set; }
        public int? SoKhachThamGia { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int? NguoiCapNhat { get; set; }
        public int? NguoiHieuChinh { get; set; }
        public bool? TrangThai { get; set; }


        //NOI DUNG
        public Guid SuKienNoiDungID { get; set; }
        public string MaNgonNgu { get; set; }
        public string MoTa { get; set; }
        public string NoiDung { get; set; }
    }
    public class SuKienHoatDongAdd
    {
        public string SuKienHoatDong { get; set; }
        public string SuKienHoatDong_NoiDung { get; set; }
        public IFormFile FileDinhKem { get; set; }
    }
    public class SuKienHoatDongRequest
    {
        public Guid? SuKienID { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? TrangThai { get; set; }
        public string TuKhoa { get; set; }

        public Guid? DonViToChucID { get; set; }
    }
}
