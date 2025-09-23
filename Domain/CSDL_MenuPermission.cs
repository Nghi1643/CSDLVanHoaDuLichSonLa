using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CSDL_MenuPermission
    {
        public int Id { get; set; }
        public string Rolename {  get; set; }
        public int MenuId { get; set; }
        public bool PermitedEdit { get; set; }
        public bool PermitedDelete { get; set; }
        public bool PermitedApprove { get; set; }
        public bool PermitedCreate { get; set; }
    }
}
