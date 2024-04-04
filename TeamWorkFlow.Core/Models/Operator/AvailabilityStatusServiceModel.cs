using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Operator
{
	public class AvailabilityStatusServiceModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = RequiredMessage)]
		[StringLength(AvailabilityStatusNameMaxLength,
			MinimumLength = AvailabilityStatusNameMinLength,
			ErrorMessage = StringLength)]
		public string Name { get; set; } = string.Empty;
	}
}
