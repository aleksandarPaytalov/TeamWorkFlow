namespace TeamWorkFlow.Core.Exceptions
{
	public class UnExistingActionException : Exception
	{
		public UnExistingActionException()
		{
		}
		public UnExistingActionException(string message)
			:base(message) { }
	}
}
