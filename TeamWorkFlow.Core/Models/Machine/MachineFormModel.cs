using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Machine
{
	public class MachineFormModel
	{

		public int Id { get; set; }

		[Required(ErrorMessage = RequiredMessage)]
		[StringLength(MachineNameMaxLength,
			MinimumLength = MachineNameMinLength,
			ErrorMessage = StringLength)]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredMessage)]
		public int Capacity { get; set; }

		[Required(ErrorMessage = RequiredMessage)]
		[Display(Name = "Next calibration date")]
		public string CalibrationSchedule { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredMessage)]
		public string IsCalibrated { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredMessage)]
		[Display(Name = "Machine picture")]
		[StringLength(MachineImageUrlMaxLength,
			MinimumLength = MachineImageUrlMinLength,
			ErrorMessage = StringLength)]
		public string ImageUrl { get; set; } = string.Empty;
	}
}
