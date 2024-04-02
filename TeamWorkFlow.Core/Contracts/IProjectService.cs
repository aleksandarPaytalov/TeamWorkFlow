using TeamWorkFlow.Core.Models.Project;

namespace TeamWorkFlow.Core.Contracts
{
	public interface IProjectService
	{
        Task<bool> ExistByProjectNumberAsync(string projectNumber);
        Task<int?> GetProjectIdByProjectNumberAsync(string projectNumber);
        Task<IEnumerable<ProjectServiceModel>> GetAllProjectsAsync();
	}
}
