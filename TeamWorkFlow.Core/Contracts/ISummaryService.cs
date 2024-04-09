using TeamWorkFlow.Core.Models.Summary;

namespace TeamWorkFlow.Core.Contracts
{
	public interface ISummaryService
	{
		Task<SummaryServiceModel> SummaryAsync();
	}
}
