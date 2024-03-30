using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Task
{
	public class PriorityViewModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(DataConstants.PriorityNameMaxLength, 
			MinimumLength = DataConstants.PriorityNameMinLength,
			ErrorMessage = Messages.StringLength)]
		public string Name { get; set; } = string.Empty;
	}
}
