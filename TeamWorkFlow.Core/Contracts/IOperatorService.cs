using TeamWorkFlow.Core.Models.Operator;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IOperatorService
	{
		Task<ICollection<OperatorServiceModel>> GetAllOperatorsAsync();
		Task<ICollection<AvailabilityStatusServiceModel>> GetAllOperatorStatusesAsync();
		Task AddNewOperatorAsync(OperatorFormModel model);
		Task<OperatorFormModel?> GetOperatorForEditAsync(int id);
		Task EditOperatorAsync(OperatorFormModel model, int id);
		Task<bool> OperatorStatusExistAsync(int statusId);
		Task<bool> OperatorExistByIdAsync(int operatorId);
		Task<OperatorDetailsServiceModel?> GetOperatorDetailsByIdAsync(int operatorId);
		Task<int> GetAllCompletedTasksAssignedToOperatorByIdAsync(int operatorId);
		Task<int> GetAllActiveAssignedTaskToOperatorByIdAsync(int operatorId);
		Task<OperatorDeleteServiceModel?> GetOperatorModelForDeleteByIdAsync(int operatorId);
		Task DeleteOperatorByIdAsync(int operatorId);
	}
}
