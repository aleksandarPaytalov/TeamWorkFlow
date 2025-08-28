using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Project;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IProjectService
	{
        Task<bool> ExistByProjectNumberAsync(string projectNumber);
        Task<int?> GetProjectIdByProjectNumberAsync(string projectNumber);
        Task<IEnumerable<ProjectServiceModel>> GetAllProjectsAsync();
        Task<(IEnumerable<ProjectServiceModel> Projects, int TotalCount)> GetAllProjectsAsync(int page, int pageSize);
        Task<ProjectQueryServiceModel> AllAsync(
            ProjectSorting sorting = ProjectSorting.LastAdded,
            string? search = null,
            int projectsPerPage = 10,
            int currentPage = 1);
        Task<IEnumerable<ProjectStatusServiceModel>> GetAllProjectStatusesAsync();
        Task<int> AddNewProjectsAsync(ProjectFormModel model);
        Task<bool> ProjectStatusExistAsync(int statusId);
        Task<ProjectFormModel?> GetProjectForEditByIdAsync(int projectId);
        Task EditProjectAsync(ProjectFormModel model, int projectId);
        Task<bool> ProjectExistByIdAsync(int projectId);
        Task<IEnumerable<int>> GetAllProjectIdsByProjectNumberAsync(string projectNumber);
        Task<ProjectDetailsServiceModel?> GetProjectDetailsByIdAsync(int projectId);
        Task<ProjectDeleteServiceModel?> GetProjectForDeleteByIdAsync(int projectId);
        Task ProjectDeleteAsync(int projectId);

        Task<ProjectTimeCalculationServiceModel?> GetProjectTimeCalculationByIdAsync(int projectId);
        Task<IEnumerable<ProjectTimeCalculationServiceModel>> GetAllProjectsWithTimeCalculationsAsync();

        // Cost calculation methods
        Task<ProjectCostCalculationModel?> GetProjectCostCalculationByIdAsync(int projectId);
        Task<ProjectCostCalculationModel> CalculateProjectCostAsync(int projectId, decimal hourlyRate);
	}
}
