using Microsoft.AspNetCore.Identity;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Extensions
{
    public static class ApplicationAdminRoleExtension
    {
        public static async Task CreateAdminRoleAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (await roleManager.RoleExistsAsync(AdminRole) == false)
            {
                var role = new IdentityRole(AdminRole);
                await roleManager.CreateAsync(role);

                var admin = await userManager.FindByEmailAsync(AdminEmail);

                if (admin != null)
                {
                    await userManager.AddToRoleAsync(admin, role.Name);
                }
            }
        }
    }
}
