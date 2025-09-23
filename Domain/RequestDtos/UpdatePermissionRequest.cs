using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RequestDtos
{
    public class UpdatePermissionRequest
    {
        public List<UpdatePermissionRequestItem> Permission {  get; set; } = new List<UpdatePermissionRequestItem>();
    }
}
