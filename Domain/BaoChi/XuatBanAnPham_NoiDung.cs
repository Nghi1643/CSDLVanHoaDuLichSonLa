using Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaoChi
{
    public class XuatBanAnPham_NoiDung : BaseEntity
    {
        public Guid? XBAPNoiDungID { get; set; }
        public Guid? XuatBanAnPhamID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenAnPham { get; set; }
        public string TacGia { get; set; }
        public string MoTa { get; set; }
    }
}
