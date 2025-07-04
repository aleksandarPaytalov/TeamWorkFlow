using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Pager;

namespace TeamWorkFlow.Core.Models.Project
{
	public class ProjectServiceModel : IProjectModel
	{
		public int Id { get; set; }

		public string ProjectName { get; set; } = string.Empty;

		public string ProjectNumber { get; set; } = string.Empty;

		public string Status { get; set; } = string.Empty;

		public int TotalParts { get; set; }
	}

	public class PaginatedProjectsViewModel
	{
		public IEnumerable<ProjectServiceModel> Projects { get; set; } = new List<ProjectServiceModel>();
		public PagerServiceModel Pager { get; set; }
	}
}
