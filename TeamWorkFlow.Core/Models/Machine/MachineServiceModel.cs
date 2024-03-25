using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Machine
{
	public class MachineServiceModel
	{

		public int Id { get; set; }

		[Required(ErrorMessage = Messages.RequiredMessage)]
		[StringLength(DataConstants.MachineNameMaxLength,
			MinimumLength = DataConstants.MachineNameMinLength,
			ErrorMessage = Messages.StringLength)]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = Messages.RequiredMessage)]
		public int Capacity { get; set; }

		[Required(ErrorMessage = Messages.RequiredMessage)]
		public string CalibrationSchedule { get; set; } = string.Empty;

		[Required(ErrorMessage = Messages.RequiredMessage)]
		public string IsCalibrated { get; set; } = string.Empty;
	}
}
