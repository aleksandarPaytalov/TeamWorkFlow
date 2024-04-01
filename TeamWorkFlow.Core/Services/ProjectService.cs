using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
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
    }
}
