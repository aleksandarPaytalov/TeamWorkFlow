using TeamWorkFlow.Core.Models.Part;

namespace TeamWorkFlow.Core.Models.Project
{
	public class ProjectDetailsServiceModel : ProjectServiceModel
	{
		public string ClientName { get; set; } = string.Empty;

		public string Appliance { get; set; } = string.Empty;

		public int TotalHoursSpent { get; set; }

		public IEnumerable<ProjectPartServiceModel> Parts { get; set; } = new List<ProjectPartServiceModel>();
	}
}
