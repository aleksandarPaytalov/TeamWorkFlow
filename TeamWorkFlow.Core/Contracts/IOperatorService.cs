using TeamWorkFlow.Core.Models.Operator;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IOperatorService
	{
		Task<ICollection<OperatorViewModel>> GetAllOperatorsAsync();
		Task<ICollection<AvailabilityStatusViewModel>> GetAllStatusesAsync();
		Task AddNewOperatorAsync(OperatorServicesModel model);
		Task<OperatorServicesModel?> GetOperatorForEditAsync(int id);

		Task EditOperatorAsync(OperatorServicesModel model, int id);
	}
}
