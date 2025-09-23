using Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class GiaiThuong_TacPham : BaseEntity
    {
        public Guid TacPhamID { get; set; }
        public Guid GiaiThuongID { get; set; }
        public string TenTacPham { get; set; }
    }
}
