using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Task
{
	public class AddTaskFormModel
    {
		public int Id { get; set; }

		[Required]
        public int ProjectId { get; set; }

        [Required]
		[StringLength(ProjectNumberMaxLength,
			MinimumLength = ProjectNumberMinLength,
			ErrorMessage = StringLength)]
	    public string ProjectNumber { get; set; } = string.Empty;

		[Required]
		[StringLength(TaskNameMaxLength,
			MinimumLength = TaskNameMinLength,
			ErrorMessage = StringLength)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[StringLength(TaskDescriptionMaxLength,
			MinimumLength = TaskDescriptionMinLength,
			ErrorMessage = StringLength)]
		public string Description { get; set; } = string.Empty;
		
	    public string? Deadline { get; set; }

		[Required]
	    public string StartDate { get; set; } = string.Empty;

	    public string? EndDate { get; set; }

	    public int PriorityId { get; set; }
	    public ICollection<TaskPriorityServiceModel> Priorities { get; set; } = new List<TaskPriorityServiceModel>();

	    public int StatusId { get; set; }
	    public ICollection<TaskStatusServiceModel> Statuses { get; set; } = new List<TaskStatusServiceModel>();
	}
}
