using Domain.Core;
using Domain.DanhMuc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class ViPham : BaseEntity
    {
        public Guid MaViPham { get; set; }
        public DateTime? NgayPhatHien { get; set; }
        public int? HinhThucXuLyID { get; set; }
        public Guid? DonViViPhamID { get; set; }
        public int? LoaiDonVi { get; set; }
        public bool? TrangThaiXuly { get; set; }
    }
    public class ViPhamRequest
    {
        public Guid? MaViPham { get; set; }
        public string MaNgonNgu { get; set; }
        public Guid? DonViViPhamID { get; set; }
        public bool? TrangThaiXuly { get; set; }
        public string TuKhoa { get; set; }
    }

    public class ViPhamDTO : BaseEntity
    {
        public Guid MaViPham { get; set; }
        public DateTime? NgayPhatHien { get; set; }
        public int? HinhThucXuLyID { get; set; }
        public Guid? DonViViPhamID { get; set; }
        public int? LoaiDonVi { get; set; }
        public bool? TrangThaiXuly { get; set; }
        //VI PHAM - NOI DUNG
        public Guid MaViPhamNoiDungID { get; set; }
        public Guid MaViPhamID { get; set; }
        public string MaNgonNgu { get; set; }
        public string NoiDungViPham { get; set; }
        public string MoTaViPham { get; set; }
        //TRINH DIEN
        public string TenLoaiDonVi { get; set; }
        public string TenDonViViPham { get; set; }
        //DANH SACH FILE
        public List<VanBanTaiLieuDTO> VanBanTaiLieus { get; set; }
    }
    public class ViPhamAdd
    {
        public string ViPham_NoiDung { get; set; }
        public string ViPham { get; set; }
        public List<IFormFile> FileDinhKem { get; set; }
    }
}
