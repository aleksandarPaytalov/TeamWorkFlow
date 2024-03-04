using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
    internal class ProjectStatusConfig : IEntityTypeConfiguration<ProjectStatus>
    {
        public void Configure(EntityTypeBuilder<ProjectStatus> builder)
        {
            var data = new SeedData();

            builder.HasData(new ProjectStatus[]
            {
                data.InProduction,
                data.InDevelopment,
                data.InAcl
            });
        }
    }
}
