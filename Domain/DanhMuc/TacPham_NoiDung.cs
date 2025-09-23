using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class TacPham_NoiDung
    {
        public Guid TPNoiDungID { get; set; }
        public Guid TacPhamID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenTacPham { get; set; }
        public string TacGia { get; set; }
        public string MoTa { get; set; }
        public string NoiDung { get; set; }
    }
    public class TacPham_NoiDungAdd : TacPham_NoiDung
    {
        public new Guid? TPNoiDungID { get; set; }
    }
}
