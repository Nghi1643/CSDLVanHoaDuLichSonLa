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
    public class DiaDiem_noiDung
    {
        public Guid SuKienNoiDungID { get; set; }
        public Guid SuKienID { get; set; }
        public string MaNgonNgu { get; set; }
        public string TenSuKien { get; set; }
        public string MoTa { get; set; }
        public string NoiDung { get; set; }
    }
}
