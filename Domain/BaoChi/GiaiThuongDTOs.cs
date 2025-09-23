using Domain.Core;
using Domain.DanhMuc;
using Microsoft.AspNetCore.Http;
using System;

namespace Domain.BaoChi
{
    /// <summary>
    /// DTO mở rộng (có thể bổ sung thêm thuộc tính liên kết nếu cần sau này)
    /// </summary>
    public class GiaiThuongDTO : BaseEntity
    {
        public Guid GiaiThuongID { get; set; }
        public int? NamTraoGiai { get; set; }
        public int? LinhVucID { get; set; }
        public string? TenGiaiThuong { get; set; }
        public Guid GiaiThuongNoiDungID { get; set; }
        public string MaNgonNgu { get; set; } = string.Empty;
        public string HangMucGiaiThuong { get; set; }
        public string MoTa { get; set; }
        public List<VanBanTaiLieuDTO> VanBanTaiLieus { get; set; }
    }

    public class GiaiThuongRequest
    {
        public Guid? GiaiThuongID { get; set; }
        public int? NamTraoGiai { get; set; }
        public string TuKhoa { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? TrangThai { get; set; }
    }

    public class GiaiThuongAdd
    {
        public string GiaiThuong { get; set; }          // JSON chuỗi thông tin chính (nếu cần truyền dạng JSON ở UI)
        public string GiaiThuong_NoiDung { get; set; }   // JSON danh sách nội dung đa ngôn ngữ
        
    }
}
