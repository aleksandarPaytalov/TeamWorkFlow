using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TeamWorkFlow.Infrastructure.Data
{
	public class TeamWorkFlowDbContext : IdentityDbContext
	{
		public TeamWorkFlowDbContext(DbContextOptions<TeamWorkFlowDbContext> options)
			: base(options)
		{
		}
	}
}