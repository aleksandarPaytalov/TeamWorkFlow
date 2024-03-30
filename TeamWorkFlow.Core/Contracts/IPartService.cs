using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Part;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IPartService
    {
        Task<PartQueryServiceModel> AllAsync(
            int partsPerPage = 1,
            int currentPage = 1,
            PartSorting sorting = PartSorting.LastAdded,
            string? searchByName = null,
            string? status = null
            );

        Task<IEnumerable<string>> AllStatusNamesAsync();

        Task<IEnumerable<PartStatusServiceModel>> AllStatusesAsync();
    }
}
