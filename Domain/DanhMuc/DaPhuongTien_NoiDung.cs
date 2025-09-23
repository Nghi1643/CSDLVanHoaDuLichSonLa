using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class DaPhuongTien_NoiDung
    {
        public Guid DaPhuongTienNoiDungID { get; set; }
        public Guid DaPhuongTienID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
    }
}
