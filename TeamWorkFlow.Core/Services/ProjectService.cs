using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Project;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

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

        public async Task<int> AddNewProjectsAsync(ProjectFormModel model)
        {
	        Project projectToAdd = new Project()
	        {
		        ProjectName = model.ProjectName,
		        ProjectNumber = model.ProjectNumber,
		        ProjectStatusId = model.ProjectStatusId,
		        TotalHoursSpent = model.TotalHoursSpent,
		        Appliance = model.Appliance,
		        ClientName = model.ClientName
	        };

	        await _repository.AddAsync(projectToAdd);
	        await _repository.SaveChangesAsync();

	        return projectToAdd.Id;
        }

        public async Task<bool> ProjectStatusExistAsync(int statusId)
        {
	        bool exist = await _repository.AllReadOnly<ProjectStatus>()
		        .AnyAsync(ps => ps.Id == statusId);

	        return exist;
        }

        public async Task<ProjectFormModel?> GetProjectForEditByIdAsync(int projectId)
        {
	        var projectForEdit = await _repository.AllReadOnly<Project>()
		        .Where(p => p.Id == projectId)
		        .Select(p => new ProjectFormModel()
		        {
					ProjectName = p.ProjectName,
					ProjectNumber = p.ProjectNumber,
					Appliance = p.Appliance,
					ClientName = p.ClientName,
					ProjectStatusId = p.ProjectStatusId,
					TotalHoursSpent = p.TotalHoursSpent
		        })
		        .FirstOrDefaultAsync();

	        if (projectForEdit != null)
	        {
		        projectForEdit.ProjectStatuses = await GetAllProjectStatusesAsync();
	        }

	        return projectForEdit;
        }

        public async Task EditProjectAsync(ProjectFormModel model, int projectId)
        {
	        var project = await _repository.GetByIdAsync<Project>(projectId);

	        if (project != null)
	        {
		        project.ProjectName = model.ProjectName;
		        project.ProjectNumber = model.ProjectNumber;
		        project.ProjectStatusId = model.ProjectStatusId;
		        project.Appliance = model.Appliance;
		        project.ClientName = model.ClientName;
		        project.TotalHoursSpent = model.TotalHoursSpent;

		        await _repository.SaveChangesAsync();
	        }
        }

        public async Task<bool> ProjectExistByIdAsync(int projectId)
        {
	        var exist = await _repository.AllReadOnly<Project>()
		        .AnyAsync(p => p.Id == projectId);

	        return exist;
        }

        public async Task<IEnumerable<int>> GetAllProjectIdsByProjectNumberAsync(string projectNumber)
        {
	        var projects = await _repository.AllReadOnly<Project>()
		        .Where(p => p.ProjectNumber == projectNumber)
		        .ToListAsync();
			
			List<int> identifiers = new List<int>();
			foreach (var p in projects)
			{
				int id = p.Id;
				identifiers.Add(id);
			}

			return identifiers;
        }

        public async Task<ProjectDetailsServiceModel?> GetProjectDetailsByIdAsync(int projectId)
        {
	        return await _repository.AllReadOnly<Project>()
		        .Where(p => p.Id == projectId)
		        .Select(p => new ProjectDetailsServiceModel()
		        {
			        Id = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber,
			        Appliance = p.Appliance ?? string.Empty,
			        ClientName = p.ClientName ?? string.Empty,
			        Status = p.ProjectStatus.Name,
			        TotalHoursSpent = p.TotalHoursSpent,
			        TotalParts = p.Parts.Count
		        })
		        .FirstOrDefaultAsync();
        }

        public async Task<ProjectDeleteServiceModel?> GetProjectForDeleteByIdAsync(int projectId)
        {
	        return await _repository.AllReadOnly<Project>()
		        .Where(p => p.Id == projectId)
		        .Select(p => new ProjectDeleteServiceModel()
		        {
			        Id = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber
		        })
		        .FirstOrDefaultAsync();
        }

        public async Task ProjectDeleteAsync(int projectId)
        {
	        var project = await _repository.GetByIdAsync<Project>(projectId);

	        if (project != null)
	        {
		        await _repository.DeleteAsync<Project>(project.Id);
		        await _repository.SaveChangesAsync();
	        }
        }
	}
}
