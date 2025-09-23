using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using static Dapper.SqlMapper;

namespace CSDLVanHoaDuLichSonLa.Extensions
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser>
    {
        private readonly RoleManager<AppRole> _roleManager;
        public CustomClaimsPrincipalFactory(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            _roleManager = roleManager;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            List<byte?> lstLinhVuc = new List<byte?>();

            var lstRole = await UserManager.GetRolesAsync(user);
            if (lstRole != null && lstRole.Count > 0)
            {
                foreach (string roleName in lstRole)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        if (role.LinhVucID != null)
                        {
                            lstLinhVuc.Add(role.LinhVucID);
                        }
                    }
                }
               
            }
            string strLinhVucList = string.Join(',', lstLinhVuc.Distinct());
            string strRolesList = string.Join(',', lstRole);

            if (principal.Identity != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(
                    new[] { 
                        new Claim("OrgUniqueCode", string.IsNullOrEmpty(user.OrgUniqueCode) ? "" : user.OrgUniqueCode),
                        new Claim("RolesList", strRolesList), 
                        new Claim("DisplayName", user.DisplayName),
                        new Claim("LinhVucList", strLinhVucList),
                    });
            }

            return principal;
        }
    }
}
