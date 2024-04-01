using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Part;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IPartService
    {
        Task<PartQueryServiceModel> AllAsync(
            PartSorting sorting = PartSorting.LastAdded,
            string? searchByName = null,
            string? status = null,
            int partsPerPage = 1,
            int currentPage = 1
            );

        Task<IEnumerable<string>> AllStatusNamesAsync();

        Task<IEnumerable<PartStatusServiceModel>> AllStatusesAsync();

        Task<bool> StatusExistAsync(int statusId);

        Task<int> AddNewPartAsync(PartFormModel model, int projectId);

        Task<PartDetailsServiceModel> PartDetailsByIdAsync(int partId);

        Task<bool> PartExistAsync(int partId);

        Task<PartFormModel?> GetPartFormModelForEditAsync(int partId);

        Task EditAsync(int partId, PartFormModel model, int projectId, int statusId);

    }
}
