using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.ViewComponents
{
    public class DropdownUserViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var user = (ClaimsIdentity)User.Identity;
            string uname = user.Name;

            return View(user);
        }
    }
}
