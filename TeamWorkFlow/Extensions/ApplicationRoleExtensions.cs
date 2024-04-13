using Microsoft.AspNetCore.Identity;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Extensions
{
    public static class ApplicationRoleExtensions
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

            if (await roleManager.RoleExistsAsync(OperatorRole) == false)
            {
                var role = new IdentityRole(OperatorRole);
                await roleManager.CreateAsync(role);

                var operatorUser = await userManager.FindByEmailAsync(OperatorEmail);

                if (operatorUser != null)
                {
                    await userManager.AddToRoleAsync(operatorUser, role.Name);
                }

            }
        }
    }
}
