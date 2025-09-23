using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class ViPham_VanBan_FileDinhKem
    {
        public Guid FileVanBanID { get; set; }
        public Guid? VanBanID { get; set; }
        public Guid? MaViPhamID { get; set; }
        public string DuongDanFile { get; set; }
    }
}
