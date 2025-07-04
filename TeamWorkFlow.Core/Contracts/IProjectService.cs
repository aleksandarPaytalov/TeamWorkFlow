using TeamWorkFlow.Core.Models.Project;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IProjectService
	{
        Task<bool> ExistByProjectNumberAsync(string projectNumber);
        Task<int?> GetProjectIdByProjectNumberAsync(string projectNumber);
        Task<IEnumerable<ProjectServiceModel>> GetAllProjectsAsync();
        Task<(IEnumerable<ProjectServiceModel> Projects, int TotalCount)> GetAllProjectsAsync(int page, int pageSize);
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
	}
}
