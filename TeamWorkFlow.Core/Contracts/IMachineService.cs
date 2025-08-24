using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Machine;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IMachineService
	{
		Task<ICollection<MachineServiceModel>> GetAllMachinesAsync();
		Task<(ICollection<MachineServiceModel> Machines, int TotalCount)> GetAllMachinesAsync(int page, int pageSize);
		Task<MachineQueryServiceModel> AllAsync(
			MachineSorting sorting = MachineSorting.LastAdded,
			string? search = null,
			int machinesPerPage = 10,
			int currentPage = 1);
		Task AddNewMachineAsync(MachineFormModel model);
		Task<MachineFormModel?> GetMachineForEditAsync(int id);
		Task EditMachineAsync(MachineFormModel model, int id);
		Task<bool> MachineExistByIdAsync(int machineId);
		Task<MachineDetailsServiceModel?> MachineDetailsAsync(int machineId);
		Task DeleteMachineAsync(int machineId);
		Task<bool> IsMachineAvailableForAssignmentAsync(int machineId);
		Task<(bool CanAssign, string Reason)> ValidateMachineAvailabilityAsync(int machineId, int? excludeTaskId = null);
		Task<MachineDeleteServiceModel?> GetMachineForDeleteByIdAsync(int machineId);
		Task<ICollection<TeamWorkFlow.Infrastructure.Data.Models.Task>> GetAllTaskByAssignedMachineId(int machineId);
		Task<(bool CanDelete, string Reason)> ValidateMachineForDeletionAsync(int machineId);
	}
}
