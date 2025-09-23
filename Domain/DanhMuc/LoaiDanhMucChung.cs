using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class LoaiDanhMucChung
    {
        public short LoaiDanhMucID { get; set; }
        public short? LoaiDanhMucCapChaID { get; set; }
        public short? ThuTu { get; set; }
        public bool? TrangThai { get; set; }
        public string Ten { get; set; }
    }
}
