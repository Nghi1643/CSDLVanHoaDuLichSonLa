using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseDtos
{
    public class PermissionDto
    {
        public bool? PermitedEdit { get; set; }
        public bool? PermitedView { get; set; }
        public bool? PermitedDelete { get; set; }
        public bool? PermitedApprove { get; set; }
        public bool? PermitedCreate {  get; set; }
    }
}
