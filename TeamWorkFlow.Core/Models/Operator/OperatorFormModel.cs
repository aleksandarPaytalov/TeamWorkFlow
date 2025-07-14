using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Contracts;
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


		[StringLength(OperatorUserIdMaxLength,
			MinimumLength = OperatorUserIdMinLength,
			ErrorMessage = StringLength)]
		public string? UserId { get; set; }

		public ICollection<AvailabilityStatusServiceModel> AvailabilityStatusModels { get; set; } =
			new List<AvailabilityStatusServiceModel>();

		/// <summary>
		/// Gets the boolean value of IsActive for database operations
		/// </summary>
		public bool GetIsActiveBool()
		{
			return bool.TryParse(IsActive, out bool result) && result;
		}

		/// <summary>
		/// Calculates the capacity percentage based on working hours and activity status
		/// </summary>
		public int GetCapacityPercentage()
		{
			// If operator is not active, capacity is 0%
			if (!GetIsActiveBool())
			{
				return 0;
			}

			// Calculate percentage based on maximum 9 hours = 100%
			const int maxWorkingHours = 9;
			int capacityPercentage = (int)Math.Round((double)Capacity / maxWorkingHours * 100);

			// Ensure the percentage is within valid range (0-100)
			return Math.Max(0, Math.Min(100, capacityPercentage));
		}

		/// <summary>
		/// Gets the capacity display text with percentage
		/// </summary>
		public string GetCapacityDisplay()
		{
			int percentage = GetCapacityPercentage();
			return $"{percentage}%";
		}
	}
}
