namespace TeamWorkFlow.Core.Models.Machine
{
    public class MachineQueryServiceModel
    {
        public int TotalMachinesCount { get; set; }

        public IEnumerable<MachineServiceModel> Machines { get; set; } = new List<MachineServiceModel>();
    }
}
