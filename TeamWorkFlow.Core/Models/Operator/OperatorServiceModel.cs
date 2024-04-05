namespace TeamWorkFlow.Core.Models.Operator
{
	public class OperatorServiceModel
	{
		public int Id { get; set; }

		public string FullName { get; set; } = string.Empty;

		public string AvailabilityStatus { get; set; } = string.Empty;

		public string Email { get; set; } = null!;

		public string PhoneNumber { get; set; } = string.Empty;

		public bool IsActive { get; set; }

		public int Capacity { get; set; }
	}
}
