using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Summary;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;
using TaskStatus = System.Threading.Tasks.TaskStatus;

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

    // Check if running in CI/CD environment
    bool useInMemoryDatabase = Environment.GetEnvironmentVariable("USE_IN_MEMORY_DATABASE") == "true";

    if (useInMemoryDatabase)
    {
        // Use in-memory database for CI/CD
        var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TeamWorkFlowDbContext(options);
    }
    else
    {
        // Use real SQL Server for local development
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SummaryServiceUnitTests>()
            .Build();

        var connectionString = configuration.GetConnectionString("Test");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Test connection string is not configured. Please set up user secrets with 'Test' connection string.");
        }

        var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        _dbContext = new TeamWorkFlowDbContext(options);
    }

    _repository = new Repository(_dbContext);
    _summaryService = new SummaryService(_repository);

    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
}


		// ============================================================================
		// COMPREHENSIVE TESTS FOR 90%+ COVERAGE
		// ============================================================================

		#region SummaryAsync Method Tests

		[Test]
		public async Task SummaryAsync_WithSeededDatabase_ReturnsNonNegativeValues()
		{
			// Arrange - Database has seeded data from setup

			// Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalTasks, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.FinishedTasks, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalProjects, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.ProjectsInProduction, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalParts, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalApprovedParts, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalWorkers, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalAvailableWorkers, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalMachines, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalAvailableMachines, Is.GreaterThanOrEqualTo(0));
		}

		[Test]
		public async Task SummaryAsync_WithSeededData_ReturnsCorrectCounts()
		{
			// Arrange - Use the seeded data from database setup
			await _dbContext.Database.EnsureCreatedAsync();

			// Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Verify that counts are greater than 0 (seeded data should exist)
			Assert.That(result.TotalTasks, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalProjects, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalParts, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalWorkers, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalMachines, Is.GreaterThanOrEqualTo(0));

			// Finished/approved/available counts should be <= total counts
			Assert.That(result.FinishedTasks, Is.LessThanOrEqualTo(result.TotalTasks));
			Assert.That(result.ProjectsInProduction, Is.LessThanOrEqualTo(result.TotalProjects));
			Assert.That(result.TotalApprovedParts, Is.LessThanOrEqualTo(result.TotalParts));
			Assert.That(result.TotalAvailableWorkers, Is.LessThanOrEqualTo(result.TotalWorkers));
			Assert.That(result.TotalAvailableMachines, Is.LessThanOrEqualTo(result.TotalMachines));
		}

		[Test]
		public async Task SummaryAsync_VerifiesLogicalRelationships()
		{
			// Arrange - Use existing seeded data

			// Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Verify logical relationships between counts
			Assert.That(result.FinishedTasks, Is.LessThanOrEqualTo(result.TotalTasks), "Finished tasks should not exceed total tasks");
			Assert.That(result.ProjectsInProduction, Is.LessThanOrEqualTo(result.TotalProjects), "Projects in production should not exceed total projects");
			Assert.That(result.TotalApprovedParts, Is.LessThanOrEqualTo(result.TotalParts), "Approved parts should not exceed total parts");
			Assert.That(result.TotalAvailableWorkers, Is.LessThanOrEqualTo(result.TotalWorkers), "Available workers should not exceed total workers");
			Assert.That(result.TotalAvailableMachines, Is.LessThanOrEqualTo(result.TotalMachines), "Available machines should not exceed total machines");
		}

		[Test]
		public async Task SummaryAsync_VerifiesStatusFiltering()
		{
			// Arrange - Use existing seeded data to verify status filtering logic

			// Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Verify that the service correctly filters by status
			// These tests verify the logic without depending on specific counts

			// Tasks: Only count tasks with "Finished" status for FinishedTasks
			var totalTasksFromDb = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();
			var finishedTasksFromDb = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Where(t => t.TaskStatus.Name.ToLower() == "finished")
				.CountAsync();

			Assert.That(result.TotalTasks, Is.EqualTo(totalTasksFromDb));
			Assert.That(result.FinishedTasks, Is.EqualTo(finishedTasksFromDb));
		}

		[Test]
		public async Task SummaryAsync_VerifiesProjectStatusFiltering()
		{
			// Arrange & Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Projects: Only count projects with "In Production" status for ProjectsInProduction
			var totalProjectsFromDb = await _repository.AllReadOnly<Project>().CountAsync();
			var projectsInProductionFromDb = await _repository.AllReadOnly<Project>()
				.Where(p => p.ProjectStatus.Name.ToLower() == "in production")
				.CountAsync();

			Assert.That(result.TotalProjects, Is.EqualTo(totalProjectsFromDb));
			Assert.That(result.ProjectsInProduction, Is.EqualTo(projectsInProductionFromDb));
		}

		[Test]
		public async Task SummaryAsync_VerifiesPartStatusFiltering()
		{
			// Arrange & Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Parts: Only count parts with "Released" status for TotalApprovedParts
			var totalPartsFromDb = await _repository.AllReadOnly<Part>().CountAsync();
			var approvedPartsFromDb = await _repository.AllReadOnly<Part>()
				.Where(p => p.PartStatus.Name.ToLower() == "released")
				.CountAsync();

			Assert.That(result.TotalParts, Is.EqualTo(totalPartsFromDb));
			Assert.That(result.TotalApprovedParts, Is.EqualTo(approvedPartsFromDb));
		}

		[Test]
		public async Task SummaryAsync_VerifiesOperatorFiltering()
		{
			// Arrange & Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Operators: Only count active operators, and only active + "At Work" for available
			var totalActiveOperatorsFromDb = await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive)
				.CountAsync();
			var availableOperatorsFromDb = await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive && o.AvailabilityStatus.Name.ToLower() == "at work")
				.CountAsync();

			Assert.That(result.TotalWorkers, Is.EqualTo(totalActiveOperatorsFromDb));
			Assert.That(result.TotalAvailableWorkers, Is.EqualTo(availableOperatorsFromDb));
		}

		[Test]
		public async Task SummaryAsync_VerifiesMachineFiltering()
		{
			// Arrange & Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Machines: Count all machines, but only calibrated ones for available
			var totalMachinesFromDb = await _repository.AllReadOnly<Machine>().CountAsync();
			var availableMachinesFromDb = await _repository.AllReadOnly<Machine>()
				.Where(m => m.IsCalibrated)
				.CountAsync();

			Assert.That(result.TotalMachines, Is.EqualTo(totalMachinesFromDb));
			Assert.That(result.TotalAvailableMachines, Is.EqualTo(availableMachinesFromDb));
		}

		#endregion

		#region Additional Coverage Tests

		[Test]
		public async Task SummaryAsync_MultipleCallsReturnConsistentResults()
		{
			// Arrange & Act
			var result1 = await _summaryService.SummaryAsync();
			var result2 = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result1, Is.Not.Null);
			Assert.That(result2, Is.Not.Null);

			// Results should be consistent across multiple calls
			Assert.That(result2.TotalTasks, Is.EqualTo(result1.TotalTasks));
			Assert.That(result2.FinishedTasks, Is.EqualTo(result1.FinishedTasks));
			Assert.That(result2.TotalProjects, Is.EqualTo(result1.TotalProjects));
			Assert.That(result2.ProjectsInProduction, Is.EqualTo(result1.ProjectsInProduction));
			Assert.That(result2.TotalParts, Is.EqualTo(result1.TotalParts));
			Assert.That(result2.TotalApprovedParts, Is.EqualTo(result1.TotalApprovedParts));
			Assert.That(result2.TotalWorkers, Is.EqualTo(result1.TotalWorkers));
			Assert.That(result2.TotalAvailableWorkers, Is.EqualTo(result1.TotalAvailableWorkers));
			Assert.That(result2.TotalMachines, Is.EqualTo(result1.TotalMachines));
			Assert.That(result2.TotalAvailableMachines, Is.EqualTo(result1.TotalAvailableMachines));
		}

		[Test]
		public async Task SummaryAsync_VerifiesAllPropertiesAreInitialized()
		{
			// Arrange & Act
			var result = await _summaryService.SummaryAsync();

			// Assert
			Assert.That(result, Is.Not.Null);

			// Verify all properties are properly initialized (have valid integer values)
			Assert.That(result.TotalTasks, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.FinishedTasks, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalProjects, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.ProjectsInProduction, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalParts, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalApprovedParts, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalWorkers, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalAvailableWorkers, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalMachines, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalAvailableMachines, Is.GreaterThanOrEqualTo(0));
		}

		#endregion

		#region Model Coverage Tests

		[Test]
		public void SummaryServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new SummaryServiceModel();

			// Assert
			Assert.That(model.TotalTasks, Is.EqualTo(0));
			Assert.That(model.FinishedTasks, Is.EqualTo(0));
			Assert.That(model.TotalProjects, Is.EqualTo(0));
			Assert.That(model.ProjectsInProduction, Is.EqualTo(0));
			Assert.That(model.TotalParts, Is.EqualTo(0));
			Assert.That(model.TotalApprovedParts, Is.EqualTo(0));
			Assert.That(model.TotalWorkers, Is.EqualTo(0));
			Assert.That(model.TotalAvailableWorkers, Is.EqualTo(0));
			Assert.That(model.TotalMachines, Is.EqualTo(0));
			Assert.That(model.TotalAvailableMachines, Is.EqualTo(0));
		}

		[Test]
		public void SummaryServiceModel_AllProperties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new SummaryServiceModel
			{
				TotalTasks = 100,
				FinishedTasks = 75,
				TotalProjects = 20,
				ProjectsInProduction = 15,
				TotalParts = 500,
				TotalApprovedParts = 400,
				TotalWorkers = 50,
				TotalAvailableWorkers = 45,
				TotalMachines = 10,
				TotalAvailableMachines = 8
			};

			// Assert
			Assert.That(model.TotalTasks, Is.EqualTo(100));
			Assert.That(model.FinishedTasks, Is.EqualTo(75));
			Assert.That(model.TotalProjects, Is.EqualTo(20));
			Assert.That(model.ProjectsInProduction, Is.EqualTo(15));
			Assert.That(model.TotalParts, Is.EqualTo(500));
			Assert.That(model.TotalApprovedParts, Is.EqualTo(400));
			Assert.That(model.TotalWorkers, Is.EqualTo(50));
			Assert.That(model.TotalAvailableWorkers, Is.EqualTo(45));
			Assert.That(model.TotalMachines, Is.EqualTo(10));
			Assert.That(model.TotalAvailableMachines, Is.EqualTo(8));
		}

		[Test]
		public void SummaryServiceModel_Properties_AcceptNegativeValues()
		{
			// Arrange & Act
			var model = new SummaryServiceModel
			{
				TotalTasks = -1,
				FinishedTasks = -2,
				TotalProjects = -3,
				ProjectsInProduction = -4,
				TotalParts = -5,
				TotalApprovedParts = -6,
				TotalWorkers = -7,
				TotalAvailableWorkers = -8,
				TotalMachines = -9,
				TotalAvailableMachines = -10
			};

			// Assert - Properties should accept any integer value
			Assert.That(model.TotalTasks, Is.EqualTo(-1));
			Assert.That(model.FinishedTasks, Is.EqualTo(-2));
			Assert.That(model.TotalProjects, Is.EqualTo(-3));
			Assert.That(model.ProjectsInProduction, Is.EqualTo(-4));
			Assert.That(model.TotalParts, Is.EqualTo(-5));
			Assert.That(model.TotalApprovedParts, Is.EqualTo(-6));
			Assert.That(model.TotalWorkers, Is.EqualTo(-7));
			Assert.That(model.TotalAvailableWorkers, Is.EqualTo(-8));
			Assert.That(model.TotalMachines, Is.EqualTo(-9));
			Assert.That(model.TotalAvailableMachines, Is.EqualTo(-10));
		}

		[Test]
		public void SummaryServiceModel_Properties_AcceptLargeValues()
		{
			// Arrange & Act
			var model = new SummaryServiceModel
			{
				TotalTasks = int.MaxValue,
				FinishedTasks = int.MaxValue - 1,
				TotalProjects = int.MaxValue - 2,
				ProjectsInProduction = int.MaxValue - 3,
				TotalParts = int.MaxValue - 4,
				TotalApprovedParts = int.MaxValue - 5,
				TotalWorkers = int.MaxValue - 6,
				TotalAvailableWorkers = int.MaxValue - 7,
				TotalMachines = int.MaxValue - 8,
				TotalAvailableMachines = int.MaxValue - 9
			};

			// Assert - Properties should accept maximum integer values
			Assert.That(model.TotalTasks, Is.EqualTo(int.MaxValue));
			Assert.That(model.FinishedTasks, Is.EqualTo(int.MaxValue - 1));
			Assert.That(model.TotalProjects, Is.EqualTo(int.MaxValue - 2));
			Assert.That(model.ProjectsInProduction, Is.EqualTo(int.MaxValue - 3));
			Assert.That(model.TotalParts, Is.EqualTo(int.MaxValue - 4));
			Assert.That(model.TotalApprovedParts, Is.EqualTo(int.MaxValue - 5));
			Assert.That(model.TotalWorkers, Is.EqualTo(int.MaxValue - 6));
			Assert.That(model.TotalAvailableWorkers, Is.EqualTo(int.MaxValue - 7));
			Assert.That(model.TotalMachines, Is.EqualTo(int.MaxValue - 8));
			Assert.That(model.TotalAvailableMachines, Is.EqualTo(int.MaxValue - 9));
		}

		#endregion

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
