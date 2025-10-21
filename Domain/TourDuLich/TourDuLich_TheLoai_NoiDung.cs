using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TourDuLich
{
    public class TourDuLich_TheLoai_NoiDung
    {
        public Guid TheLoaiID { get; set; }
        public string TenTheLoai { get; set; } = string.Empty;
        public string? MoTa { get; set; }
    }
}
