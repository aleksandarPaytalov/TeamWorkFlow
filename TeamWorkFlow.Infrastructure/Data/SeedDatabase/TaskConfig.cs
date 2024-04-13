using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = TeamWorkFlow.Infrastructure.Data.Models.Task;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class TaskConfig : IEntityTypeConfiguration<Task>
	{
		public void Configure(EntityTypeBuilder<Task> builder)
		{
			var data = new SeedData();

			builder.HasData(new Task[]
			{
				data.TaskOne,
				data.TaskTwo,
				data.TaskThree,
				data.TaskFour,
				data.TaskFive,
				data.TaskSix
			});
		}
	}
}
