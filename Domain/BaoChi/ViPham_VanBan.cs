using Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class ViPham_VanBan : BaseEntity
    {
        public Guid VanBanID { get; set; }
        public Guid? MaViPhamID { get; set; }
        public string SoKyHieu { get; set; }
        public string TrichYeu { get; set; }
        public Guid? CoQuanBanHanhID { get; set; }
        public DateTime? NgayBanHanh { get; set; }
    }
}
