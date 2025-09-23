using Domain.Core;
using System;

namespace Domain.BaoChi
{
    /// <summary>
    /// Nội dung (bản dịch) của giải thưởng theo ngôn ngữ
    /// </summary>
    public class GiaiThuong_NoiDung : BaseEntity
    {

        public Guid GiaiThuongNoiDungID { get; set; }
        public Guid GiaiThuongID { get; set; }
        public string MaNgonNgu { get; set; } = string.Empty;
        public string TenGiaiThuong { get; set; } = string.Empty;
        public string? HangMucGiaiThuong { get; set; }
        public string? MoTa { get; set; }       
    }
}
