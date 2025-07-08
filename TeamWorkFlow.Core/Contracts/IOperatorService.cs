using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Admin.Operator;
using TeamWorkFlow.Core.Models.Operator;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IOperatorService
	{
		Task<ICollection<OperatorServiceModel>> GetAllActiveOperatorsAsync();
		Task<(ICollection<OperatorServiceModel> Operators, int TotalCount)> GetAllActiveOperatorsAsync(int page, int pageSize);
		Task<OperatorQueryServiceModel> AllAsync(
			OperatorSorting sorting = OperatorSorting.LastAdded,
			string? search = null,
			int operatorsPerPage = 10,
			int currentPage = 1);
		Task<ICollection<AvailabilityStatusServiceModel>> GetAllOperatorStatusesAsync();
		Task AddNewOperatorAsync(OperatorFormModel model, string userId);
		Task<OperatorFormModel?> GetOperatorForEditAsync(int id);
		Task EditOperatorAsync(OperatorFormModel model, int id);
		Task<bool> OperatorStatusExistAsync(int statusId);
		Task<bool> OperatorExistByIdAsync(int operatorId);
		Task<OperatorDetailsServiceModel?> GetOperatorDetailsByIdAsync(int operatorId);
		Task<int> GetAllCompletedTasksAssignedToOperatorByIdAsync(int operatorId);
		Task<int> GetAllActiveAssignedTaskToOperatorByIdAsync(int operatorId);
		Task<OperatorDeleteServiceModel?> GetOperatorModelForDeleteByIdAsync(int operatorId);
		Task DeleteOperatorByIdAsync(int operatorId);
		Task<ICollection<OperatorAccessServiceModel>> GetAllOperatorsAsync();
		Task<ICollection<OperatorServiceModel>> GetAllUnActiveOperatorsAsync();
		Task ActivateOperatorAsync(int id);
		Task<string?> GetUserIdByEmailAsync(string emailAddress);
		Task<string?> GetOperatorFullNameByUserIdAsync(string userId);
	}
}
