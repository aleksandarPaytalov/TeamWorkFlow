using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class MachineConfig : IEntityTypeConfiguration<Machine>
	{
		public void Configure(EntityTypeBuilder<Machine> builder)
		{
			var data = new SeedData();

			builder.HasData(new Machine[]
			{
				data.ZeissConturaOne,
				data.ZeissInspect,
				data.ZeissMetrotom
			});
		}
	}
}
