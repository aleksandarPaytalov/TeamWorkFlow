using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class UserClaimsConfig : IEntityTypeConfiguration<IdentityUserClaim<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
		{
			var data = new SeedData();

			builder.HasData(data.GuestUserClaim, data.OperatorUserClaim, data.AdminUserClaim);
		}
	}
}
