using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	public class TaskOperatorConfig : IEntityTypeConfiguration<TaskOperator>
	{
		public void Configure(EntityTypeBuilder<TaskOperator> builder)
		{
			var data = new SeedData();

			builder.HasData(new TaskOperator[]
			{
				data.TaskOperatorOne,
				data.TaskOperatorTwo,
				data.TaskOperatorThree,
				data.TaskOperatorFour,
				data.TaskOperatorFive
			});
		}
	}
}
