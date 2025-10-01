using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoSoKinhDoanhDichVuDuLich
{
    public class CoSoAnUong_NoiDung
    {
        public Guid CoSoAnUongID { get; set; }

        public string MaNgonNgu { get; set; }

        public string TenCoSo { get; set; }

        public string MoTa { get; set; } = string.Empty;

        public string DiaChi { get; set; } = string.Empty;

        public string NguoiDaiDien { get; set; } = string.Empty;
    }
}
