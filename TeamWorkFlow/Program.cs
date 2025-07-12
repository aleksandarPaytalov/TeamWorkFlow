using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Extensions;

namespace TeamWorkFlow
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddApplicationDbContext(builder.Configuration);
			builder.Services.AddApplicationIdentity(builder.Configuration);
			
			builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });

			builder.Services.AddApplicationServices();
			//builder.Services.AddMemoryCache();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error/500");
				app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
				// Explicit routes to ensure proper authentication
				endpoints.MapControllerRoute(
					name: "TaskList",
					pattern: "/Task",
					defaults: new { Controller = "Task", Action = "All" }
				);

				endpoints.MapControllerRoute(
					name: "TaskCreate",
					pattern: "/Task/Create",
					defaults: new { Controller = "Task", Action = "Create" }
				);

				endpoints.MapControllerRoute(
					name: "ProjectList",
					pattern: "/Project",
					defaults: new { Controller = "Project", Action = "All" }
				);

				endpoints.MapControllerRoute(
					name: "OperatorList",
					pattern: "/Operator",
					defaults: new { Controller = "Operator", Action = "All", Area = "" }
				);

				//TaskRouting
				endpoints.MapEntityControllerRoutes("Task");

				//OperatorRouting
				endpoints.MapEntityControllerRoutes("Operator");

				//MachineRouting
				endpoints.MapEntityControllerRoutes("Machine");

				//ProjectRouting
				endpoints.MapEntityControllerRoutes("Project");

				//PartRouting
				endpoints.MapEntityControllerRoutes("Part");

				endpoints.MapControllerRoute(
					name: "areas",
					pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				);

				endpoints.MapDefaultControllerRoute();
				endpoints.MapRazorPages();
            });

            await app.CreateAdminRoleAsync();
            await app.CreateOperatorRoleAsync();

            await app.RunAsync();
		}
	}
}
