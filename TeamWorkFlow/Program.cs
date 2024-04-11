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
				//TaskRouting
				endpoints.MapControllerRoute(
				    name: "Task Details",
				    pattern: "/Task/Details/{id}/{extension}",
				    defaults: new { Controller = "Task", Action = "Details" }
				    );
				endpoints.MapControllerRoute(
				    name: "Task Delete",
				    pattern: "/Task/Delete/{id}/{extension}",
				    defaults: new { Controller = "Task", Action = "Delete" }
				);
				endpoints.MapControllerRoute(
				    name: "Task Edit",
				    pattern: "/Task/Edit/{id}/{extension}",
				    defaults: new { Controller = "Task", Action = "Edit" }
				);

				//OperatorRouting
				endpoints.MapControllerRoute(
					name: "Operator Details",
					pattern: "/Operator/Details/{id}/{extension}",
					defaults: new { Controller = "Operator", Action = "Details" }
				);

                endpoints.MapControllerRoute(
                    name: "Operator Delete",
                    pattern: "/Operator/Delete/{id}/{extension}",
                    defaults: new { Controller = "Operator", Action = "Delete" }
                );

                endpoints.MapControllerRoute(
                    name: "Operator Edit",
                    pattern: "/Operator/Edit/{id}/{extension}",
                    defaults: new { Controller = "Operator", Action = "Edit" }
                );

				//MachineRouting
				endpoints.MapControllerRoute(
					name: "Machine Details",
					pattern: "/Machine/Details/{id}/{extension}",
					defaults: new { Controller = "Machine", Action = "Details" }
				);

				endpoints.MapControllerRoute(
					name: "Machine Delete",
					pattern: "/Machine/Delete/{id}/{extension}",
					defaults: new { Controller = "Machine", Action = "Delete" }
				);

				endpoints.MapControllerRoute(
					name: "Machine Edit",
					pattern: "/Machine/Edit/{id}/{extension}",
					defaults: new { Controller = "Machine", Action = "Edit" }
				);

				endpoints.MapDefaultControllerRoute();
				endpoints.MapRazorPages();
            });
			

            await app.RunAsync();
		}
	}
}