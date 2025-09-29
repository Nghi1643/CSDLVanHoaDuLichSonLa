using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class AmThuc_NoiDung
    {
        public Guid MonAnUongID { get; set; }
        public string MaNgonNgu { get; set; }
        public string MoTa { get; set; }
        public string NguyenLieu { get; set; }
        public string TraiNghiem { get; set; }
        public string TenMon { get; set; }
    }
    public class AmThuc_NoiDungAdd
    {
        public Guid? MonAnUongID { get; set; }
        public string MaNgonNgu { get; set; }
        public string MoTa { get; set; }
        public string NguyenLieu { get; set; }
        public string TraiNghiem { get; set; }
        public string TenMon { get; set; }
    }
}
