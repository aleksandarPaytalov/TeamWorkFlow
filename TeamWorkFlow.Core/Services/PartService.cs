using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Part;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Services
{
    public class PartService : IPartService
    {
        private readonly IRepository _repository;

        public PartService(IRepository repository)
        {
            _repository = repository;
        }


        public async Task<PartQueryServiceModel> AllAsync(
            PartSorting sorting = PartSorting.LastAdded,
            string? search = null, 
            string? status = null,
            int partsPerPage = 1,
            int currentPage = 1)
        {
            var partsToBeDisplayed = _repository.AllReadOnly<Part>();

            if (status != null)
            {
                partsToBeDisplayed = partsToBeDisplayed
                    .Where(p => p.PartStatus.Name == status);
            }

            if (search != null)
            {
                string normalizePartName = search.ToLower();

                if (int.TryParse(normalizePartName, out int toolNumber))
                {
                    partsToBeDisplayed = partsToBeDisplayed
                        .Where(p => p.ToolNumber == toolNumber);
                }

                else
                {
                    partsToBeDisplayed = partsToBeDisplayed
                        .Where(p => p.Name.ToLower().Contains(normalizePartName) ||
                                    p.PartArticleNumber.ToLower().Contains(normalizePartName) ||
                                    p.PartClientNumber.ToLower().Contains(normalizePartName));
                }
            }

            partsToBeDisplayed = sorting switch
            {
                PartSorting.ProjectNumberDescending => partsToBeDisplayed
                    .OrderByDescending(p => p.Project.ProjectNumber),
                PartSorting.ProjectNumberAscending => partsToBeDisplayed
                    .OrderBy(p => p.Project.ProjectNumber),
                _ => partsToBeDisplayed
                    .OrderByDescending(p => p.Id)
            };

            var parts = await partsToBeDisplayed
                .Skip((currentPage - 1) * partsPerPage)
                .Take(partsPerPage)
                .ProjectToPartServiceModels()
                .ToListAsync();

            int totalParts = await partsToBeDisplayed.CountAsync();

            return new PartQueryServiceModel()
            {
                Parts = parts,
                TotalPartsCount = totalParts
            };
        }

        public async Task<IEnumerable<string>> AllStatusNamesAsync()
        {
            return await _repository.AllReadOnly<PartStatus>()
                .Select(ps => ps.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<PartStatusServiceModel>> AllStatusesAsync()
        {
            return await _repository.AllReadOnly<PartStatus>()
                .Select(ps => new PartStatusServiceModel()
                {
                    Id = ps.Id,
                    Name = ps.Name
                })
                .ToListAsync();
        }

        public async Task<bool> StatusExistAsync(int statusId)
        {
            return await _repository.AllReadOnly<PartStatus>()
                .AnyAsync(ps => ps.Id == statusId);
        }

        public async Task<int> AddNewPartAsync(PartFormModel model, int projectId)
        {
            var partToCreate = new Part()
            {
                Name = model.Name,
                PartArticleNumber = model.PartArticleNumber,
                PartClientNumber = model.PartClientNumber,
                PartModel = model.PartModel,
                PartStatusId = model.PartStatusId,
                ImageUrl = model.ImageUrl,
                ToolNumber = model.ToolNumber,
                ProjectId = projectId
            };

            await _repository.AddAsync(partToCreate);
            await _repository.SaveChangesAsync();

            return partToCreate.Id;
        }

        public async Task<PartDetailsServiceModel> PartDetailsByIdAsync(int partId)
        {
            return await _repository.AllReadOnly<Part>()
                .Where(p => p.Id == partId)
                .Select(p => new PartDetailsServiceModel()
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                    Name = p.Name,
                    PartArticleNumber = p.PartArticleNumber,
                    PartClientNumber = p.PartClientNumber,
                    PartModel = p.PartModel,
                    ProjectNumber = p.Project.ProjectNumber,
                    ToolNumber = p.ToolNumber,
                    Status = p.PartStatus.Name
                    })
                .FirstAsync();
        }

        public async Task<bool> PartExistAsync(int partId)
        {
            return await _repository.AllReadOnly<Part>()
                .AnyAsync(p => p.Id == partId);
        }

        public async Task<PartFormModel?> GetPartFormModelForEditAsync(int partId)
        {
            var modelForEdit = await _repository.AllReadOnly<Part>()
                .Where(p => p.Id == partId)
                .Select(p => new PartFormModel()
                {
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    PartArticleNumber = p.PartArticleNumber,
                    PartClientNumber = p.PartClientNumber,
                    ProjectNumber = p.Project.ProjectNumber,
                    PartModel = p.PartModel,
                    ToolNumber = p.ToolNumber
                })
                .FirstOrDefaultAsync();

            if (modelForEdit != null)
            {
                modelForEdit.Statuses = await AllStatusesAsync();
            }

            return modelForEdit;
        }

        public async Task EditAsync(int partId, PartFormModel model, int projectId, int statusId)
        {
	        var partForEdit = await _repository.GetByIdAsync<Part>(partId);

	        if (partForEdit != null)
	        {
		        partForEdit.Name = model.Name;
		        partForEdit.ImageUrl = model.ImageUrl;
		        partForEdit.PartArticleNumber = model.PartArticleNumber;
		        partForEdit.PartClientNumber = model.PartClientNumber;
		        partForEdit.PartModel = model.PartModel;
		        partForEdit.ToolNumber = model.ToolNumber;
		        partForEdit.ProjectId = projectId;
		        partForEdit.PartStatusId = model.PartStatusId;

				await _repository.SaveChangesAsync();
	        }
        }

        public async Task<PartDeleteServiceModel?> GetPartForDeletingByIdAsync(int partId)
        {
            var partForDelete = await _repository.AllReadOnly<Part>()
                .Where(p => p.Id == partId)
                .Select(p => new PartDeleteServiceModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    PartNumber = p.PartArticleNumber,
                    ProjectNumber = p.Project.ProjectNumber
                })
                .FirstOrDefaultAsync();

            return partForDelete;
        }

        public async Task DeletePartByIdAsync(int partId)
        {
	        var partToDelete = await _repository.AllReadOnly<Part>()
		        .Where(p => p.Id == partId)
		        .FirstOrDefaultAsync();

	        if (partToDelete != null)
	        {
		        await _repository.DeleteAsync<Part>(partId);
		        await _repository.SaveChangesAsync();
	        }
        }
    }
}
