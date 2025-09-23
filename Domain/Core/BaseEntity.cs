using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class BaseEntity
    {
        public bool SuDung { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int? NguoiHieuChinh { get; set; }
    }

    public class BaseEntityNoStatus
    {
        public DateTime NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public DateTime? NgayHieuChinh { get; set; }
        public int? NguoiHieuChinh { get; set; }
    }
}
