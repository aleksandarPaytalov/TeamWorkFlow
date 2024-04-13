using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamWorkFlow.Infrastructure.Data.SeedDatabase
{
	internal class UserConfig : IEntityTypeConfiguration<IdentityUser>
	{
		public void Configure(EntityTypeBuilder<IdentityUser> builder)
		{
			var data = new SeedData();

			builder.HasData(new IdentityUser[]
			{
				data.GuestUser,
				data.OperatorUser,
				data.AdminUser
			});
		}
	}
}
