using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Infrastructure.Data;

namespace TeamWorkFlow.Core.Services
{
	public class PartService : IPartService
	{
		private readonly TeamWorkFlowDbContext _context;

		public PartService(TeamWorkFlowDbContext context)
		{
			_context = context;
		}
	}
}
