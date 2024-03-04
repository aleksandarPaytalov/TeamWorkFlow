using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
    internal class PriorityConfig : IEntityTypeConfiguration<Priority>
    {
        public void Configure(EntityTypeBuilder<Priority> builder)
        {
            var data = new SeedData();

            builder.HasData(new Priority[]
            {
                data.Low,
                data.Normal,
                data.High
            });
        }
    }
}
