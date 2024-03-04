using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskStatus = TeamWorkFlow.Infrastructure.Data.Models.TaskStatus;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
    internal class TaskStatusConfig : IEntityTypeConfiguration<TaskStatus>
    {
        public void Configure(EntityTypeBuilder<TaskStatus> builder)
        {
            var data = new SeedData();

            builder.HasData(new TaskStatus[]
            {
                data.Open,
                data.InProgress,
                data.Finished,
                data.Canceled
            });
        }
    }
}
