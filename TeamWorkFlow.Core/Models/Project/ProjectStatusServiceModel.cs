using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Project
{
    public class ProjectStatusServiceModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = RequiredMessage)]
		[StringLength(ProjectStatusNameMaxLength,
			MinimumLength = ProjectStatusNameMinLength,
			ErrorMessage = StringLength)]
		public string Name { get; set; } = string.Empty;
	}
}
