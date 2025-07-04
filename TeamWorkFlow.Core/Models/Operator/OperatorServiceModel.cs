using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Pager;

namespace TeamWorkFlow.Core.Models.Operator
{
	public class OperatorServiceModel : IOperatorModel
	{
		public int Id { get; set; }

		public string FullName { get; set; } = string.Empty;

		public string AvailabilityStatus { get; set; } = string.Empty;

		public string Email { get; set; } = null!;

		public string PhoneNumber { get; set; } = string.Empty;

		public bool IsActive { get; set; }

		public int Capacity { get; set; }
	}

	public class PaginatedOperatorsViewModel
	{
		public IEnumerable<OperatorServiceModel> Operators { get; set; } = new List<OperatorServiceModel>();
		public PagerServiceModel Pager { get; set; }
	}
}
