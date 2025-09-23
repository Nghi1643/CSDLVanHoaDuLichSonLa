using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ToChuc
{

    public class ToChuc_NoiDung
    {
        public int ID { get; set; }
        public Guid? ToChucID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenToChuc { get; set; }
        public string DiaChi { get; set; }
        public string GioiThieu { get; set; }
        public string GhiChu { get; set; }
    }
   

}
