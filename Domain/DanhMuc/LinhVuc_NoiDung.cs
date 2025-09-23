using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DanhMuc
{
    public class LinhVuc_NoiDung
    {
        public short ID { get; set; }
        public short? LinhVucID { get; set; }
        public string MaNgonNgu { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
    }
}
