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
			return services;
		}

		public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
		{
			var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			services.AddDbContext<TeamWorkFlowDbContext>(options =>
				options.UseSqlServer(connectionString));

            services.AddScoped<IRepository, Repository>();

			services.AddDatabaseDeveloperPageExceptionFilter();
			
            return services;
		}

		public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration config)
		{
			services.AddDefaultIdentity<IdentityUser>(options =>
				{
					options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                })
				.AddEntityFrameworkStores<TeamWorkFlowDbContext>();

			return services;
		}
	}
}
