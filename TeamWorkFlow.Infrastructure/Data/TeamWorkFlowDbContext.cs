using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Infrastructure.Data.Models;
using TeamWorkFlow.Infrastructure.Data.SeedDatabase;
using Machine = TeamWorkFlow.Infrastructure.Data.Models.Machine;
using Task = TeamWorkFlow.Infrastructure.Data.Models.Task;
using TaskStatus = TeamWorkFlow.Infrastructure.Data.Models.TaskStatus;

namespace TeamWorkFlow.Infrastructure.Data
{
    public class TeamWorkFlowDbContext : IdentityDbContext
	{
		public TeamWorkFlowDbContext(DbContextOptions<TeamWorkFlowDbContext> options)
			: base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new OperatorAvailabilityStatusConfig());
            builder.ApplyConfiguration(new PartStatusConfig());
            builder.ApplyConfiguration(new PriorityConfig());
            builder.ApplyConfiguration(new ProjectStatusConfig());
            builder.ApplyConfiguration(new TaskStatusConfig());
            builder.ApplyConfiguration(new MachineConfig());
            builder.ApplyConfiguration(new ProjectConfig());
            builder.ApplyConfiguration(new UserConfig());
            builder.ApplyConfiguration(new UserClaimsConfig());

            builder.Entity<TaskOperator>()
                .HasKey(e => new
                {
                    e.OperatorId,
                    e.TaskId
                });

            base.OnModelCreating(builder);
        }

        public DbSet<Machine> Machines { get; set; } = null!;
        public DbSet<Operator> Operators { get; set; } = null!;
        public DbSet<OperatorAvailabilityStatus> OperatorAvailabilityStatusEnumerable { get; set; } = null!;
        public DbSet<Part> Parts { get; set; } = null!;
        public DbSet<PartStatus> PartStatusEnumerable { get; set; } = null!;
        public DbSet<Priority> Priorities { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectStatus> ProjectStatusEnumerable { get; set; } = null!;
        public DbSet<Task> Tasks { get; set; } = null!;
        public DbSet<TaskOperator> TasksOperators { get; set; } = null!;
        public DbSet<TaskStatus> TaskStatusEnumerable { get; set; } = null!;
    }
}