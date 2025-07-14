using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Models.Operator
{
	public class OperatorDeleteServiceModel : IOperatorModel
	{
		public int Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public int Capacity { get; set; }
	}
}
