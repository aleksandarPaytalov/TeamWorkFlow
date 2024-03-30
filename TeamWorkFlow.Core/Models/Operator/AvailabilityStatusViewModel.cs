using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Operator
{
	public class AvailabilityStatusViewModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(DataConstants.AvailabilityStatusNameMaxLength,
			MinimumLength = DataConstants.AvailabilityStatusNameMinLength,
			ErrorMessage = Messages.StringLength)]
		public string Name { get; set; } = string.Empty;
	}
}
