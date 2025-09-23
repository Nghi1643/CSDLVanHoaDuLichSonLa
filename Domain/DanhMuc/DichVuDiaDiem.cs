using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class DichVuDiaDiem
    {
        public Guid DM_DichVuDiaDiemID { get; set; }

        public Guid ThongTinDiaDiemID { get; set; }

        public Guid DoiTuongSoHuuID { get; set; }

        public string LoaiDoiTuong { get; set; }

        public int DichVuID { get; set; }
    }
}
