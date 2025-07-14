namespace TeamWorkFlow.Core.Models.Admin.Operator
{
	public class OperatorAccessServiceModel
	{
		public int Id { get; set; }

		public string Email { get; set; } = null!;

		public string FullName { get; set; } = null!;

		public string PhoneNumber { get; set; } = null!;

		public bool IsActive { get; set; }
	}
}
