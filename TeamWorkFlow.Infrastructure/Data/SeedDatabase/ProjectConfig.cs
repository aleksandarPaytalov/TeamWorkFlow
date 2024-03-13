using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class ProjectConfig : IEntityTypeConfiguration<Project>
	{
		public void Configure(EntityTypeBuilder<Project> builder)
		{
			var data = new SeedData();

			builder.HasData(new Project[]
			{
				data.BmwHousingGx9,
				data.VwFrontPanel,
				data.ToyotaClimaticModule
			});
		}
	}
}
