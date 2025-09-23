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
    [Table("DM_LinhVuc")]
    public class DM_LinhVuc
    {
        [Key]
        public short LinhVucID { get; set; }
        public short? LinhVucCapChaID { get; set; }
        public short? ThuTu { get; set; }
        public bool? TrangThai { get; set; }
        public string Ten { get; set; }
    }
}
