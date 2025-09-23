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

namespace Domain.DiaDiem

{
    public class DiaDiem
    {
        public Guid? DiaDiemID { get; set; }
        public Guid? DiaDiemCapChaID { get; set; }
        public short? LinhVucID { get; set; }
        public Guid? XaID { get; set; }
        public Guid? TinhID { get; set; }
        public decimal? KinhDo { get; set; }   // (11,8) => decimal
        public decimal? ViDo { get; set; }     // (11,8) => decimal
        public short? CaoDo { get; set; }
        public string BanDo { get; set; }
        public string HinhAnh { get; set; }
        public bool? NguoiKhuyetTat { get; set; }
        public bool? NhaVeSinh { get; set; }
        public bool? BaiDoXe { get; set; }
    }
    public class DiaDiemDTO
    {
        public Guid DiaDiemID { get; set; }
        public Guid? DiaDiemCapChaID { get; set; }
        public short? LinhVucID { get; set; }
        public Guid? XaID { get; set; }
        public Guid? TinhID { get; set; }
        public decimal? KinhDo { get; set; }   // (11,8) => decimal
        public decimal? ViDo { get; set; }     // (11,8) => decimal
        public short? CaoDo { get; set; }
        public string BanDo { get; set; }
        public string HinhAnh { get; set; }
        public bool? NguoiKhuyetTat { get; set; }
        public bool? NhaVeSinh { get; set; }
        public bool? BaiDoXe { get; set; }
        //NOI DUNG
        public string MaNgonNgu { get; set; }
        public string TenDiaDiem { get; set; }
        public string MoTa { get; set; }
        public string DiaChi { get; set; }
        public string HeToaDo { get; set; }
        public string ThoiGianHoatDong { get; set; }
        public string SucChua { get; set; }
        //TRINHDIEN
        public string TenLinhVuc { get; set; }
        public string TenXa { get; set; }
        public string TenTinh { get; set; }
    }
    public class DiaDiemAdd
    {
        public string DiaDiem { get; set; }
        public string DiaDiem_NoiDung { get; set; }    
    
    }
    public class DiaDiemRequest
    {
        public Guid? DiaDiemID { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? TrangThai { get; set; }
        public string TuKhoa { get; set; }
        public Guid? DiaDiemCapChaID { get; set; }
        public short? LinhVucID { get; set; }
        public Guid? XaID { get; set; }
        public Guid? TinhID { get; set; }
    }

    public class DM_DiaDiemViewByMaDiaDiem
    {
        public Guid DiaDiemID { get; set; }
        public string TenDiaDiem { get; set; }
        public string DiaChi { get;set; }
        public decimal? KinhDo { get; set; }   // (11,8) => decimal
        public decimal? ViDo { get; set; }     // (11,8) => decimal
    }
}
