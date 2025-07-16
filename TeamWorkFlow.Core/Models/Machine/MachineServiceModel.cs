using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Pager;

namespace TeamWorkFlow.Core.Models.Machine
{
    public class MachineServiceModel : IMachineModel
    {
	    public int Id { get; set; }
	    public string Name { get; set; } = string.Empty;
		public string CalibrationSchedule { get; set; } = string.Empty;
	    public bool IsCalibrated { get; set; }
		public int Capacity { get; set; }

		// Assignment information
		public bool IsOccupied { get; set; }
		public int? AssignedTaskId { get; set; }
		public string? AssignedTaskName { get; set; }
		public string? AssignedTaskProjectNumber { get; set; }
		public string? AssignedOperatorNames { get; set; }
		public string? TaskStatus { get; set; }
    }

    public class PaginatedMachinesViewModel
    {
        public IEnumerable<MachineServiceModel> Machines { get; set; } = new List<MachineServiceModel>();
        public PagerServiceModel Pager { get; set; } = null!;
    }
}
