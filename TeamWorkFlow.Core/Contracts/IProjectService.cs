namespace TeamWorkFlow.Core.Contracts
{
	public interface IProjectService
	{
        Task<bool> ExistByProjectNumberAsync(string projectNumber);
        Task<int?> GetProjectIdByProjectNumberAsync(string projectNumber);
    }
}
