using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TourDuLich
{
    public class TourDuLich_TheLoai
    {
        public Guid TheLoaiID { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class TourDuLich_TheLoaiDTO : TourDuLich_TheLoai
    { 
        public string? TenTheLoai { get; set; }
        public string? MoTa { get; set; }
        public string? TenLinhVuc { get; set; }
    }
    public class TourDuLich_TheLoaiAdd
    {
        public string TourDuLich_TheLoai { get; set; }
        public string TourDuLich_TheLoai_NoiDung { get; set; }
        public string ListLinhVucID { get; set; }
    }
}
