using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RequestDtos
{
    public class UpdatePermissionRequestItem
    {
        public int PermissionId {  get; set; }
        public bool permitedApprove { get; set; }
        public bool permitedDelete { get; set; }
        public bool permitedEdit { get; set; }
        public bool permitedCreate { get; set; }
    }
}
