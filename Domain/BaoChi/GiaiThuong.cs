using Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class GiaiThuong : BaseEntity
    {
        public Guid GiaiThuongID { get; set; }
        public int? NamTraoGiai { get; set; }
        public int? LinhVucID { get; set; }
        public string? TenGiaiThuong { get; set; }

    }
}
