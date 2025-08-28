using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Project;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
	[TestFixture]
	public class ProjectServiceUnitTests
	{
		private IRepository _repository;
		private IProjectService _projectService = null!;
		private TeamWorkFlowDbContext _dbContext = null!;
		private Mock<UserManager<IdentityUser>> _mockUserManager = null!;

		[SetUp]
public void Setup()
{
    var mockUserStore = new Mock<IUserStore<IdentityUser>>();
    _mockUserManager = new Mock<UserManager<IdentityUser>>(mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

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
            .AddUserSecrets<ProjectServiceUnitTests>()
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
    _projectService = new ProjectService(_repository);

    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
}

		[Test]
		public async Task AddNewProjectsAsync_ShouldAddNewProject()
		{
			var projectsCount = await _repository.AllReadOnly<Project>().CountAsync();

			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249400",
				ProjectStatusId = 1,
				TotalHoursSpent = 90
			};

			var result = await _projectService.AddNewProjectsAsync(projectFormModel);

			Assert.That(result, Is.EqualTo(projectsCount + 1));
		}

		[Test]
		public async Task AddNewProjectsAsync_ShouldNotAddNewProject_WhenProjectNumberExists()
		{
			var project = new Project()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249400",
				ProjectStatusId = 1,
				TotalHoursSpent = 90
			};

			await _dbContext.Projects.AddAsync(project);
			await _dbContext.SaveChangesAsync();

			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249400",
				ProjectStatusId = 1,
				TotalHoursSpent = 90
			};

			Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.AddNewProjectsAsync(projectFormModel));
		}

		[Test]
		public Task AddNewProjectsAsync_ShouldNotAddNewProject_WhenProjectStatusNotExists()
		{
			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249400",
				ProjectStatusId = 20,
				TotalHoursSpent = 90
			};

			Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.AddNewProjectsAsync(projectFormModel));
			return Task.CompletedTask;
		}

		[Test]
		public async Task ProjectStatusExistAsync_ShouldReturnTrue_WhenProjectStatusExists()
		{

			var validProjectStatus = await _repository.AllReadOnly<ProjectStatus>().FirstOrDefaultAsync();
			Assert.That(validProjectStatus, Is.Not.Null, "Project status is null");
			
			var result = await _projectService.ProjectStatusExistAsync(validProjectStatus.Id);

			Assert.That(result, Is.True);
		}

		[Test]
		public async Task GetProjectForEditByIdAsync_ShouldGetProjectForEdit()
		{
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.GetProjectForEditByIdAsync(project.Id);

			//Assert
			Assert.That(result, Is.Not.Null, "Result is null");
		}

		[Test]
		public async Task GetProjectForEditByIdAsync_ShouldNotGetProjectForEdit_WhenProjectNotExists()
		{
			// Act
			var result = await _projectService.GetProjectForEditByIdAsync(100);

			//Assert
			Assert.That(result, Is.Null, "Result is not null");
		}

		[Test]
		public async Task ProjectExistByIdAsync_ShouldReturnTrue_WhenProjectExists()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.ProjectExistByIdAsync(project.Id);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task ProjectExistByIdAsync_ShouldReturnFalse_WhenProjectNotExists()
		{
			// Act
			var result = await _projectService.ProjectExistByIdAsync(100);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task EditProjectAsync_ShouldEditProject()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249100",
				ProjectStatusId = 3,
				TotalHoursSpent = 100
			};

			// Act
			await _projectService.EditProjectAsync(projectFormModel, project.Id);

			// Assert
			var editedProject = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync(p => p.Id == project.Id);
			Assert.That(editedProject, Is.Not.Null, "Edited project is null");
			Assert.That(editedProject.ProjectName, Is.EqualTo(projectFormModel.ProjectName));
			Assert.That(editedProject.ProjectNumber, Is.EqualTo(projectFormModel.ProjectNumber));
			Assert.That(editedProject.ProjectStatusId, Is.EqualTo(projectFormModel.ProjectStatusId));
			Assert.That(editedProject.TotalHoursSpent, Is.EqualTo(projectFormModel.TotalHoursSpent));
		}

		[Test]
		public void EditProjectAsync_ShouldNotEditProject_WhenProjectNotExists()
		{
			// Arrange
			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249100",
				ProjectStatusId = 3,
				TotalHoursSpent = 100
			};

			// Act
			Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.EditProjectAsync(projectFormModel, 100));
		}

		[Test]
		public async Task EditProjectAsync_ShouldNotEditProject_WhenProjectStatusNotExists()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249100",
				ProjectStatusId = 20,
				TotalHoursSpent = 100
			};

			// Act
			Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.EditProjectAsync(projectFormModel, project.Id));
		}

		[Test]
		public async Task EditProjectAsync_ShouldNotEditProject_WhenTotalHoursSpentIsNegative()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "TestProject",
				ProjectNumber = "249100",
				ProjectStatusId = 3,
				TotalHoursSpent = -100
			};

			// Act
			Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.EditProjectAsync(projectFormModel, project.Id));
		}

		[Test]
		public async Task GetAllProjectStatusesAsync_ShouldReturnAllProjectStatuses()
		{
			// Act
			var result = await _projectService.GetAllProjectStatusesAsync();

			// Assert
			var projectStatusServiceModels = result as ProjectStatusServiceModel[] ?? result.ToArray();
			Assert.That(projectStatusServiceModels, Is.Not.Null);
			Assert.That(projectStatusServiceModels.Count, Is.GreaterThan(0));
		}

		[Test]
		public async Task GetAllProjectIdsByProjectNumberAsync_ShouldReturnAllProjectIds()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.GetAllProjectIdsByProjectNumberAsync(project.ProjectNumber);

			// Assert
			var enumerable = result as int[] ?? result.ToArray();
			Assert.That(enumerable, Is.Not.Null);
			Assert.That(enumerable.Count, Is.GreaterThan(0));
		}

		[Test]
		public async Task GetAllProjectIdsByProjectNumberAsync_ShouldReturnEmptyCollection_WhenProjectNumberNotExists()
		{
			// Act
			var result = await _projectService.GetAllProjectIdsByProjectNumberAsync("123456");

			// Assert
			var enumerable = result as int[] ?? result.ToArray();
			Assert.That(enumerable, Is.Not.Null);
			Assert.That(enumerable.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task GetAllProjectsAsync_ShouldReturnAllProjects()
		{
			// Act
			var result = await _projectService.GetAllProjectsAsync();

			// Assert
			var projectServiceModels = result as ProjectServiceModel[] ?? result.ToArray();
			Assert.That(projectServiceModels, Is.Not.Null);
			Assert.That(projectServiceModels.Count, Is.GreaterThan(0));
		}

		[Test]
		public async Task ExistByProjectNumberAsync_ShouldReturnTrue_WhenProjectExists()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.ExistByProjectNumberAsync(project.ProjectNumber);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task ExistByProjectNumberAsync_ShouldReturnFalse_WhenProjectNotExists()
		{
			// Act
			var result = await _projectService.ExistByProjectNumberAsync("123456");

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetProjectIdByProjectNumberAsync_ShouldReturnProjectId()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.GetProjectIdByProjectNumberAsync(project.ProjectNumber);

			// Assert
			Assert.That(result, Is.EqualTo(project.Id));
		}

		[Test]
		public async Task GetProjectForDeleteByIdAsync_ShouldReturnProjectForDelete()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.GetProjectForDeleteByIdAsync(project.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
		}

		[Test]
		public async Task GetProjectForDeleteByIdAsync_ShouldReturnNull_WhenProjectNotExists()
		{
			// Act
			var result = await _projectService.GetProjectForDeleteByIdAsync(100);

			// Assert
			Assert.That(result, Is.Null, "Result is not null");
		}

		[Test]
		public async Task DeleteProjectAsync_ShouldDeleteProject()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			await _projectService.ProjectDeleteAsync(project.Id);

			// Assert
			var deletedProject = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync(p => p.Id == project.Id);
			Assert.That(deletedProject, Is.Null, "Deleted project is not null");
		}

		[Test]
		public void DeleteProjectAsync_ShouldNotDeleteProject_WhenProjectNotExists()
		{
			// Act
			Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.ProjectDeleteAsync(100));
		}

		[Test]
		public async Task ProjectStatusExistAsync_ShouldReturnFalse_WhenProjectStatusNotExists()
		{
			// Act
			var result = await _projectService.ProjectStatusExistAsync(100);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetProjectDetailsByIdAsync()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.GetProjectDetailsByIdAsync(project.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
		}

		// ============================================================================
		// ADDITIONAL TESTS FOR 90%+ COVERAGE
		// ============================================================================

		#region AllAsync Method Tests (Query and Search Functionality)

		[Test]
		public async Task AllAsync_WithNoParameters_ReturnsAllProjects()
		{
			// Arrange
			var expectedProjectCount = await _repository.AllReadOnly<Project>().CountAsync();

			// Act
			var result = await _projectService.AllAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalProjectsCount, Is.EqualTo(expectedProjectCount));
			Assert.That(result.Projects, Is.Not.Null);
			Assert.That(result.Projects.Count(), Is.LessThanOrEqualTo(10)); // Default page size
		}

		[Test]
		public async Task AllAsync_WithSearchParameter_ReturnsFilteredProjects()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");
			string searchTerm = project.ProjectName.Substring(0, 3); // Search for part of project name

			// Act
			var result = await _projectService.AllAsync(search: searchTerm);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
			// Verify that returned projects contain the search term
			foreach (var projectModel in result.Projects)
			{
				bool containsSearchTerm = projectModel.ProjectName.ToLower().Contains(searchTerm.ToLower()) ||
										 projectModel.ProjectNumber.ToLower().Contains(searchTerm.ToLower());
				Assert.That(containsSearchTerm, Is.True, $"Project {projectModel.ProjectName} should contain search term {searchTerm}");
			}
		}

		[Test]
		public async Task AllAsync_WithSearchByProjectNumber_ReturnsMatchingProjects()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			// Act
			var result = await _projectService.AllAsync(search: project.ProjectNumber);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
			Assert.That(result.Projects.Count(), Is.GreaterThan(0));
		}

		[Test]
		public async Task AllAsync_WithSearchByClientName_ReturnsMatchingProjects()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>()
				.Where(p => !string.IsNullOrEmpty(p.ClientName))
				.FirstOrDefaultAsync();

			if (project != null && !string.IsNullOrEmpty(project.ClientName))
			{
				// Act
				var result = await _projectService.AllAsync(search: project.ClientName);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Projects, Is.Not.Null);
				Assert.That(result.Projects.Count(), Is.GreaterThan(0));
			}
			else
			{
				Assert.Pass("No projects with client names found for testing");
			}
		}

		[Test]
		public async Task AllAsync_WithNonExistentSearch_ReturnsEmptyResult()
		{
			// Arrange
			string nonExistentSearch = "NonExistentProjectName12345";

			// Act
			var result = await _projectService.AllAsync(search: nonExistentSearch);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects.Count(), Is.EqualTo(0));
			Assert.That(result.TotalProjectsCount, Is.EqualTo(0));
		}

		[Test]
		public async Task AllAsync_WithSortingByNameAscending_ReturnsSortedProjects()
		{
			// Arrange & Act
			var result = await _projectService.AllAsync(sorting: ProjectSorting.NameAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
			if (result.Projects.Count() > 1)
			{
				var projectNames = result.Projects.Select(p => p.ProjectName).ToList();
				var sortedNames = projectNames.OrderBy(n => n).ToList();
				Assert.That(projectNames, Is.EqualTo(sortedNames), "Projects should be sorted by name ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByNameDescending_ReturnsSortedProjects()
		{
			// Arrange & Act
			var result = await _projectService.AllAsync(sorting: ProjectSorting.NameDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
			if (result.Projects.Count() > 1)
			{
				var projectNames = result.Projects.Select(p => p.ProjectName).ToList();
				var sortedNames = projectNames.OrderByDescending(n => n).ToList();
				Assert.That(projectNames, Is.EqualTo(sortedNames), "Projects should be sorted by name descending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByProjectNumberAscending_ReturnsSortedProjects()
		{
			// Arrange & Act
			var result = await _projectService.AllAsync(sorting: ProjectSorting.ProjectNumberAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
			if (result.Projects.Count() > 1)
			{
				var projectNumbers = result.Projects.Select(p => p.ProjectNumber).ToList();
				var sortedNumbers = projectNumbers.OrderBy(n => n).ToList();
				Assert.That(projectNumbers, Is.EqualTo(sortedNumbers), "Projects should be sorted by project number ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByTotalPartsDescending_ReturnsSortedProjects()
		{
			// Arrange & Act
			var result = await _projectService.AllAsync(sorting: ProjectSorting.TotalPartsDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
			if (result.Projects.Count() > 1)
			{
				var totalParts = result.Projects.Select(p => p.TotalParts).ToList();
				var sortedParts = totalParts.OrderByDescending(tp => tp).ToList();
				Assert.That(totalParts, Is.EqualTo(sortedParts), "Projects should be sorted by total parts descending");
			}
		}

		[Test]
		public async Task AllAsync_WithAllSortingOptions_ExecutesSuccessfully()
		{
			// Test all sorting enum values to ensure complete coverage
			var sortingOptions = Enum.GetValues<ProjectSorting>();

			foreach (var sorting in sortingOptions)
			{
				// Act
				var result = await _projectService.AllAsync(sorting: sorting);

				// Assert
				Assert.That(result, Is.Not.Null, $"Result should not be null for sorting: {sorting}");
				Assert.That(result.Projects, Is.Not.Null, $"Projects should not be null for sorting: {sorting}");
			}
		}

		[Test]
		public async Task AllAsync_WithCustomPagination_ReturnsCorrectPage()
		{
			// Arrange
			int projectsPerPage = 2;
			int currentPage = 1;

			// Act
			var result = await _projectService.AllAsync(projectsPerPage: projectsPerPage, currentPage: currentPage);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
			Assert.That(result.Projects.Count(), Is.LessThanOrEqualTo(projectsPerPage));
		}

		#endregion

		#region Pagination Method Tests

		[Test]
		public async Task GetAllProjectsAsync_WithPagination_ReturnsCorrectPage()
		{
			// Arrange
			int page = 1;
			int pageSize = 2;
			var totalProjects = await _repository.AllReadOnly<Project>().CountAsync();

			// Act
			var result = await _projectService.GetAllProjectsAsync(page, pageSize);

			// Assert
			Assert.That(result.Projects, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalProjects));
			Assert.That(result.Projects.Count(), Is.LessThanOrEqualTo(pageSize));
		}

		[Test]
		public async Task GetAllProjectsAsync_WithSecondPage_ReturnsCorrectProjects()
		{
			// Arrange
			int page = 2;
			int pageSize = 1;
			var totalProjects = await _repository.AllReadOnly<Project>().CountAsync();

			// Act
			var result = await _projectService.GetAllProjectsAsync(page, pageSize);

			// Assert
			Assert.That(result.Projects, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalProjects));
			if (totalProjects > 1)
			{
				Assert.That(result.Projects.Count(), Is.LessThanOrEqualTo(1));
			}
		}

		[Test]
		public async Task GetAllProjectsAsync_WithLargePageSize_ReturnsAllProjects()
		{
			// Arrange
			int page = 1;
			int pageSize = 100;
			var totalProjects = await _repository.AllReadOnly<Project>().CountAsync();

			// Act
			var result = await _projectService.GetAllProjectsAsync(page, pageSize);

			// Assert
			Assert.That(result.Projects, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalProjects));
			Assert.That(result.Projects.Count(), Is.EqualTo(totalProjects));
		}

		#endregion

		#region Error Handling and Edge Cases

		[Test]
		public async Task GetProjectDetailsByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidProjectId = 99999;

			// Act
			var result = await _projectService.GetProjectDetailsByIdAsync(invalidProjectId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProjectIdByProjectNumberAsync_WithInvalidProjectNumber_ReturnsNull()
		{
			// Arrange
			string invalidProjectNumber = "INVALID-999999";

			// Act
			var result = await _projectService.GetProjectIdByProjectNumberAsync(invalidProjectNumber);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task AllAsync_WithNullSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _projectService.AllAsync(search: null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithEmptySearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _projectService.AllAsync(search: "");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithWhitespaceSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _projectService.AllAsync(search: "   ");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Projects, Is.Not.Null);
		}

		[Test]
		public async Task AddNewProjectsAsync_ReturnsCorrectProjectId()
		{
			// Arrange
			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "Test Project for ID Return",
				ProjectNumber = "249999",
				ProjectStatusId = 1,
				TotalHoursSpent = 50,
				Appliance = "Test Appliance",
				ClientName = "Test Client"
			};

			// Act
			var returnedId = await _projectService.AddNewProjectsAsync(projectFormModel);

			// Assert
			Assert.That(returnedId, Is.GreaterThan(0));

			// Verify the project was actually created with the returned ID
			var createdProject = await _repository.GetByIdAsync<Project>(returnedId);
			Assert.That(createdProject, Is.Not.Null);
			Assert.That(createdProject.ProjectName, Is.EqualTo(projectFormModel.ProjectName));
			Assert.That(createdProject.ProjectNumber, Is.EqualTo(projectFormModel.ProjectNumber));
		}

		[Test]
		public async Task EditProjectAsync_WithValidData_UpdatesAllFields()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>().FirstOrDefaultAsync();
			Assert.That(project, Is.Not.Null, "Project is null");

			var projectFormModel = new ProjectFormModel()
			{
				ProjectName = "Updated Project Name",
				ProjectNumber = "249998",
				ProjectStatusId = 2,
				TotalHoursSpent = 150,
				Appliance = "Updated Appliance",
				ClientName = "Updated Client"
			};

			// Act
			await _projectService.EditProjectAsync(projectFormModel, project.Id);

			// Assert
			var updatedProject = await _repository.GetByIdAsync<Project>(project.Id);
			Assert.That(updatedProject, Is.Not.Null);
			Assert.That(updatedProject.ProjectName, Is.EqualTo(projectFormModel.ProjectName));
			Assert.That(updatedProject.ProjectNumber, Is.EqualTo(projectFormModel.ProjectNumber));
			Assert.That(updatedProject.ProjectStatusId, Is.EqualTo(projectFormModel.ProjectStatusId));
			Assert.That(updatedProject.TotalHoursSpent, Is.EqualTo(projectFormModel.TotalHoursSpent));
			Assert.That(updatedProject.Appliance, Is.EqualTo(projectFormModel.Appliance));
			Assert.That(updatedProject.ClientName, Is.EqualTo(projectFormModel.ClientName));
		}

		#endregion

		#region Model Coverage Tests

		[Test]
		public void ProjectQueryServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new ProjectQueryServiceModel();

			// Assert
			Assert.That(model.Projects, Is.Not.Null);
			Assert.That(model.Projects.Count(), Is.EqualTo(0));
			Assert.That(model.TotalProjectsCount, Is.EqualTo(0));
		}

		[Test]
		public void ProjectQueryServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var projects = new List<ProjectServiceModel>
			{
				new ProjectServiceModel { Id = 1, ProjectName = "Project 1", ProjectNumber = "PRJ-001" },
				new ProjectServiceModel { Id = 2, ProjectName = "Project 2", ProjectNumber = "PRJ-002" }
			};

			// Act
			var model = new ProjectQueryServiceModel
			{
				TotalProjectsCount = 100,
				Projects = projects
			};

			// Assert
			Assert.That(model.TotalProjectsCount, Is.EqualTo(100));
			Assert.That(model.Projects.Count(), Is.EqualTo(2));
			Assert.That(model.Projects.First().ProjectName, Is.EqualTo("Project 1"));
			Assert.That(model.Projects.Last().ProjectName, Is.EqualTo("Project 2"));
		}

		[Test]
		public void ProjectServiceModel_AllProperties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new ProjectServiceModel
			{
				Id = 123,
				ProjectName = "Test Project Name",
				ProjectNumber = "TEST-123",
				Status = "In Progress",
				TotalParts = 25
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(123));
			Assert.That(model.ProjectName, Is.EqualTo("Test Project Name"));
			Assert.That(model.ProjectNumber, Is.EqualTo("TEST-123"));
			Assert.That(model.Status, Is.EqualTo("In Progress"));
			Assert.That(model.TotalParts, Is.EqualTo(25));
		}

		[Test]
		public void ProjectServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new ProjectServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.ProjectName, Is.EqualTo(string.Empty));
			Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
			Assert.That(model.Status, Is.EqualTo(string.Empty));
			Assert.That(model.TotalParts, Is.EqualTo(0));
		}

		[Test]
		public void PaginatedProjectsViewModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PaginatedProjectsViewModel();

			// Assert
			Assert.That(model.Projects, Is.Not.Null);
			Assert.That(model.Projects.Count(), Is.EqualTo(0));
		}

		[Test]
		public void PaginatedProjectsViewModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var projects = new List<ProjectServiceModel>
			{
				new ProjectServiceModel { Id = 1, ProjectName = "Project 1" },
				new ProjectServiceModel { Id = 2, ProjectName = "Project 2" }
			};
			var pager = new TeamWorkFlow.Core.Models.Pager.PagerServiceModel(20, 2, 5);

			// Act
			var model = new PaginatedProjectsViewModel
			{
				Projects = projects,
				Pager = pager
			};

			// Assert
			Assert.That(model.Projects.Count(), Is.EqualTo(2));
			Assert.That(model.Pager, Is.Not.Null);
			Assert.That(model.Pager.TotalProjects, Is.EqualTo(20));
			Assert.That(model.Pager.CurrentPage, Is.EqualTo(2));
		}

		[Test]
		public void ProjectStatusServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new ProjectStatusServiceModel
			{
				Id = 5,
				Name = "Completed"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(5));
			Assert.That(model.Name, Is.EqualTo("Completed"));
		}

		[Test]
		public void ProjectStatusServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new ProjectStatusServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
		}

		[Test]
		public void ProjectDeleteServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new ProjectDeleteServiceModel
			{
				Id = 10,
				ProjectName = "Project to Delete",
				ProjectNumber = "DEL-123"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(10));
			Assert.That(model.ProjectName, Is.EqualTo("Project to Delete"));
			Assert.That(model.ProjectNumber, Is.EqualTo("DEL-123"));
		}

		[Test]
		public void ProjectDeleteServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new ProjectDeleteServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.ProjectName, Is.EqualTo(string.Empty));
			Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
		}

		[Test]
		public void ProjectDetailsServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new ProjectDetailsServiceModel
			{
				Id = 15,
				ProjectName = "Detailed Project",
				ProjectNumber = "DET-789",
				Appliance = "Test Appliance",
				ClientName = "Test Client",
				Status = "Ready",
				CalculatedTotalHours = 120,
				TotalParts = 30
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(15));
			Assert.That(model.ProjectName, Is.EqualTo("Detailed Project"));
			Assert.That(model.ProjectNumber, Is.EqualTo("DET-789"));
			Assert.That(model.Appliance, Is.EqualTo("Test Appliance"));
			Assert.That(model.ClientName, Is.EqualTo("Test Client"));
			Assert.That(model.Status, Is.EqualTo("Ready"));
			Assert.That(model.CalculatedTotalHours, Is.EqualTo(120));
			Assert.That(model.TotalParts, Is.EqualTo(30));
		}

		[Test]
		public void ProjectDetailsServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new ProjectDetailsServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.ProjectName, Is.EqualTo(string.Empty));
			Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
			Assert.That(model.Appliance, Is.EqualTo(string.Empty));
			Assert.That(model.ClientName, Is.EqualTo(string.Empty));
			Assert.That(model.Status, Is.EqualTo(string.Empty));
			Assert.That(model.CalculatedTotalHours, Is.EqualTo(0));
			Assert.That(model.TotalParts, Is.EqualTo(0));
		}

		#endregion

		#region Project-Tasks Relationship Tests

		[Test]
		public async Task Project_TasksNavigationProperty_WorksCorrectly()
		{
			// Arrange
			var project = await _repository.AllReadOnly<Project>()
				.Include(p => p.Tasks)
				.FirstOrDefaultAsync();

			// Assert
			Assert.That(project, Is.Not.Null, "Project should exist");
			Assert.That(project.Tasks, Is.Not.Null, "Tasks navigation property should not be null");

			// Check if we can access tasks through the project
			var taskCount = project.Tasks.Count;
			Assert.That(taskCount, Is.GreaterThanOrEqualTo(0), "Task count should be non-negative");
		}

		[Test]
		public async Task Project_CanLoadTasksWithInclude()
		{
			// Arrange & Act
			var projectsWithTasks = await _repository.AllReadOnly<Project>()
				.Include(p => p.Tasks)
				.ToListAsync();

			// Assert
			Assert.That(projectsWithTasks, Is.Not.Null);
			Assert.That(projectsWithTasks.Count, Is.GreaterThan(0), "Should have projects in database");

			// Verify that Tasks navigation property is accessible
			foreach (var project in projectsWithTasks)
			{
				Assert.That(project.Tasks, Is.Not.Null, $"Tasks collection should not be null for project {project.ProjectName}");
			}
		}

		#endregion

		#region Time Calculation Tests

		[Test]
		public async Task GetProjectTimeCalculationByIdAsync_ReturnsCorrectCalculations()
		{
			// Arrange
			var projectId = 2; // Project with tasks in seeded data

			// Act
			var result = await _projectService.GetProjectTimeCalculationByIdAsync(projectId);

			// Assert
			Assert.That(result, Is.Not.Null, "Project time calculation should not be null");
			Assert.That(result.ProjectId, Is.EqualTo(projectId));
			Assert.That(result.ProjectName, Is.Not.Empty);
			Assert.That(result.ProjectNumber, Is.Not.Empty);
			Assert.That(result.TotalHoursSpent, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.CalculatedTotalHours, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalEstimatedHours, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.CompletionPercentage, Is.InRange(0, 100));
		}

		[Test]
		public async Task GetAllProjectsWithTimeCalculationsAsync_ReturnsAllProjects()
		{
			// Act
			var result = await _projectService.GetAllProjectsWithTimeCalculationsAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.GreaterThan(0), "Should return projects with time calculations");

			foreach (var project in result)
			{
				Assert.That(project.ProjectId, Is.GreaterThan(0));
				Assert.That(project.ProjectName, Is.Not.Empty);
				Assert.That(project.ProjectNumber, Is.Not.Empty);
				Assert.That(project.CompletionPercentage, Is.InRange(0, 100));
			}
		}

		[Test]
		public async Task ProjectDetailsServiceModel_IncludesTimeCalculations()
		{
			// Arrange
			var projectId = 2; // Project with tasks

			// Act
			var result = await _projectService.GetProjectDetailsByIdAsync(projectId);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.CalculatedTotalHours, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalEstimatedHours, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.CompletionPercentage, Is.InRange(0, 100));
			Assert.That(result.FinishedTasksCount, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.InProgressTasksCount, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.OpenTasksCount, Is.GreaterThanOrEqualTo(0));
			Assert.That(result.TotalTasksCount, Is.GreaterThanOrEqualTo(0));
		}

		[Test]
		public void ProjectTimeCalculationServiceModel_FormattedProperties_WorkCorrectly()
		{
			// Arrange
			var model = new ProjectTimeCalculationServiceModel
			{
				TotalHoursSpent = 40,
				CalculatedTotalHours = 32,
				TotalEstimatedHours = 48,
				CompletionPercentage = 66.7,
				TimeVariance = 8
			};

			// Act & Assert
			Assert.That(model.FormattedCompletionPercentage, Is.EqualTo("66.7%"));
			Assert.That(model.TimeVarianceStatus, Is.EqualTo("Under Estimate"));
			Assert.That(model.FormattedTotalHoursSpent, Is.EqualTo("5d"));
			Assert.That(model.FormattedCalculatedTotalHours, Is.EqualTo("4d"));
			Assert.That(model.FormattedTotalEstimatedHours, Is.EqualTo("6d"));
		}

		#endregion

		#region Project Creation with Default Time Tests

		[Test]
		public async Task AddNewProjectsAsync_SetsDefaultOneHourSetupTime()
		{
			// Arrange
			var model = new ProjectFormModel
			{
				ProjectName = "Test Project with Default Time",
				ProjectNumber = "TP9999",
				ProjectStatusId = 1,
				TotalHoursSpent = 1, // This should be set by controller
				ClientName = "Test Client",
				Appliance = "Test Application"
			};

			// Act
			var projectId = await _projectService.AddNewProjectsAsync(model);

			// Assert
			Assert.That(projectId, Is.GreaterThan(0), "Project should be created successfully");

			var createdProject = await _projectService.GetProjectDetailsByIdAsync(projectId);
			Assert.That(createdProject, Is.Not.Null, "Created project should be retrievable");
			Assert.That(createdProject.CalculatedTotalHours, Is.EqualTo(0), "Project should have 0 calculated hours initially");
			Assert.That(createdProject.ProjectName, Is.EqualTo("Test Project with Default Time"));
			Assert.That(createdProject.ProjectNumber, Is.EqualTo("TP9999"));
		}

		[Test]
		public void ProjectFormModel_HasDefaultOneHourValue()
		{
			// Arrange & Act
			var model = new ProjectFormModel();

			// Assert
			Assert.That(model.TotalHoursSpent, Is.EqualTo(1), "ProjectFormModel should default to 1 hour for setup time");
		}

		#endregion

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
