using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Project;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using static TeamWorkFlow.Core.Constants.Messages;
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

        public async Task<(IEnumerable<ProjectServiceModel> Projects, int TotalCount)> GetAllProjectsAsync(int page, int pageSize)
        {
	        var query = _repository.AllReadOnly<Project>();
	        var totalCount = await query.CountAsync();
	        var projects = await query
		        .OrderBy(p => p.Id)
		        .Skip((page - 1) * pageSize)
		        .Take(pageSize)
		        .Select(p => new ProjectServiceModel()
		        {
			        Id = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber,
			        Status = p.ProjectStatus.Name,
			        TotalParts = p.Parts.Count
		        })
		        .ToListAsync();

	        return (projects, totalCount);
        }

        public async Task<ProjectQueryServiceModel> AllAsync(
            ProjectSorting sorting = ProjectSorting.LastAdded,
            string? search = null,
            int projectsPerPage = 10,
            int currentPage = 1)
        {
	        IQueryable<Project> projectsToBeDisplayed = _repository.AllReadOnly<Project>();

	        if (!string.IsNullOrWhiteSpace(search))
	        {
		        string normalizedSearch = search.ToLower();
		        projectsToBeDisplayed = projectsToBeDisplayed
			        .Where(p => p.ProjectName.ToLower().Contains(normalizedSearch) ||
			                   p.ProjectNumber.ToLower().Contains(normalizedSearch) ||
			                   (p.ClientName != null && p.ClientName.ToLower().Contains(normalizedSearch)));
	        }

	        projectsToBeDisplayed = sorting switch
	        {
		        ProjectSorting.NameAscending => projectsToBeDisplayed.OrderBy(p => p.ProjectName),
		        ProjectSorting.NameDescending => projectsToBeDisplayed.OrderByDescending(p => p.ProjectName),
		        ProjectSorting.ProjectNumberAscending => projectsToBeDisplayed.OrderBy(p => p.ProjectNumber),
		        ProjectSorting.ProjectNumberDescending => projectsToBeDisplayed.OrderByDescending(p => p.ProjectNumber),
		        ProjectSorting.StatusAscending => projectsToBeDisplayed.OrderBy(p => p.ProjectStatus.Name),
		        ProjectSorting.StatusDescending => projectsToBeDisplayed.OrderByDescending(p => p.ProjectStatus.Name),
		        ProjectSorting.TotalPartsAscending => projectsToBeDisplayed.OrderBy(p => p.Parts.Count),
		        ProjectSorting.TotalPartsDescending => projectsToBeDisplayed.OrderByDescending(p => p.Parts.Count),
		        _ => projectsToBeDisplayed.OrderByDescending(p => p.Id)
	        };

	        var projects = await projectsToBeDisplayed
		        .Include(p => p.Tasks)
		        .Skip((currentPage - 1) * projectsPerPage)
		        .Take(projectsPerPage)
		        .Select(p => new ProjectServiceModel()
		        {
			        Id = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber,
			        Status = p.ProjectStatus.Name,
			        TotalParts = p.Parts.Count,
			        // Time tracking properties
			        CalculatedTotalHours = p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime),
			        TotalEstimatedHours = p.Tasks.Sum(t => t.EstimatedTime),
			        CompletionPercentage = p.Tasks.Sum(t => t.EstimatedTime) > 0
				        ? (double)p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime) / p.Tasks.Sum(t => t.EstimatedTime) * 100
				        : 0
		        })
		        .ToListAsync();

	        int totalProjects = await projectsToBeDisplayed.CountAsync();

	        return new ProjectQueryServiceModel()
	        {
		        Projects = projects,
		        TotalProjectsCount = totalProjects
	        };
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
	        if (await ExistByProjectNumberAsync(model.ProjectNumber))
	        {
		        throw new ArgumentException($"{ProjectWithThisNumberAlreadyCreated}");
	        }

	        if (!await ProjectStatusExistAsync(model.ProjectStatusId))
	        {
		        throw new ArgumentException($"{StatusNotExisting}");
	        }

	        Project projectToAdd = new()
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
				var statusExist = await ProjectStatusExistAsync(model.ProjectStatusId);
				if (!statusExist)
				{
			        throw new ArgumentException($"{StatusNotExisting}");
		        }

				if (model.TotalHoursSpent < 0)
				{
					throw new ArgumentException($"{TotalHoursNegative}");
				}

		        project.ProjectName = model.ProjectName;
		        project.ProjectNumber = model.ProjectNumber;
		        project.ProjectStatusId = model.ProjectStatusId;
		        project.Appliance = model.Appliance;
		        project.ClientName = model.ClientName;
		        project.TotalHoursSpent = model.TotalHoursSpent;

		        await _repository.SaveChangesAsync();
	        }
	        else
	        {
		        throw new ArgumentException($"{ProjectNotExisting}");
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
			
			List<int> identifiers = new ();
			foreach (var p in projects)
			{
				int id = p.Id;
				identifiers.Add(id);
			}

			return identifiers;
        }

        public async Task<ProjectDetailsServiceModel?> GetProjectDetailsByIdAsync(int projectId)
        {
	        var project = await _repository.AllReadOnly<Project>()
		        .Include(p => p.Tasks)
		        .Where(p => p.Id == projectId)
		        .Select(p => new ProjectDetailsServiceModel()
		        {
			        Id = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber,
			        Appliance = p.Appliance ?? string.Empty,
			        ClientName = p.ClientName ?? string.Empty,
			        Status = p.ProjectStatus.Name,
			        TotalParts = p.Parts.Count,
			        // Task-based calculations
			        CalculatedTotalHours = p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime),
			        TotalEstimatedHours = p.Tasks.Sum(t => t.EstimatedTime),
			        CompletionPercentage = p.Tasks.Sum(t => t.EstimatedTime) > 0
				        ? (double)p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime) / p.Tasks.Sum(t => t.EstimatedTime) * 100
				        : 0,
			        // Task counts
			        FinishedTasksCount = p.Tasks.Count(t => t.TaskStatusId == 3),
			        InProgressTasksCount = p.Tasks.Count(t => t.TaskStatusId == 2),
			        OpenTasksCount = p.Tasks.Count(t => t.TaskStatusId == 1),
			        TotalTasksCount = p.Tasks.Count
		        })
		        .FirstOrDefaultAsync();

	        if (project != null)
	        {
		        // Get operator contributions for finished tasks
		        var operatorContributions = await _repository.AllReadOnly<TaskOperator>()
			        .Include(to => to.Task)
			        .Include(to => to.Operator)
			        .Where(to => to.Task.ProjectId == projectId && to.Task.TaskStatusId == 3) // Only finished tasks
			        .GroupBy(to => new { to.OperatorId, to.Operator.FullName })
			        .Select(g => new OperatorContributionServiceModel
			        {
				        OperatorId = g.Key.OperatorId,
				        OperatorName = g.Key.FullName,
				        TotalHours = g.Sum(to => to.Task.EstimatedTime)
			        })
			        .ToListAsync();

		        // Calculate contribution percentages
		        var totalProjectHours = project.CalculatedTotalHours;
		        if (totalProjectHours > 0)
		        {
			        foreach (var contribution in operatorContributions)
			        {
				        contribution.ContributionPercentage = (double)contribution.TotalHours / totalProjectHours * 100;
			        }
		        }

		        // Sort by contribution (highest first)
		        project.OperatorContributions = operatorContributions
			        .OrderByDescending(oc => oc.TotalHours)
			        .ToList();
	        }

	        return project;
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

	        else
	        {
		        throw new ArgumentException($"{ProjectNotExisting}");
	        }
        }

        public async Task<ProjectTimeCalculationServiceModel?> GetProjectTimeCalculationByIdAsync(int projectId)
        {
	        return await _repository.AllReadOnly<Project>()
		        .Include(p => p.Tasks)
		        .Where(p => p.Id == projectId)
		        .Select(p => new ProjectTimeCalculationServiceModel()
		        {
			        ProjectId = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber,
			        TotalHoursSpent = p.TotalHoursSpent,
			        // Task-based calculations
			        CalculatedTotalHours = p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime),
			        TotalEstimatedHours = p.Tasks.Sum(t => t.EstimatedTime),
			        TimeVariance = p.TotalHoursSpent - p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime),
			        CompletionPercentage = p.Tasks.Sum(t => t.EstimatedTime) > 0
				        ? (double)p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime) / p.Tasks.Sum(t => t.EstimatedTime) * 100
				        : 0,
			        // Task counts
			        FinishedTasksCount = p.Tasks.Count(t => t.TaskStatusId == 3),
			        InProgressTasksCount = p.Tasks.Count(t => t.TaskStatusId == 2),
			        OpenTasksCount = p.Tasks.Count(t => t.TaskStatusId == 1),
			        TotalTasksCount = p.Tasks.Count
		        })
		        .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProjectTimeCalculationServiceModel>> GetAllProjectsWithTimeCalculationsAsync()
        {
	        return await _repository.AllReadOnly<Project>()
		        .Include(p => p.Tasks)
		        .Select(p => new ProjectTimeCalculationServiceModel()
		        {
			        ProjectId = p.Id,
			        ProjectName = p.ProjectName,
			        ProjectNumber = p.ProjectNumber,
			        TotalHoursSpent = p.TotalHoursSpent,
			        // Task-based calculations
			        CalculatedTotalHours = p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime),
			        TotalEstimatedHours = p.Tasks.Sum(t => t.EstimatedTime),
			        TimeVariance = p.TotalHoursSpent - p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime),
			        CompletionPercentage = p.Tasks.Sum(t => t.EstimatedTime) > 0
				        ? (double)p.Tasks.Where(t => t.TaskStatusId == 3).Sum(t => t.EstimatedTime) / p.Tasks.Sum(t => t.EstimatedTime) * 100
				        : 0,
			        // Task counts
			        FinishedTasksCount = p.Tasks.Count(t => t.TaskStatusId == 3),
			        InProgressTasksCount = p.Tasks.Count(t => t.TaskStatusId == 2),
			        OpenTasksCount = p.Tasks.Count(t => t.TaskStatusId == 1),
			        TotalTasksCount = p.Tasks.Count
		        })
		        .ToListAsync();
        }
	}
}
