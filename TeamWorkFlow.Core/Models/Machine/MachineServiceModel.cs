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
    }

    public class PaginatedMachinesViewModel
    {
        public IEnumerable<MachineServiceModel> Machines { get; set; } = new List<MachineServiceModel>();
        public PagerServiceModel Pager { get; set; }
    }
}
