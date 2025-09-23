using CSDLVanHoaDuLichSonLa.Controllers.API;
using Domain;
using Domain.RequestDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSDLVanHoaDuLichSonLa.Controllers
{
    public class AccountApiController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AccountApiController(IWebHostEnvironment hostingEnvironment, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration config) : base(hostingEnvironment, config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleManager.Roles.Where(r => r.Name != "Host").ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(string keyword="", string macoquan = "", string trangthai = "")
        {
            //var perm = getPermission();
            //if (!perm.IsAdmin)
            //{
            //    macoquan = perm.UniqueCode;
            //}

            var result = await _userManager.Users.Where(o => (o.UserName.Contains(keyword) || o.DisplayName.Contains(keyword)) && (string.IsNullOrEmpty(macoquan) || o.OrgUniqueCode == macoquan) && (string.IsNullOrEmpty(trangthai) || o.LockoutEnabled == (trangthai == "1" ? true : false))).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllUsersRoles")]
        public async Task<IActionResult> GetAllUsersRoles(string keyword = "", string macoquan = "", string trangthai = "")
        {
            var perm = getPermission();
            //if (!perm.IsAdmin)
            //{
            //    macoquan = perm.UniqueCode;
            //}

            //var result = await _userManager.Users.Where(o => (o.UserName.Contains(keyword) || o.DisplayName.Contains(keyword)) && (string.IsNullOrEmpty(macoquan) || o.OrgUniqueCode == macoquan) && (string.IsNullOrEmpty(trangthai) || o.LockoutEnabled == (trangthai == "1" ? true : false))).ToListAsync();
            try
            {
                List<UserViewModel> res = new List<UserViewModel>();
                var result = await _userManager.Users.Where(o => o.UserName != "Host" && ( o.UserName.Contains(keyword) || o.DisplayName.Contains(keyword)) && (string.IsNullOrEmpty(macoquan) || o.OrgUniqueCode == macoquan) && (string.IsNullOrEmpty(trangthai) || o.LockoutEnabled == (trangthai == "1" ? true : false))).ToListAsync();

                if (result != null)
                { 
                    foreach (var user in result)
                    {
                        UserViewModel u = new UserViewModel();
                        var roles = await _userManager.GetRolesAsync(user);
                        u.Roles = string.Join(", ", roles);
                        u.User = user;

                        res.Add(u);
                    }
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet]
        [Route("GetUserDetail/{userId}")]
        public async Task<IActionResult> GetUserDetail(Int64 userId)
        {
            var result = await Mediator.Send(new Application.TaiKhoan.ChiTiet.Query { UserId = userId });
            return HandleResult(result);
        }

        [HttpPost]
        [Route("CreateNewAccount")]
        public async Task<IActionResult> CreateNewAccount([FromBody] CreateAccountRequestDto _request)
        {
            AppUser newUser = new AppUser
            {
                UserName = _request.UserName,
                DisplayName = _request.DisplayName,
                Email = _request.Email,
                OrgUniqueCode = _request.UniqueOrgCode,
                LockoutEnabled = false,
                LockoutEnd = new DateTime(9999,12,31)
            };

            var createResult = await _userManager.CreateAsync(newUser, _request.Password);

            if (!createResult.Succeeded)
            {
                List<string> errors = new List<string>();
                foreach(var error in createResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(". ", errors));
            }
            List<string> lstRoles = _request.Roles.Split(',').ToList();
            await _userManager.AddToRolesAsync(newUser, lstRoles);

            return Ok();
        }

        [HttpPut]
        [Route("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount([FromBody] CreateAccountRequestDto _request)
        {
            AppUser appUser = await _userManager.FindByNameAsync(_request.UserName);

            if (appUser == null)
            {
                return BadRequest($"Không tìm thấy tài khoản có tên đăng nhập là {_request.UserName}");
            }

            appUser.DisplayName = _request.DisplayName;
            appUser.Email = _request.Email;
            appUser.OrgUniqueCode = _request.UniqueOrgCode;
            var updateResult = await _userManager.UpdateAsync(appUser);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult);
            }

            return Ok();
        }

        [HttpPut]
        [Route("UpdateAccountPassword")]
        public async Task<IActionResult> UpdateAccountPassword([FromBody] CreateAccountRequestDto _request)
        {
            AppUser appUser = await _userManager.FindByNameAsync(_request.UserName);

            if (appUser == null)
            {
                return BadRequest($"Không tìm thấy tài khoản có tên đăng nhập là {_request.UserName}");
            }

            var pToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var updateResult = await _userManager.ResetPasswordAsync(appUser, pToken, _request.Password);
            if (!updateResult.Succeeded)
            {
                List<string> errors = new List<string>();
                foreach (var error in updateResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(". ", errors));
            }

            return Ok();
        }

        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto _request)
        {
            AppUser appUser = await _userManager.FindByNameAsync(_request.username);

            if (appUser == null)
            {
                return BadRequest($"Không tìm thấy tài khoản có tên đăng nhập là {_request.username}");
            }
            var updateResult = await _userManager.ChangePasswordAsync(appUser, _request.oldpassword, _request.newpassword);

            if (!updateResult.Succeeded)
            {
                List<string> errors = new List<string>();
                foreach (var error in updateResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(". ", errors));
            }

            return Ok();
        }

        [HttpPut]
        [Route("AddRoleToAccount/{username}/{rolename}")]
        public async Task<IActionResult> AddRoleToAccount(string username, string rolename)
        {
            AppUser appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest($"Không tìm thấy tài khoản có tên đăng nhập là {username}");
            }
            var updateResult = await _userManager.AddToRoleAsync(appUser, rolename);
            if (!updateResult.Succeeded)
            {
                List<string> errors = new List<string>();
                foreach (var error in updateResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(". ", errors));
                //return BadRequest(updateResult);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("RemoveRoleFromAccount/{username}/{rolename}")]
        public async Task<IActionResult> RemoveRoleFromAccount(string username, string rolename)
        {
            AppUser appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest($"Không tìm thấy tài khoản có tên đăng nhập là {username}");
            }
            var updateResult = await _userManager.RemoveFromRoleAsync(appUser, rolename);
            if (!updateResult.Succeeded)
            {
                List<string> errors = new List<string>();
                foreach (var error in updateResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(". ", errors));
                //return BadRequest(updateResult);
            }

            return Ok();
        }

        [HttpPost]
        [Route("DisableUser/{username}")]
        public async Task<IActionResult> DisableUser(string username)
        {
            AppUser appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest($"Không tìm thấy tài khoản có tên đăng nhập là {username}");
            }

            var disableResult =  await _userManager.SetLockoutEnabledAsync(appUser, true);
            List<string> errors = new List<string>();

            if (!disableResult.Succeeded)
            {
                foreach (var error in disableResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(". ", errors));
            }

            return Ok();
        }

        [HttpPost]
        [Route("ActiveUser/{username}")]
        public async Task<IActionResult> ActiveUser(string username)
        {
            AppUser appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest($"Không tìm thấy tài khoản có tên đăng nhập là {username}");
            }

            List<string> errors = new List<string>();

            var disableResult = await _userManager.SetLockoutEnabledAsync(appUser, false);

            if (!disableResult.Succeeded)
            {
                foreach (var error in disableResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(". ", errors));
            }

            return Ok();
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] AppRole _request)
        {
            if (string.IsNullOrWhiteSpace(_request.Name))
            {
                return BadRequest("Tên role không hợp lệ.");
            }

            // Kiểm tra role đã tồn tại chưa
            var roleExists = await _roleManager.RoleExistsAsync(_request.Name);
            if (roleExists)
            {
                return BadRequest($"Role '{_request.Name}' đã tồn tại.");
            }

            // Tạo role mới
            var result = await _roleManager.CreateAsync(new AppRole
            {
                Name = _request.Name,
                RoleDescription = _request.RoleDescription,
                NormalizedName = _request.Name.ToUpper(),
                LinhVucID = _request.LinhVucID
            });

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(string.Join(". ", errors));
            }

            return Ok();
        }

        [HttpPut]
        [Route("UpdateRole/{roleId}")]
        public async Task<IActionResult> UpdateRole(long roleId, [FromBody] AppRole _request)
        {
            if (string.IsNullOrWhiteSpace(_request.Name))
            {
                return BadRequest("Tên role không hợp lệ.");
            }

            // Tìm role theo Id
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound($"Không tìm thấy role với Id = {roleId}");
            }

            // Cập nhật tên role
            role.Name = _request.Name;
            role.RoleDescription = _request.RoleDescription;
            role.NormalizedName = _request.Name.ToUpper();
            role.LinhVucID = _request.LinhVucID;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(string.Join(". ", errors));
            }

            return Ok();
        }

        [HttpDelete]
        [Route("DeleteRole/{roleId}")]
        public async Task<IActionResult> DeleteRole(long roleId)
        {
            // Tìm role theo Id
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound($"Không tìm thấy role với Id = {roleId}");
            }

            // Nếu role là Host thì không cho xoá
            if (role.Name.Equals("Host", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Không được phép xoá role Host.");
            }

            // Nếu user hiện tại là Host thì mới cho xoá (ngược lại chặn)
            var curUser = await _userManager.GetUserAsync(User);
            if (curUser == null)
            {
                return BadRequest("Người dùng chưa đăng nhập.");
            }

            var userRoles = await _userManager.GetRolesAsync(curUser);
            if (!userRoles.Contains("Host"))
            {
                return Forbid("Chỉ Host mới có quyền xoá role.");
            }

            // Kiểm tra xem role này có user nào đang dùng không
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                return BadRequest($"Role '{role.Name}' đang được sử dụng bởi {usersInRole.Count} người dùng, không thể xoá.");
            }

            // Xoá role
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(string.Join(". ", errors));
            }

            return Ok();
        }

    }
}
