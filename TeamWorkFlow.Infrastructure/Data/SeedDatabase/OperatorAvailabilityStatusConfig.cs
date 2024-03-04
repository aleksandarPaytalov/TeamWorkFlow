using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
    internal class OperatorAvailabilityStatusConfig : IEntityTypeConfiguration<OperatorAvailabilityStatus>
    {
        public void Configure(EntityTypeBuilder<OperatorAvailabilityStatus> builder)
        {
            var data = new SeedData();

            builder.HasData(new OperatorAvailabilityStatus[]
            {
                data.AtWorkStatus,
                data.InSickLeaveStatus,
                data.OnVacation,
                data.OnTraining
            });
        }
    }
}
