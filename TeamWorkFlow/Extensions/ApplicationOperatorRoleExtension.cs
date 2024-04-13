using Microsoft.AspNetCore.Identity;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Extensions
{
    public static class ApplicationOperatorRoleExtension
    {
        public static async Task CreateOperatorRoleAsync(this IApplicationBuilder app)
        {
            List<string> operatorEmails = OperatorEmails.ToList();

            using var scope = app.ApplicationServices.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (await roleManager.RoleExistsAsync(OperatorRole) == false)
            {
                var role = new IdentityRole(OperatorRole);
                await roleManager.CreateAsync(role);
            }

            foreach (var email in operatorEmails)
            {
                var operatorUser = await userManager.FindByEmailAsync(email);

                if (operatorUser != null)
                {
                    bool isInRole = await userManager.IsInRoleAsync(operatorUser, OperatorRole);

                    if (!isInRole)
                    {
                        await userManager.AddToRoleAsync(operatorUser, OperatorRole);
                    }
                }
            }
        }
    }
}
