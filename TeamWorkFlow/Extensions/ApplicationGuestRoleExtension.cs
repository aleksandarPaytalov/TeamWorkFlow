using Microsoft.AspNetCore.Identity;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Extensions
{
    public static class ApplicationGuestRoleExtension
    {
        public static async Task CreateGuestRoleAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (await roleManager.RoleExistsAsync(GuestRole) == false)
            {
                var role = new IdentityRole(GuestRole);
                await roleManager.CreateAsync(role);
            }
        }
    }
}
