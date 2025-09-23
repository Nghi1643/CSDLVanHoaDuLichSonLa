using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class DiTich_NoiDung
    {
        public Guid DiTichNoiDungID { get; set; }

        public Guid DiTichID { get; set; }

        public string MaNgonNgu { get; set; }

        public string TenDiTich { get; set; }

        public string TenGoiKhac { get; set; }

        public string NienDai { get; set; }

        public string MoTaDiTich { get; set; }
    }
}
