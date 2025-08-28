using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Contracts;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Project
{
    public class ProjectFormModel : IProjectModel
	{

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(ProjectNumberMaxLength,
            MinimumLength = ProjectNumberMinLength,
            ErrorMessage = StringLength)]
        public string ProjectNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(ProjectNameMaxLength,
            MinimumLength = ProjectNameMinLength,
            ErrorMessage = StringLength)]
        public string ProjectName { get; set; } = string.Empty;

        public int ProjectStatusId { get; set; }

        public IEnumerable<ProjectStatusServiceModel> ProjectStatuses { get; set; } =
            new List<ProjectStatusServiceModel>();

        [StringLength(ProjectClientNameMaxLength,
            MinimumLength = ProjectClientNameMinLength,
            ErrorMessage = StringLength)]
        public string? ClientName { get; set; }

        [StringLength(ProjectApplianceMaxLength,
            MinimumLength = ProjectApplianceMinLength,
            ErrorMessage = StringLength)]
        public string? Appliance { get; set; }

        // TotalHoursSpent is set automatically to 1 hour on creation (project setup time)
        // This field is not displayed in the form but used internally
        [Range(ProjectTotalHoursMinValue,
	        ProjectTotalHoursMaxValue,
	        ErrorMessage = StringNumberRange)]
        public int TotalHoursSpent { get; set; } = 1;
    }
}
