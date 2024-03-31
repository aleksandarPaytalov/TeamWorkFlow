using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Part;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;

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

        public async  Task<IEnumerable<string>> AllStatusNamesAsync()
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
    }
}
