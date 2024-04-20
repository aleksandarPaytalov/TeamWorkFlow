using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
	[TestFixture]
	public class SummaryServiceUnitTests
	{
		private IRepository _repository;
		private ISummaryService _summaryService;
		private TeamWorkFlowDbContext _dbContext;
		private Mock<UserManager<IdentityUser>> _mockUserManager;

		[SetUp]
		public void Setup()
		{
			var mockUserStore = new Mock<IUserStore<IdentityUser>>();
			_mockUserManager = new Mock<UserManager<IdentityUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

			_mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
				.ReturnsAsync((IdentityUser user, string role) => role == "Operator" || role == "Admin");

			var configuration = new ConfigurationBuilder()
				.AddUserSecrets<SummaryServiceUnitTests>()
				.Build();

			var connectionString = configuration.GetConnectionString("Test");

			var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			_dbContext = new TeamWorkFlowDbContext(options);
			_repository = new Repository(_dbContext);
			_summaryService = new SummaryService(_repository);

			_dbContext.Database.EnsureDeleted();
			_dbContext.Database.EnsureCreated();
		}


		// To be Implemented
		[Test]
		public async Task SummaryAsync_ShouldReturnCorrectSummary()
		{
			// Arrange
			var totalTasksCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.CountAsync();
			var finishedTasksCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
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
				.Where(w => w.IsActive)
				.CountAsync();
			int totalAvailableWorkers = await _repository.AllReadOnly<Operator>()
				.Where(w => w.AvailabilityStatus.Name.ToLower() == "at work" && w.IsActive)
				.CountAsync();

			int totalMachines = await _repository.AllReadOnly<Machine>()
				.CountAsync();
			int totalAvailableMachines = await _repository.AllReadOnly<Machine>()
				.Where(m => m.IsCalibrated == true)
				.CountAsync();

			// Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result.FinishedTasks, Is.EqualTo(finishedTasksCount));
			Assert.That(result.TotalTasks, Is.EqualTo(totalTasksCount));
			Assert.That(result.TotalProjects, Is.EqualTo(totalProjects));
			Assert.That(result.ProjectsInProduction, Is.EqualTo(projectsInProduction));
			Assert.That(result.TotalParts, Is.EqualTo(totalParts));
			Assert.That(result.TotalApprovedParts, Is.EqualTo(totalPartsWithStatusApproved));
			Assert.That(result.TotalWorkers, Is.EqualTo(totalWorkers));
			Assert.That(result.TotalAvailableWorkers, Is.EqualTo(totalAvailableWorkers));
			Assert.That(result.TotalMachines, Is.EqualTo(totalMachines));
			Assert.That(result.TotalAvailableMachines, Is.EqualTo(totalAvailableMachines));
		}



		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
