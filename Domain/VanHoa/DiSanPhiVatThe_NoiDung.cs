using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class DiSanPhiVatThe_NoiDung
    {
        public Guid DiSanID { get; set; }
        public string MaNgonNgu { get; set; }
        public string CongDong { get; set; }
        public string MoTa { get; set; }
        public string TinhTrang { get; set; }
        public string TenDiSan { get; set; }
    }
    public class DiSanPhiVatThe_NoiDungAdd
    {
        public Guid? DiSanID { get; set; }
        public string MaNgonNgu { get; set; }
        public string CongDong { get; set; }
        public string MoTa { get; set; }
        public string TinhTrang { get; set; }
        public string TenDiSan { get; set; }
    }
}
