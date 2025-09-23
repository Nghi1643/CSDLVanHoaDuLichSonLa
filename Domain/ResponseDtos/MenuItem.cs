using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseDtos
{
    public class MenuItem : CSDL_AdminMenu
    {
        public bool HasChildren { get; set; }
    }
}
