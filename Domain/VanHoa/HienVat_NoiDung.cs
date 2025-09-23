using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.VanHoa
{
    public class HienVat_NoiDung
    {

        public Guid HienVatID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenHienVat { get; set; }
        public string TenGoiKhac { get; set; }
        public string SoLuong { get; set; }
        public string MauSac { get; set; }
        public string KichThuoc { get; set; }
        public string NienDai { get; set; }
        public string NguonGoc { get; set; }
        public string KhaoTa { get; set; }
        public string TinhTrangSuuTam { get; set; }
        public string HoanCanhSuuTam { get; set; }
        public string ChuHienVat { get; set; }
        public string NguoiSuuTam { get; set; }
    }

    public class HienVat_NoiDungAdd : HienVat_NoiDung
    {
        public new Guid? HienVatID { get; set; }
    }
}
