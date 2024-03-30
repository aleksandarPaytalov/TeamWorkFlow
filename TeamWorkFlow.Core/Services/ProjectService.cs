using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Infrastructure.Data;

namespace TeamWorkFlow.Core.Services
{
	public class ProjectService : IProjectService
	{
		private readonly TeamWorkFlowDbContext _context;

		public ProjectService (TeamWorkFlowDbContext context)
		{
			_context = context;
		}
	}
}
