using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class DiTich_TrungTu
    {
        public Guid ID { get; set; }
        public Guid DiTichID { get; set; }
        public int LanTrungTu { get; set; }
        public string MaNgonNgu { get; set; }
        public string ToChucThucHien { get; set; }
        public string ThoiGian { get; set; }
        public string CongViec { get; set; }
        public string MucDich { get; set; }
    }
}
