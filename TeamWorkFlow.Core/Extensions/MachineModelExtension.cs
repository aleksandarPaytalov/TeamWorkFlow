using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Extensions
{
	public static class MachineModelExtension
	{
		public static string GetMachineExtension(this IMachineModel machineModel)
		{
			string[] machineName = machineModel.Name.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			string machineUrlNameExtension = string.Join("-", machineName.Take(1));

			return machineUrlNameExtension;
		}
	}
}
