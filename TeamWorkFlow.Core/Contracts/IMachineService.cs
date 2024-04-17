using TeamWorkFlow.Core.Models.Machine;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IMachineService
	{
		Task<ICollection<MachineServiceModel>> GetAllMachinesAsync();
		Task AddNewMachineAsync(MachineFormModel model);
		Task<MachineFormModel?> GetMachineForEditAsync(int id);
		Task EditMachineAsync(MachineFormModel model, int id);
		Task<bool> MachineExistByIdAsync(int machineId);
		Task<MachineDetailsServiceModel?> MachineDetailsAsync(int machineId);
		Task DeleteMachineAsync(int machineId);
		Task<MachineDeleteServiceModel?> GetMachineForDeleteByIdAsync(int machineId);
		Task<ICollection<TeamWorkFlow.Infrastructure.Data.Models.Task>> GetAllTaskByAssignedMachineId(int machineId);
	}
}
