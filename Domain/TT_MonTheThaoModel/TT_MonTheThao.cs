using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Domain.TT_MonTheThaoModel
{
    public class TT_MonTheThao
    {
        public Guid MonTheThaoID { get; set; }
        public string MaDinhDanh { get; set; }
        public int CachThiDauID { get; set; }
        public string TepKemTheo { get; set; }
        public int TrangThaiID { get; set; }
        public bool TrangThai {  get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int NguoiHieuChinh { get; set; }
    }

    public class TT_MonTheThao_AddRequest : TT_MonTheThao_NoiDung
    {
        public string MaDinhDanh { get; set; }
        public int CachThiDauID { get; set; }
        public string TepKemTheo { get; set; }
        public int TrangThaiID { get; set; }
        public string Ten { get; set; }
        public int NguoiCapNhat { get; set; }
        public bool TrangThai { get; set; }
    }

    public class TT_MonTheThao_UpdateRequest : TT_MonTheThao_NoiDung
    {
        public string MaDinhDanh { get; set; }
        public int CachThiDauID { get; set; }
        public string TepKemTheo { get; set; }
        public string TepKemTheoDetail { get; set; }
        public int TrangThaiID { get; set; }
        public string Ten { get; set; }
        public DateTime NgayHieuChinh { get; set; }
        public int NguoiHieuChinh { get; set; }
        public bool TrangThai { get; set; }
    }

    public class TT_MonTheThaoRequestAddFile
    {
        public IFormFile File { get; set; }
        public string Data { get; set; }
    }

    public class TT_MonTheThaoViewModel : TT_MonTheThao_NoiDung
    {
        public string TepKemTheo { get; set; }
        public string MaDinhDanh { get; set; }
        public string CachThiDau { get; set; }
        public string TrangThai { get; set; }
        public int CachThiDauID { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int NguoiHieuChinh { get; set; }
    }
}
