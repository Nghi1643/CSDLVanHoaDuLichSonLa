using System.Security.Claims;
using CSDLVanHoaDuLichSonLa.Models;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CSDLVanHoaDuLichSonLa.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        //private readonly IOptions<AdminNavModel> _adminNav;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager /*IOptions<AdminNavModel> adminNav*/)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            //_adminNav = adminNav;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Hệ thống quản lý cơ sở dữ liệu Văn hóa, Thể thao và Du lịch tỉnh Sơn La";
            var curUser = (ClaimsIdentity)User.Identity;
            var user = curUser != null && curUser.Name != null ? await _userManager.FindByNameAsync(curUser.Name) : null;
            ViewBag.UserLogin = user;
            return View();
        }

        public async Task<IActionResult> Landing()
        {
            ViewData["Title"] = "Hệ thống quản lý cơ sở dữ liệu Văn hóa, Thể thao và Du lịch tỉnh Sơn La";
            var curUser = (ClaimsIdentity)User.Identity;
            var user = curUser != null && curUser.Name != null ? await _userManager.FindByNameAsync(curUser.Name) : null;
            ViewBag.UserLogin = user;
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            var shjt = User.Identity.IsAuthenticated;
            var curUser = (ClaimsIdentity)User.Identity;
            var dkm = curUser.Name;
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            var mUser = await _userManager.GetUserAsync(currentUser);
            var abc = await _userManager.FindByNameAsync(dkm);
            var roles = await _userManager.GetRolesAsync(mUser);
            //mUser.DisplayName;
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Login()
        {
            //await _signInManager.SignOutAsync();
            var curUser = (ClaimsIdentity)User.Identity;
            var user = curUser != null && curUser.Name != null ? await _userManager.FindByNameAsync(curUser.Name) : null;
            if (user == null)
            {
                return View();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            //login functionality
            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user != null)
            {
                //sign in
                var signInResult = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

                if (signInResult.IsLockedOut)
                {
                    ViewData["ErrorMessage"] = "Tài khoản đã bị khóa, xin vui lòng liên hệ quản trị viên để mở khóa tài khoản!";
                    return View();
                }

                if (!signInResult.Succeeded)
                {
                    ViewData["ErrorMessage"] = "Tên tài khoản hoặc mật khẩu không chính xác, vui lòng thử lại!";
                    return View();
                }

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home", new { area = "AdminTool" });
                    //return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
