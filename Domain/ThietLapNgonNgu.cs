using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ThietLapNgonNgu
    {
        public int ID { get; set; }
        public string Khoa { get; set; }
        public string GiaTriDich { get; set; }
        public string MoTa { get; set; }
        public string MaNgonNgu { get; set; }
    }
    public class ThietLapNgonNguRequest
    {
        public int? ID { get; set; }
        public string TuKhoa { get; set; }
        public string MaNgonNgu { get; set; }
        public string Khoa { get; set; }
        public string GiaTriDich { get; set; }
        public string MoTa { get; set; }
    }
}
