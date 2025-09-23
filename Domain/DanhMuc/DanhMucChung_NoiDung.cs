using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class DanhMucChung_NoiDung
    {
        public int ID { get; set; }
        public int? DanhMucID { get; set; }
        public string MaNgonNgu { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
    }
}
