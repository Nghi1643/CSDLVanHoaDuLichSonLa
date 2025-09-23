using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class BaoChiAnPham_NoiDung
    {
        public Guid BCAPNoiDungID { get; set; }
        public Guid BaoChiAnPhamID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenAnPham { get; set; }
        public string DoiTuongDocGia { get; set; }
        public string LinhVucChuyenSau { get; set; }
    }
}
