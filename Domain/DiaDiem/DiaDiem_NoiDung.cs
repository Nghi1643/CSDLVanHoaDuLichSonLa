using Domain.BaoChi;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SuKienHoatDong

{
    public class DiaDiem_NoiDung
    {
        public Guid? DiaDiemID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenDiaDiem { get; set; }
        public string MoTa { get; set; }
        public string DiaChi { get; set; }
        public string HeToaDo { get; set; }
        public string ThoiGianHoatDong { get; set; }
        public string SucChua { get; set; }
    }
}
