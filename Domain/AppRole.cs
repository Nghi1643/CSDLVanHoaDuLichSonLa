using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AppRole : IdentityRole<Int64>
    {
        public string RoleDescription { get; set; }
        public byte? LinhVucID { get; set; }
    }
}
