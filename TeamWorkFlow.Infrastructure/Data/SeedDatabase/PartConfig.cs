using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class PartConfig : IEntityTypeConfiguration<Part>
	{
		public void Configure(EntityTypeBuilder<Part> builder)
		{
			var data = new SeedData();

			builder.HasData(new Part[]
			{
				data.PartOne,
				data.PartTwo,
				data.PartThree,
				data.PartFour,
				data.PartFive,
				data.PartSix,
				data.PartSeven,
				data.PartEight
			});
		}
	}
}
