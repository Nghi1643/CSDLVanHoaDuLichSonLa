using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseDtos
{
    public class MenuItemWithRoles : CSDL_AdminMenu
    {
        public string ListRole {  get; set; }
    }
}
