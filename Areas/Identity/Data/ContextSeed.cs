#nullable disable
using Dhicoin.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;


namespace Dhicoin.Areas.Identity.Data
{
    public static class ContextSeed
    {
        public static async Task seedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] RoleName = { "Admin", "Employee", "User" };

            foreach(var role in RoleName)
            {
                // check if the role already exicts
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultAdmin = new ApplicationUser
            {
                UserName="admin123@gmail.com",
                Email= "admin123@gmail.com",
                EmailConfirmed=true,
                
            };
            if (userManager.Users.All(u => u.Id != defaultAdmin.Id))
            {
                var user = await userManager.CreateAsync(defaultAdmin, "Pakistan123@");

                if (user.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, "Admin");
                    

                }
            }

        }

        public static class Enums
        {
            public enum Roles
            {
                
                Admin,
                Employee,
                User
            }
        }
    }
}
