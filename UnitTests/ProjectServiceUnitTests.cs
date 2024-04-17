using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
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
		private IProjectService _projectService;
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
				.AddUserSecrets<ProjectServiceUnitTests>()
				.Build();

			var connectionString = configuration.GetConnectionString("Test");

			var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			_dbContext = new TeamWorkFlowDbContext(options);
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
		public async Task EditProjectAsync_ShouldNotEditProject_WhenProjectNotExists()
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
		public async Task DeleteProjectAsync_ShouldNotDeleteProject_WhenProjectNotExists()
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


		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
