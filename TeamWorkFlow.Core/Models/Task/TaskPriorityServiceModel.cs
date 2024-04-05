using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Task
{
	public class TaskPriorityServiceModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(PriorityNameMaxLength, 
			MinimumLength = PriorityNameMinLength,
			ErrorMessage = StringLength)]
		public string Name { get; set; } = string.Empty;
	}
}
