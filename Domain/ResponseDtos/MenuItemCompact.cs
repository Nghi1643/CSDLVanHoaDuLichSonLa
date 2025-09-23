using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseDtos
{
    public class MenuItemCompact
    {
        public int Id { get; set; }
        public int? PermissionType { get; set; }
        public int? ParentId { get; set; }
        public string Title {  get; set; }
        public bool IsLeaf {  get; set; }
        public bool IsPermission { get; set; } = false;
        public bool? PermitedEdit { get; set; }
        public bool? PermitedCreate { get; set; }
        public bool? PermitedDelete { get; set; }
        public bool? PermitedApprove { get; set; }
        public int? PermissionId { get; set; }
        public bool HasPermission { get; set; } = false;
    }
}
