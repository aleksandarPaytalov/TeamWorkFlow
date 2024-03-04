using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
    internal class PartStatusConfig : IEntityTypeConfiguration<PartStatus>
    {
        public void Configure(EntityTypeBuilder<PartStatus> builder)
        {
            var data = new SeedData();

            builder.HasData(new PartStatus[]
            {
                data.Released,
                data.NotReleased,
                data.ConditionalReleased
            });
        }
    }
}
