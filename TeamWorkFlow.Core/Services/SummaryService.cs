using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Summary;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Core.Services
{
	public class SummaryService : ISummaryService
	{
		private readonly IRepository _repository;

		public SummaryService(IRepository repository)
		{
			_repository = repository;
		}

		public async Task<SummaryServiceModel> SummaryAsync()
		{
			int totalTasks = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
				.CountAsync();
			int finishedTasks = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
				.Where(t => t.TaskStatus.Name.ToLower() == "finished")
				.CountAsync();

			int totalProjects = await _repository.AllReadOnly<Project>()
				.CountAsync();
			int projectsInProduction = await _repository.AllReadOnly<Project>()
				.Where(p => p.ProjectStatus.Name.ToLower() == "in production")
				.CountAsync();

			int totalParts = await _repository.AllReadOnly<Part>()
				.CountAsync();
			int totalPartsWithStatusApproved = await _repository.AllReadOnly<Part>()
				.Where(p => p.PartStatus.Name.ToLower() == "released")
				.CountAsync();

			int totalWorkers = await _repository.AllReadOnly<Operator>()
				.CountAsync();
			int totalAvailableWorkers = await _repository.AllReadOnly<Operator>()
				.Where(w => w.AvailabilityStatus.Name.ToLower() == "at work")
				.CountAsync();

			int totalMachines = await _repository.AllReadOnly<Machine>()
				.CountAsync();
			int totalAvailableMachines = await _repository.AllReadOnly<Machine>()
				.Where(m => m.IsCalibrated == true)
				.CountAsync();

			var model = new SummaryServiceModel()
			{
				TotalTasks = totalTasks,
				FinishedTasks = finishedTasks,
				TotalProjects = totalProjects,
				ProjectsInProduction = projectsInProduction,
				TotalParts = totalParts,
				TotalApprovedParts = totalPartsWithStatusApproved,
				TotalWorkers = totalWorkers,
                TotalAvailableWorkers = totalAvailableWorkers,
				TotalMachines = totalMachines,
				TotalAvailableMachines = totalAvailableMachines
			};

			return model;
		}
	}
}
