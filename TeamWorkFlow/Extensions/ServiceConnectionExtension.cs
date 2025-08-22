using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;

namespace TeamWorkFlow.Extensions
{
    public static class ServiceConnectionExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<ITaskService, TaskService>();
			services.AddScoped<IOperatorService, OperatorService>();
			services.AddScoped<IMachineService, MachineService>();
			services.AddScoped<IPartService, PartService>();
			services.AddScoped<IProjectService, ProjectService>();
			services.AddScoped<ISummaryService, SummaryService>();
			services.AddScoped<ISprintService, SprintService>();
			services.AddScoped<IUserRoleService, UserRoleService>();
			return services;
		}

		public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
		{
			// Check if running in CI/CD environment with in-memory database
			bool useInMemoryDatabase = Environment.GetEnvironmentVariable("USE_IN_MEMORY_DATABASE") == "true";

			if (useInMemoryDatabase)
			{
				// Use in-memory database for CI/CD
				services.AddDbContext<TeamWorkFlowDbContext>(options =>
					options.UseInMemoryDatabase(databaseName: "TeamWorkFlowInMemoryDb"));
			}
			else
			{
				// Use SQL Server for normal operation
				var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
				services.AddDbContext<TeamWorkFlowDbContext>(options =>
					options.UseSqlServer(connectionString));
			}

            services.AddScoped<IRepository, Repository>();

			services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
		}

		public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration config)
		{
			services.AddDefaultIdentity<IdentityUser>(options =>
				{
					options.User.RequireUniqueEmail = true;
					options.SignIn.RequireConfirmedAccount = false;
					options.Password.RequireDigit = true;
					options.Password.RequireUppercase = true;
					options.Password.RequireNonAlphanumeric = true;
					options.Password.RequireLowercase = true;
					options.Password.RequiredLength = 8;
                })
                .AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<TeamWorkFlowDbContext>();

			return services;
		}
	}
}
