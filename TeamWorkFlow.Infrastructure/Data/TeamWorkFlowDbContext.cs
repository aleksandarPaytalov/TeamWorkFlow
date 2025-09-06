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
            builder.ApplyConfiguration(new PartConfig());
            builder.ApplyConfiguration(new OperatorConfig());
            builder.ApplyConfiguration(new TaskConfig());
            builder.ApplyConfiguration(new TaskOperatorConfig());

            builder.Entity<TaskOperator>()
                .HasKey(e => new
                {
                    e.OperatorId,
                    e.TaskId
                });

            builder.Entity<Operator>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure AdminDemotionRequest foreign key relationships to avoid cascade conflicts
            builder.Entity<AdminDemotionRequest>()
                .HasOne(r => r.TargetUser)
                .WithMany()
                .HasForeignKey(r => r.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AdminDemotionRequest>()
                .HasOne(r => r.RequestedByUser)
                .WithMany()
                .HasForeignKey(r => r.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AdminDemotionRequest>()
                .HasOne(r => r.ApprovedByUser)
                .WithMany()
                .HasForeignKey(r => r.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure TaskTimeEntry relationships
            builder.Entity<TaskTimeEntry>()
                .HasOne(tte => tte.Task)
                .WithMany()
                .HasForeignKey(tte => tte.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskTimeEntry>()
                .HasOne(tte => tte.Operator)
                .WithMany()
                .HasForeignKey(tte => tte.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure TaskTimeSession relationships
            builder.Entity<TaskTimeSession>()
                .HasOne(tts => tts.Task)
                .WithMany()
                .HasForeignKey(tts => tts.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskTimeSession>()
                .HasOne(tts => tts.Operator)
                .WithMany()
                .HasForeignKey(tts => tts.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }

        public DbSet<AdminDemotionRequest> AdminDemotionRequests { get; set; } = null!;
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
        public DbSet<TaskTimeEntry> TaskTimeEntries { get; set; } = null!;
        public DbSet<TaskTimeSession> TaskTimeSessions { get; set; } = null!;
    }
}
