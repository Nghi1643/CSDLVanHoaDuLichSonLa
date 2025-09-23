using Domain.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class DiSanPhiVatThe : BaseEntity
    {
        public Guid DiSanID { get; set; }
        public int? LoaiHinhID { get; set; }
        public int? TinhTrangID { get; set; }
        public string MaDinhDanh { get; set; }
        public string AnhDaiDien { get; set; }
        public int? ThuTu { get; set; }
    }
    public class DiSanPhiVatTheDTO : DiSanPhiVatThe
    {
        public string TenDiSan { get; set; }
        public string TenLoaiHinh { get; set; }
        public string TenTinhTrang { get; set; }
        public List<DiSanPhiVatThe_NoiDung> BanDich { get; set; }
    }

    public class DiSanPhiVatTheRequest
    {
        public string TuKhoa { get; set; }
        public int? LoaiHinhID { get; set; }
        public int? TinhTrangID { get; set; }
        public bool? SuDung { get; set; }
        public string MaNgonNgu { get; set; }
    }

    public class DiSanPhiVatTheAdd : DiSanPhiVatThe
    {
        public new Guid? DiSanID { get; set; }
    }

    public class DiSanPhiVatTheJson
    {
        public string PhiVatThe { get; set; }
        public string PhiVatThe_NoiDung { get; set; }
        public IFormFile AnhDaiDien { get; set; }
    }
}
