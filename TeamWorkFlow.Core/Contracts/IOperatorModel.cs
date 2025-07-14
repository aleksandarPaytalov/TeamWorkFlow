namespace TeamWorkFlow.Core.Contracts
{
	public interface IOperatorModel
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public int Capacity { get; set; }
	}
}
