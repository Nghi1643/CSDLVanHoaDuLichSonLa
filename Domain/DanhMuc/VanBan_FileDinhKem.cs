using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class VanBan_FileDinhKem
    {
        public Guid FileVanBanID { get; set; }
        public Guid VanBanID { get; set; }
        public string DuongDanFile { get; set; }
    }
}
