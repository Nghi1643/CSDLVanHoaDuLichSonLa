using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CSDL_AdminMenu
    {
        public int Id {  get; set; }
        public int? ParentId {  get; set; }
        public string AreaName {  get; set; }
        public string ControllerName {  get; set; }
        public string ActionName {  get; set; }
        public string Title {  get; set; }
        public bool IsLeaf {  get; set; }
        public bool IsShow {  get; set; }
        public string Icon {  get; set; }
        public int DisplayOrder {  get; set; }
    }
}
