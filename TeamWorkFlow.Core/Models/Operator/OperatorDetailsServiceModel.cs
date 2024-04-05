namespace TeamWorkFlow.Core.Models.Operator
{
	public class OperatorDetailsServiceModel : OperatorServiceModel
	{
		public int CurrentTasks { get; set; }

		public int TotalCompletedTasks { get; set; }
	}
}
