using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Operator
{
	public class OperatorServicesModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = Messages.RequiredMessage)]
		[StringLength(DataConstants.OperatorFullNameMaxLength,
			MinimumLength = DataConstants.OperatorFullNameMinLength,
			ErrorMessage = Messages.StringLength)]
		public string FullName { get; set; } = string.Empty;
		
		[Required(ErrorMessage = Messages.RequiredMessage)]
		[EmailAddress]
		[StringLength(DataConstants.OperatorEmailMaxLength,
			MinimumLength = DataConstants.OperatorEmailMinLength,
			ErrorMessage = Messages.StringLength)]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = Messages.RequiredMessage)]
		[StringLength(DataConstants.OperatorPhoneMaxLength,
			MinimumLength = DataConstants.OperatorPhoneMinLength,
			ErrorMessage = Messages.StringLength)]
		[Comment("Operator phoneNumber")]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = Messages.RequiredMessage)]
		public string IsActive { get; set; } = string.Empty;

		[Required(ErrorMessage = Messages.RequiredMessage)]
		[Range(DataConstants.OperatorMinCapacity, DataConstants.OperatorMaxCapacity)]
		public int Capacity { get; set; }

		[Required(ErrorMessage = Messages.RequiredMessage)]
		public int AvailabilityStatusId { get; set; }

		public ICollection<AvailabilityStatusViewModel> AvailabilityStatusModels { get; set; } =
			new List<AvailabilityStatusViewModel>();
	}
}
