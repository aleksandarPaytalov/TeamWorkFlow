using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Models.Machine
{
	public class MachineDeleteServiceModel : IMachineModel
	{
		public int Id { get; set; }
		public string ImageUrl { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public bool CanDelete { get; set; } = true;
		public string? DeletionBlockReason { get; set; }
	}
}
