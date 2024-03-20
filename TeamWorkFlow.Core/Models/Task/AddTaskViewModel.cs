using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Core.Models.Task
{
    public class AddTaskViewModel
    {
		public int Id { get; set; }

		[Required]
        public int ProjectId { get; set; }

        [Required]
		[StringLength(DataConstants.ProjectNumberMaxLength,
			MinimumLength = DataConstants.ProjectNumberMinLength,
			ErrorMessage = Messages.StringLength)]
	    public string ProjectNumber { get; set; } = string.Empty;

		[Required]
		[StringLength(DataConstants.TaskNameMaxLength,
			MinimumLength = DataConstants.TaskNameMinLength,
			ErrorMessage = Messages.StringLength)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[StringLength(DataConstants.TaskDescriptionMaxLength,
			MinimumLength = DataConstants.TaskDescriptionMinLength,
			ErrorMessage = Messages.StringLength)]
		public string Description { get; set; } = string.Empty;
		
	    public string? Deadline { get; set; }

		[Required]
	    public string StartDate { get; set; } = string.Empty;

	    public string? EndDate { get; set; }

	    public int PriorityId { get; set; }
	    public ICollection<PriorityViewModel> Priorities { get; set; } = new List<PriorityViewModel>();

	    public int StatusId { get; set; }
	    public ICollection<StatusViewModel> Statuses { get; set; } = new List<StatusViewModel>();
	}
}
