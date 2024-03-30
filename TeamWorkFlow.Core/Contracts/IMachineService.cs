using TeamWorkFlow.Core.Models.Machine;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IMachineService
	{
		Task<ICollection<MachineViewModel>> GetAllMachinesAsync();
		Task AddNewMachineAsync(MachineServiceModel model);

		Task<MachineServiceModel?> GetMachineForEditAsync(int id);
		Task EditMachineAsync(MachineServiceModel model, int id);
	}
}
