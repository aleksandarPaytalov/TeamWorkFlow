using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Operator
{
	public class OperatorFormModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = RequiredMessage)]
		[StringLength(OperatorFullNameMaxLength,
			MinimumLength = OperatorFullNameMinLength,
			ErrorMessage = StringLength)]
		public string FullName { get; set; } = string.Empty;
		
		[Required(ErrorMessage = RequiredMessage)]
		[EmailAddress]
		[StringLength(OperatorEmailMaxLength,
			MinimumLength = OperatorEmailMinLength,
			ErrorMessage = StringLength)]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = RequiredMessage)]
		[StringLength(OperatorPhoneMaxLength,
			MinimumLength = OperatorPhoneMinLength,
			ErrorMessage = StringLength)]
		[Phone]
		[Comment("Operator phoneNumber")]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredMessage)]
		public string IsActive { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredMessage)]
		[Range(OperatorMinCapacity, OperatorMaxCapacity)]
		public int Capacity { get; set; }

		[Required(ErrorMessage = RequiredMessage)]
		public int AvailabilityStatusId { get; set; }

		public ICollection<AvailabilityStatusServiceModel> AvailabilityStatusModels { get; set; } =
			new List<AvailabilityStatusServiceModel>();
	}
}
