using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Project
{
	public class ProjectStatusServiceModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = Messages.RequiredMessage)]
		[StringLength(DataConstants.ProjectStatusNameMaxLength,
			MinimumLength = DataConstants.ProjectStatusNameMinLength,
			ErrorMessage = Messages.StringLength)]
		public string Name { get; set; } = string.Empty;
	}
}
