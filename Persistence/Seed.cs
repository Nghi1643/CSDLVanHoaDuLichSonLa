using Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                var roleList = new List<AppRole>
                {
                    new AppRole
                    {
                        Name = "Host",
                        RoleDescription = "Quản trị toàn cục"
                    },
                    new AppRole
                    {
                        Name = "Admin",
                        RoleDescription = "Quản trị cục bộ"
                    }
                };

                foreach (var role in roleList)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        DisplayName = "Admin",
                        UserName = "admin",
                        Email = "admin@email.com",
                        LockoutEnabled = false,
                        LockoutEnd = new DateTime(9999,12,31)
                    },
                    //new AppUser
                    //{
                    //    DisplayName = "Host",
                    //    UserName = "host",
                    //    Email = "host@email.com",
                    //    LockoutEnabled = false,
                    //    LockoutEnd = new DateTime(9999,12,31)
                    //}
                };

                foreach(var user in users)
                {
                    await userManager.CreateAsync(user, "Abc@12345");
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
