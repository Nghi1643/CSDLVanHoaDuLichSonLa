using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VH_LeHoiModel
{
    public class VH_LeHoi_NoiDung
    {
        public Guid LeHoiNoiDungID { get; set; }
        public Guid LeHoiID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenLeHoi { get; set; }
        public string NhanVatPhungTho { get; set; }
        public string NoiDung { get; set; }
        public string QuiDinh { get; set; }
        public string DanhGia { get; set; }
        public string DonViPhoiHop { get; set; }
        public string DiaDiemChiTiet { get; set; }
    }

    public class VH_LeHoi_NoiDung_Request
    {
        public Guid LeHoiID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenLeHoiChild { get; set; }
        public string NhanVatPhungTho { get; set; }
        public string NoiDung { get; set; }
        public string QuiDinh { get; set; }
        public string DanhGia { get; set; }
        public string DonViPhoiHop { get; set; }
        public string DiaDiemChiTiet { get; set; }
    }
}
