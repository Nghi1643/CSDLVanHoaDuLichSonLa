using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseDtos
{
    public class ChiTietTaiKhoan
    {
        public Int64 Id {  get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string OrgUniqueCode { get; set; }
        public string Roles { get; set; }
    }
}
