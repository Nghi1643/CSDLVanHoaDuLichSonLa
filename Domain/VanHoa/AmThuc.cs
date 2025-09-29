using Domain.Core;
using Microsoft.AspNetCore.Http;

namespace Domain.VanHoa
{
    public class AmThuc : BaseEntity
    {
        public Guid MonAnUongID { get; set; }
        public int? KieuMonID { get; set; }
        public int? CheDoAnID { get; set; }
        public string AnhDaiDien { get; set; }
        public bool? DacSan { get; set; }
        public int? ThuTu { get; set; }
    }
    public class AmThucDTO : AmThuc
    {
        public string TenMon { get; set; }
        public string TenKieuMon { get; set; }
        public string TenCheDoAn { get; set; }
        public List<AmThuc_NoiDung> BanDich { get; set; }
    }

    public class AmThucRequest
    {
        public string TuKhoa { get; set; }
        public int? KieuMonID { get; set; }
        public int? CheDoAnID { get; set; }
        public bool? DacSan { get; set; }
        public string MaNgonNgu { get; set; }
        public bool? SuDung { get; set; }
    }

    public class AmThucAdd : AmThuc
    {
        public new Guid? MonAnUongID { get; set; }
    }

    public class AmThucJson
    {
        public string AmThuc { get; set; }
        public string AmThuc_NoiDung { get; set; }
        public IFormFile AnhDaiDien { get; set; }
    }
}
