using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TourDuLich
{
    public class TourDuLich_NoiDung
    {
        public Guid? TourID { get; set; }
        public string MaNgonNgu { get; set; }= string.Empty;
        public string TenTour { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string ThoiGian { get; set; } = string.Empty;
        public string TraiNghiem { get; set; }= string.Empty;
        public string GoiY { get; set; }= string.Empty;
    }
}
