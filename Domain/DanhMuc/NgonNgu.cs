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
    public class NgonNgu
    {
        public Guid NgonNguID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenNgonNgu { get; set; }
        public byte ThuTu { get; set; }
        public bool TrangThai { get; set; }
    }
    public class NgonNguRequest
    {
        public Guid? NgonNguID { get; set; }
        public bool? TrangThai { get; set; }
        public string TuKhoa { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenNgonNgu { get; set; }
        public byte ThuTu { get; set; }
    }
}
