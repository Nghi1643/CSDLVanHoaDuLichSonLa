using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DaoTaoBoiDuong
{
    public class DaoTaoBoiDuong_NoiDung
    {
        public Guid DaoTaoID { get; set; }
        public string MaNgonNgu { get; set; }
        public string NoiDungDaoTao { get; set; }
        public string DonVi { get; set; } = string.Empty;
    }
}
