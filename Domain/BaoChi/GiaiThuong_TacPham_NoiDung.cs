using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class GiaiThuong_TacPham_NoiDung
    {
        public Guid TacPhamNoiDungID { get; set; }
        public Guid TacPhamID { get; set; }
        public Guid GiaiThuongID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenTacPham { get; set; }
        public string MoTaTacPham { get; set; }
    }
}
