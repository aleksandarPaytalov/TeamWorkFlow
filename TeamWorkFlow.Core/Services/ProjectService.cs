using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Project;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Core.Services
{
    public class ProjectService : IProjectService
	{
        private readonly IRepository _repository;

        public ProjectService(IRepository repository)
        {
            _repository = repository;
        }


        public async Task<bool> ExistByProjectNumberAsync(string projectNumber)
        {
            return await _repository.AllReadOnly<Project>()
                .AnyAsync(p => p.ProjectNumber == projectNumber);
        }

        public async Task<int?> GetProjectIdByProjectNumberAsync(string projectNumber)
        {
	        var projectId = (await _repository.AllReadOnly<Project>()
		        .FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber))?.Id;

		        return projectId;
        }

        public async Task<IEnumerable<ProjectServiceModel>> GetAllProjectsAsync()
        {
	        return await _repository.AllReadOnly<Project>()
		        .Select(p => new ProjectServiceModel()
		        {
			        Id = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber,
			        Status = p.ProjectStatus.Name,
			        TotalParts = p.Parts.Count
		        })
		        .ToListAsync();
        }

        public async Task<IEnumerable<ProjectStatusServiceModel>> GetAllProjectStatusesAsync()
        {
	        return await _repository.AllReadOnly<ProjectStatus>()
		        .Select(ps => new ProjectStatusServiceModel()
		        {
			        Id = ps.Id,
			        Name = ps.Name
		        })
		        .ToListAsync();
        }
	}
}
