using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class ViPham_NoiDung
    {
        public Guid MaViPhamNoiDungID { get; set; }
        public Guid MaViPhamID { get; set; }
        public string MaNgonNgu { get; set; }
        public string NoiDungViPham { get; set; }
        public string MoTaViPham { get; set; }
    }
}
