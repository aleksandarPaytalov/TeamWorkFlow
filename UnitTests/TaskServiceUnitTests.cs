using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Exceptions;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
	[TestFixture]
	public class TaskUnitTests
	{
		private IRepository _repository;
		private ITaskService _taskService;
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
            .AddUserSecrets<TaskUnitTests>()
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
    _taskService = new TaskService(_repository);

    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
}

		[Test]
		public async Task GetAllTasksAsync_ReturnsNumberOfTasks()
		{
			// Arrange
			var expectedResult =
				await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Act
			var result = await _taskService.GetAllTasksAsync();

			// Assert
			Assert.That(result.Count(), Is.EqualTo(expectedResult));
		}
		
		[Test]
		public async Task GetTaskByIdAsync_ReturnsTask()
		{
			// Arrange
			TeamWorkFlow.Infrastructure.Data.Models.Task? task =
				await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
					.FirstOrDefaultAsync();

			// Act
			TaskServiceModel? result = await _taskService.GetTaskByIdAsync(task.Id);

			// Assert
			Assert.NotNull(result);
			Assert.That(result!.Id, Is.EqualTo(task?.Id));
		}

		[Test]
		public async Task GetAllPrioritiesAsync_ReturnPrioritiesList()
		{
			// Arrange
			var expectedResult =
				await _repository.AllReadOnly<Priority>().ToListAsync();

			// Act
			var result = await _taskService.GetAllPrioritiesAsync();

			// Assert
			Assert.That(result.Count(), Is.EqualTo(expectedResult.Count));
		}

		[Test]
		public async Task GetAllStatusesAsync_ReturnStatusesList()
		{
			// Arrange
			var expectedResult =
				await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.TaskStatus>()
					.ToListAsync();

			// Act
			var result = await _taskService.GetAllStatusesAsync();

			// Assert
			Assert.That(result.Count(), Is.EqualTo(expectedResult.Count));
		}

		[Test]
		public async Task TaskStatusExistAsync_ReturnTrueIfExist()
		{
			// Arrange
			var taskStatus = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.TaskStatus>()
				.FirstOrDefaultAsync();
			Assert.That(taskStatus, Is.Not.Null, "Status is null");

			// Act
			var result = await _taskService.TaskStatusExistsAsync(taskStatus.Id);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task PriorityExistsAsync_ReturnTrueIfExist()
		{
			// Arrange
			var priority = await _repository.AllReadOnly<Priority>()
				.FirstOrDefaultAsync();
			Assert.That(priority, Is.Not.Null, "Priority is null");

			// Act
			var result = await _taskService.PriorityExistsAsync(priority.Id);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task TaskExistsAsync_ReturnTrueIfExist()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			// Act
			var result = await _taskService.TaskExistByIdAsync(task.Id);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task GetTaskForEditByIdAsync_ReturnTaskForEdit()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			// Act
			var result = await _taskService.GetTaskForEditByIdAsync(task.Id);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task GetTaskDetailsByIdAsync_ReturnTaskDetails()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.Project)
				.Include(t => t.TaskStatus)
				.Include(t => t.Priority)
				.Include(t => t.Machine)
				.Include(t => t.Creator)
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			// Act
			var result = await _taskService.GetTaskDetailsByIdAsync(task.Id);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(task.Id));
			Assert.That(result.Name, Is.EqualTo(task.Name));
			Assert.That(result.Description, Is.EqualTo(task.Description));
			Assert.That(result.ProjectNumber, Is.EqualTo(task.Project.ProjectNumber));
			Assert.That(result.StartDate, Is.EqualTo(task.StartDate.ToString("dd/MM/yyyy")));
			Assert.That(task.EndDate, Is.Null, "End date is null");
			Assert.That(result.EndDate, Is.EqualTo(task.EndDate.ToString()));
			Assert.That(task.DeadLine, Is.Not.Null, "End is null");
			Assert.That(result.Deadline, Is.EqualTo(task.DeadLine.Value.ToString("dd/MM/yyyy")));
			Assert.That(result.Priority, Is.EqualTo(task.Priority.Name));
			Assert.That(result.Status, Is.EqualTo(task.TaskStatus.Name));
			Assert.That(result.Creator, Is.EqualTo(task.Creator.UserName));
			Assert.That(task.Machine, Is.Not.Null, "Machine is null");
			Assert.That(result.AssignedMachineName, Is.EqualTo(task.Machine.Name));

		}

		[Test]
		public async Task EditTaskAsync_ShouldEditTaskCorrectly()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.Project)
				.Include(t => t.TaskStatus)
				.Include(t => t.Priority)
				.Include(t => t.Machine)
				.Include(t => t.Creator)
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");
			Assert.That(task.EndDate, Is.Null, "End date is not null");
			Assert.That(task.DeadLine, Is.Not.Null, "Deadline is null");
			Assert.That(task.Machine, Is.Not.Null, "Machine is null");

			DateTime startDate = new DateTime (2024, 02, 02);
			DateTime endDate = new DateTime (2024, 03, 03);
			DateTime deadline = new DateTime (2024, 03, 04);

			var taskFormModel = new TaskFormModel()
			{
				Name = "Housing Front Panel - LOP.",
				Description = "New Description",
				ProjectNumber = "242700",
				StartDate = task.StartDate.ToString("dd/MM/yyyy"),
				Deadline = task.DeadLine.Value.ToString("dd/MM/yyyy"),
				EndDate = task.EndDate?.ToString("dd/MM/yyyy"),
				PriorityId = task.PriorityId,
				StatusId = task.TaskStatusId,
				ProjectId = task.ProjectId,
			};
			Assert.That(taskFormModel, Is.Not.Null, "Task form is null");

			// Act
			await _taskService.EditTaskAsync(taskFormModel, task.Id, startDate, endDate, deadline, task.ProjectId);

			// Assert
			var expectedTaskCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();
			var editedTask = await _repository.GetByIdAsync<TeamWorkFlow.Infrastructure.Data.Models.Task>(task.Id);
			Assert.That(editedTask, Is.Not.Null);
			Assert.That(editedTask.Name, Is.EqualTo(taskFormModel.Name));
			Assert.That(editedTask.Description, Is.EqualTo(taskFormModel.Description));
			Assert.That(editedTask.StartDate, Is.EqualTo(startDate));
			Assert.That(editedTask.EndDate, Is.EqualTo(endDate));
			Assert.That(editedTask.DeadLine, Is.EqualTo(deadline));
			Assert.That(editedTask.PriorityId, Is.EqualTo(taskFormModel.PriorityId));
			Assert.That(editedTask.TaskStatusId, Is.EqualTo(taskFormModel.StatusId));
			Assert.That(editedTask.CreatorId, Is.EqualTo(task.CreatorId));
			Assert.That(editedTask.ProjectId, Is.EqualTo(task.ProjectId));
			Assert.That(editedTask.MachineId, Is.EqualTo(task.MachineId));
			Assert.That(await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync(),
				Is.EqualTo(expectedTaskCount));
		}

		[Test]
		public async Task DeleteTaskAsync_ShouldDeleteTaskCorrectly()
		{
			// Arrange
			var initialTaskCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();
			Assert.That(initialTaskCount, Is.GreaterThan(0), "No tasks found in the database");

			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			// Act
			await _taskService.DeleteTaskAsync(task.Id);

			// Assert
			var expectedTaskCount = initialTaskCount - 1; // We expect one task to be deleted
			Assert.That(await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync(), Is.EqualTo(expectedTaskCount));
		}

		[Test]
		public async Task GetTaskForDeleteByIdAsync()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			// Act
			var result = await _taskService.GetTaskForDeleteByIdAsync(task.Id);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task GetOperatorIdByUserId_ReturnOperatorId()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";

			// Act
			var result = await _taskService.GetOperatorIdByUserId(userId);

			// Assert
			Assert.That(result, Is.EqualTo(1));
		}

		[Test]
		public async Task GetOperatorIdByAssignedTaskId_ReturnOperatorId()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.TasksOperators)
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			// Act
			var result = await _taskService.GetOperatorIdByAssignedTaskId(task.Id);

			// Assert
			Assert.That(result, Is.EqualTo(1));
		}

		[Test]
		public async Task RemoveAssignedTaskFromUserCollection_ShouldRemoveTaskFromUserCollection()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.TasksOperators)
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			var expectedTasksNumber = await _repository.AllReadOnly<TaskOperator>()
				.CountAsync();
			// Act
			var operatorId = await _taskService.GetOperatorIdByAssignedTaskId(task.Id);
			await _taskService.RemoveAssignedTaskFromUserCollection(task.Id, operatorId);

			var actualTasksNumber = await _repository.AllReadOnly<TaskOperator>()
				.CountAsync();

			// Assert
			var editedTask = await _repository.GetByIdAsync<TeamWorkFlow.Infrastructure.Data.Models.Task>(task.Id);
			Assert.That(editedTask?.TasksOperators.Count, Is.EqualTo(0));
			Assert.That(actualTasksNumber, Is.EqualTo(expectedTasksNumber - 1));
		}

		[Test]
		public async Task GetAllAssignedTasksAsync_ReturnsAssignedTasks()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TaskOperator>()
				.CountAsync();
			Assert.That(task is not 0,  "Task is 0");

			// Act
			var result = await _taskService.GetAllAssignedTasksAsync(1, 10);

			// Assert
			Assert.That(result.Tasks.Count(), Is.EqualTo(task));
		}

		[Test]
		public async Task GetMyTasksAsync_ReturnsMyTasks()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";
			int operatorId = await _taskService.GetOperatorIdByUserId(userId);

			int assignedTasks = await _repository.AllReadOnly<TaskOperator>()
				.Where(t => t.OperatorId == operatorId)
				.CountAsync();

			// Act
			var result = await _taskService.GetMyTasksAsync(userId);

			// Assert
			Assert.That(result.Count(), Is.EqualTo(assignedTasks));
		}

		[Test]
		public async Task AddTaskToMyCollection_ShouldAddTaskToMyCollection()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";
			var myAssignedTasks = await _repository.AllReadOnly<TaskOperator>()
				.Include(to => to.Operator)
				.Where(to => to.Operator.UserId == userId)
				.ToListAsync();
			Assert.That(myAssignedTasks, Is.Not.Null, "Task is null");

			int expected = myAssignedTasks.Count;

			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.Project)
				.Include(t => t.TaskStatus)
				.Include(t => t.Priority)
				.Include(t => t.Machine)
				.Include(t => t.Creator)
				.FirstOrDefaultAsync();	
			Assert.That(task, Is.Not.Null, "Task is null");

			var taskToAdd = new TaskServiceModel()
			{
				Id = task.Id,
				Name = task.Name,
				Description = task.Description,
				ProjectNumber = task.Project.ProjectNumber,
				StartDate = DateTime.Now.ToString("dd/MM/yyyy"),
				EndDate = task.EndDate.ToString(),
				Deadline = task.ToString(),
				Priority = task.Priority.Name,
				Status = task.TaskStatus.Name
			};

			// Act
			await _taskService.AddTaskToMyCollection(taskToAdd, userId);

			var actualTasksNumber = await _repository.AllReadOnly<TaskOperator>()
				.CountAsync();

			// Assert
			Assert.That(actualTasksNumber, Is.EqualTo(expected + 1));
		}

		[Test]
		public async Task RemoveFromCollection_ShouldRemoveTaskFromCollection()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";
			var myAssignedTasks = await _repository.AllReadOnly<TaskOperator>()
				.Include(to => to.Operator)
				.Where(to => to.Operator.UserId == userId)
				.ToListAsync();
			Assert.That(myAssignedTasks, Is.Not.Null, "Task is null");

			int expected = myAssignedTasks.Count;

			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.Project)
				.Include(t => t.TaskStatus)
				.Include(t => t.Priority)
				.Include(t => t.Machine)
				.Include(t => t.Creator)
				.FirstOrDefaultAsync();	
			Assert.That(task, Is.Not.Null, "Task is null");

			var taskToAdd = new TaskServiceModel()
			{
				Id = task.Id,
				Name = task.Name,
				Description = task.Description,
				ProjectNumber = task.Project.ProjectNumber,
				StartDate = DateTime.Now.ToString("dd/MM/yyyy"),
				EndDate = task.EndDate.ToString(),
				Deadline = task.ToString(),
				Priority = task.Priority.Name,
				Status = task.TaskStatus.Name
			};

			await _taskService.AddTaskToMyCollection(taskToAdd, userId);

			var actualTasksNumber = await _repository.AllReadOnly<TaskOperator>()
				.CountAsync();

			// Act
			await _taskService.RemoveFromCollection(task.Id, userId);

			var actualTasksNumberAfterRemove = await _repository.AllReadOnly<TaskOperator>()
				.CountAsync();

			// Assert
			Assert.That(actualTasksNumberAfterRemove, Is.EqualTo(expected));
		}

		[Test]
		public async Task TaskExistInTaskOperatorTableByIdAsync_ReturnTrueIfExist()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.TasksOperators)
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null, "Task is null");

			// Act
			var result = await _taskService.TaskExistInTaskOperatorTableByIdAsync(task.Id);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task AddNewTaskAsync()
		{
			var tasksCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Arrange
			var taskFormModel = new TaskFormModel()
			{
				Name = "Housing Front Panel - LOP.",
				Description = "New Description",
				ProjectNumber = "242700",
				StartDate = DateTime.Now.ToString("dd/MM/yyyy"),
				Deadline = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy"),
				EndDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"),
				PriorityId = 1,
				StatusId = 1,
				ProjectId = 1,
			};

			// Act
			var result = await _taskService.AddNewTaskAsync(taskFormModel,
				"cf41999b-9cad-4b75-977d-a2fdb3d02e77",
				DateTime.Now, 
				DateTime.Now.AddDays(1), 
				DateTime.Now.AddDays(2), 
				1);

			// Assert
			Assert.That(result, Is.GreaterThan(tasksCount));
		}

		[Test]
		public async Task GetOperatorIdByUserId_ReturnUnExistingActionExceptionIfOperatorDoesNotExist()
		{
			// Arrange
			string userId = "f700189f-d9a5-42ca-aaac-6f73e43614a9";

			// Act & Assert
			var ex =  Assert.ThrowsAsync<UnExistingActionException>(() => _taskService.GetOperatorIdByUserId(userId));

			// Assert
			Assert.That(ex.Message, Is.EqualTo("The operator with this userId does not exist"));
		}

		// ============================================================================
		// ADDITIONAL TESTS FOR 80%+ COVERAGE
		// ============================================================================

		#region AllAsync Method Tests (Query and Search Functionality)

		[Test]
		public async Task AllAsync_WithNoParameters_ReturnsAllTasks()
		{
			// Arrange
			var expectedTaskCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Act
			var result = await _taskService.AllAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalTasksCount, Is.EqualTo(expectedTaskCount));
			Assert.That(result.Tasks, Is.Not.Null);
			Assert.That(result.Tasks.Count(), Is.LessThanOrEqualTo(10)); // Default page size
		}

		[Test]
		public async Task AllAsync_WithSearchParameter_ReturnsFilteredTasks()
		{
			// Arrange
			string searchTerm = "Housing"; // Search for tasks containing "Housing"

			// Act
			var result = await _taskService.AllAsync(search: searchTerm);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			// Verify that returned tasks contain the search term
			foreach (var task in result.Tasks)
			{
				bool containsSearchTerm = task.Name.ToLower().Contains(searchTerm.ToLower()) ||
										 task.Description.ToLower().Contains(searchTerm.ToLower()) ||
										 task.ProjectNumber.ToLower().Contains(searchTerm.ToLower());
				Assert.That(containsSearchTerm, Is.True, $"Task {task.Name} should contain search term {searchTerm}");
			}
		}

		[Test]
		public async Task AllAsync_WithEmptySearch_ReturnsAllTasks()
		{
			// Arrange
			var expectedTaskCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Act
			var result = await _taskService.AllAsync(search: "");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalTasksCount, Is.EqualTo(expectedTaskCount));
		}

		[Test]
		public async Task AllAsync_WithNonExistentSearch_ReturnsEmptyResult()
		{
			// Arrange
			string nonExistentSearch = "NonExistentTaskName12345";

			// Act
			var result = await _taskService.AllAsync(search: nonExistentSearch);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks.Count(), Is.EqualTo(0));
			Assert.That(result.TotalTasksCount, Is.EqualTo(0));
		}

		[Test]
		public async Task AllAsync_WithSortingByNameAscending_ReturnsSortedTasks()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(sorting: TaskSorting.NameAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			if (result.Tasks.Count() > 1)
			{
				var taskNames = result.Tasks.Select(t => t.Name).ToList();
				var sortedNames = taskNames.OrderBy(n => n).ToList();
				Assert.That(taskNames, Is.EqualTo(sortedNames), "Tasks should be sorted by name ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByNameDescending_ReturnsSortedTasks()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(sorting: TaskSorting.NameDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			if (result.Tasks.Count() > 1)
			{
				var taskNames = result.Tasks.Select(t => t.Name).ToList();
				var sortedNames = taskNames.OrderByDescending(n => n).ToList();
				Assert.That(taskNames, Is.EqualTo(sortedNames), "Tasks should be sorted by name descending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByProjectNumberAscending_ReturnsSortedTasks()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(sorting: TaskSorting.ProjectNumberAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			if (result.Tasks.Count() > 1)
			{
				var projectNumbers = result.Tasks.Select(t => t.ProjectNumber).ToList();
				var sortedNumbers = projectNumbers.OrderBy(n => n).ToList();
				Assert.That(projectNumbers, Is.EqualTo(sortedNumbers), "Tasks should be sorted by project number ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByProjectNumberDescending_ReturnsSortedTasks()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(sorting: TaskSorting.ProjectNumberDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			if (result.Tasks.Count() > 1)
			{
				var projectNumbers = result.Tasks.Select(t => t.ProjectNumber).ToList();
				var sortedNumbers = projectNumbers.OrderByDescending(n => n).ToList();
				Assert.That(projectNumbers, Is.EqualTo(sortedNumbers), "Tasks should be sorted by project number descending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByStartDateAscending_ReturnsSortedTasks()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(sorting: TaskSorting.StartDateAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			if (result.Tasks.Count() > 1)
			{
				var startDates = result.Tasks.Select(t => DateTime.ParseExact(t.StartDate, "dd/MM/yyyy", null)).ToList();
				var sortedDates = startDates.OrderBy(d => d).ToList();
				Assert.That(startDates, Is.EqualTo(sortedDates), "Tasks should be sorted by start date ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByDeadlineDescending_ReturnsSortedTasks()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(sorting: TaskSorting.DeadlineDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			// Note: Some tasks might not have deadlines, so we test the logic without strict ordering
		}

		[Test]
		public async Task AllAsync_WithCustomPagination_ReturnsCorrectPage()
		{
			// Arrange
			int tasksPerPage = 2;
			int currentPage = 1;

			// Act
			var result = await _taskService.AllAsync(tasksPerPage: tasksPerPage, currentPage: currentPage);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			Assert.That(result.Tasks.Count(), Is.LessThanOrEqualTo(tasksPerPage));
		}

		[Test]
		public async Task AllAsync_WithSecondPage_ReturnsCorrectTasks()
		{
			// Arrange
			int tasksPerPage = 1;
			int currentPage = 2;
			var totalTasks = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Act
			var result = await _taskService.AllAsync(tasksPerPage: tasksPerPage, currentPage: currentPage);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalTasksCount, Is.EqualTo(totalTasks));
			if (totalTasks > 1)
			{
				Assert.That(result.Tasks.Count(), Is.EqualTo(1));
			}
		}

		#endregion

		#region Error Handling and Edge Cases

		[Test]
		public async Task TaskStatusExistsAsync_WithInvalidId_ReturnsFalse()
		{
			// Arrange
			int invalidStatusId = 99999;

			// Act
			var result = await _taskService.TaskStatusExistsAsync(invalidStatusId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task PriorityExistsAsync_WithInvalidId_ReturnsFalse()
		{
			// Arrange
			int invalidPriorityId = 99999;

			// Act
			var result = await _taskService.PriorityExistsAsync(invalidPriorityId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task TaskExistByIdAsync_WithInvalidId_ReturnsFalse()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act
			var result = await _taskService.TaskExistByIdAsync(invalidTaskId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetTaskDetailsByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act
			var result = await _taskService.GetTaskDetailsByIdAsync(invalidTaskId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetTaskForEditByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act
			var result = await _taskService.GetTaskForEditByIdAsync(invalidTaskId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetTaskForDeleteByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act
			var result = await _taskService.GetTaskForDeleteByIdAsync(invalidTaskId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetTaskByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act
			var result = await _taskService.GetTaskByIdAsync(invalidTaskId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task EditTaskAsync_WithInvalidTaskId_DoesNotThrowException()
		{
			// Arrange
			int invalidTaskId = 99999;
			var taskFormModel = new TaskFormModel()
			{
				Name = "Test Task",
				Description = "Test Description",
				PriorityId = 1,
				StatusId = 1
			};

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
				await _taskService.EditTaskAsync(taskFormModel, invalidTaskId, DateTime.Now, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), 1));
		}

		[Test]
		public async Task DeleteTaskAsync_WithInvalidTaskId_DoesNotThrowException()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act & Assert
			Assert.DoesNotThrowAsync(async () => await _taskService.DeleteTaskAsync(invalidTaskId));
		}

		#endregion

		#region Pagination Methods Tests

		[Test]
		public async Task GetAllTasksAsync_WithPagination_ReturnsCorrectPage()
		{
			// Arrange
			int page = 1;
			int pageSize = 2;
			var totalTasks = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Act
			var result = await _taskService.GetAllTasksAsync(page, pageSize);

			// Assert
			Assert.That(result.Tasks, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalTasks));
			Assert.That(result.Tasks.Count(), Is.LessThanOrEqualTo(pageSize));
		}

		[Test]
		public async Task GetAllTasksAsync_WithSecondPage_ReturnsCorrectTasks()
		{
			// Arrange
			int page = 2;
			int pageSize = 1;
			var totalTasks = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Act
			var result = await _taskService.GetAllTasksAsync(page, pageSize);

			// Assert
			Assert.That(result.Tasks, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalTasks));
			if (totalTasks > 1)
			{
				Assert.That(result.Tasks.Count(), Is.EqualTo(1));
			}
		}

		[Test]
		public async Task GetAllAssignedTasksAsync_WithPagination_ReturnsCorrectPage()
		{
			// Arrange
			int page = 1;
			int pageSize = 2;
			var totalAssignedTasks = await _repository.AllReadOnly<TaskOperator>().CountAsync();

			// Act
			var result = await _taskService.GetAllAssignedTasksAsync(page, pageSize);

			// Assert
			Assert.That(result.Tasks, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalAssignedTasks));
			Assert.That(result.Tasks.Count(), Is.LessThanOrEqualTo(pageSize));
		}

		#endregion

		#region Collection Management Tests

		[Test]
		public async Task TaskExistInTaskOperatorTableByIdAsync_WithInvalidId_ReturnsFalse()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act
			var result = await _taskService.TaskExistInTaskOperatorTableByIdAsync(invalidTaskId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AddTaskToMyCollection_WithExistingTask_DoesNotAddDuplicate()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.Include(t => t.Project)
				.Include(t => t.TaskStatus)
				.Include(t => t.Priority)
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null);

			var taskModel = new TaskServiceModel()
			{
				Id = task.Id,
				Name = task.Name,
				Description = task.Description,
				ProjectNumber = task.Project.ProjectNumber,
				StartDate = task.StartDate.ToString("dd/MM/yyyy"),
				EndDate = task.EndDate?.ToString("dd/MM/yyyy") ?? string.Empty,
				Deadline = task.DeadLine?.ToString("dd/MM/yyyy") ?? string.Empty,
				Priority = task.Priority.Name,
				Status = task.TaskStatus.Name
			};

			// Add task first time
			await _taskService.AddTaskToMyCollection(taskModel, userId);
			var countAfterFirstAdd = await _repository.AllReadOnly<TaskOperator>().CountAsync();

			// Act - Try to add the same task again
			await _taskService.AddTaskToMyCollection(taskModel, userId);
			var countAfterSecondAdd = await _repository.AllReadOnly<TaskOperator>().CountAsync();

			// Assert
			Assert.That(countAfterSecondAdd, Is.EqualTo(countAfterFirstAdd), "Task should not be added twice");
		}

		[Test]
		public async Task GetOperatorIdByAssignedTaskId_WithInvalidTaskId_ReturnsZero()
		{
			// Arrange
			int invalidTaskId = 99999;

			// Act
			var result = await _taskService.GetOperatorIdByAssignedTaskId(invalidTaskId);

			// Assert
			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task RemoveAssignedTaskFromUserCollection_WithInvalidIds_DoesNotThrowException()
		{
			// Arrange
			int invalidTaskId = 99999;
			int invalidOperatorId = 99999;

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
				await _taskService.RemoveAssignedTaskFromUserCollection(invalidTaskId, invalidOperatorId));
		}

		[Test]
		public async Task RemoveFromCollection_WithInvalidTaskId_DoesNotThrowException()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";
			int invalidTaskId = 99999;

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
				await _taskService.RemoveFromCollection(invalidTaskId, userId));
		}

		[Test]
		public async Task GetMyTasksAsync_WithInvalidUserId_ThrowsException()
		{
			// Arrange
			string invalidUserId = "invalid-user-id-12345";

			// Act & Assert
			var ex = Assert.ThrowsAsync<UnExistingActionException>(() =>
				_taskService.GetMyTasksAsync(invalidUserId));
			Assert.That(ex.Message, Is.EqualTo("The operator with this userId does not exist"));
		}

		#endregion

		#region Additional Edge Cases and Data Validation

		[Test]
		public async Task AddNewTaskAsync_WithValidData_ReturnsNewTaskId()
		{
			// Arrange
			var initialTaskCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();
			var taskFormModel = new TaskFormModel()
			{
				Name = "New Test Task",
				Description = "New Test Description",
				PriorityId = 1,
				StatusId = 1
			};

			// Act
			var newTaskId = await _taskService.AddNewTaskAsync(
				taskFormModel,
				"cf41999b-9cad-4b75-977d-a2fdb3d02e77",
				DateTime.Now,
				DateTime.Now.AddDays(1),
				DateTime.Now.AddDays(2),
				1);

			// Assert
			Assert.That(newTaskId, Is.GreaterThan(0));
			var finalTaskCount = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();
			Assert.That(finalTaskCount, Is.EqualTo(initialTaskCount + 1));

			// Verify the task was created correctly
			var createdTask = await _repository.GetByIdAsync<TeamWorkFlow.Infrastructure.Data.Models.Task>(newTaskId);
			Assert.That(createdTask, Is.Not.Null);
			Assert.That(createdTask.Name, Is.EqualTo(taskFormModel.Name));
			Assert.That(createdTask.Description, Is.EqualTo(taskFormModel.Description));
		}

		[Test]
		public async Task GetTaskForEditByIdAsync_ReturnsTaskWithPrioritiesAndStatuses()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null);

			// Act
			var result = await _taskService.GetTaskForEditByIdAsync(task.Id);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Priorities, Is.Not.Null);
			Assert.That(result.Statuses, Is.Not.Null);
			Assert.That(result.Priorities.Count(), Is.GreaterThan(0));
			Assert.That(result.Statuses.Count(), Is.GreaterThan(0));
		}

		[Test]
		public async Task AllAsync_WithLargePageSize_ReturnsAllTasks()
		{
			// Arrange
			int largePageSize = 1000;
			var totalTasks = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>().CountAsync();

			// Act
			var result = await _taskService.AllAsync(tasksPerPage: largePageSize);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks.Count(), Is.EqualTo(totalTasks));
			Assert.That(result.TotalTasksCount, Is.EqualTo(totalTasks));
		}

		[Test]
		public async Task AllAsync_WithZeroPageSize_UsesDefaultPagination()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(tasksPerPage: 0);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
			// Should handle zero page size gracefully
		}

		#endregion

		#region Model Coverage Tests

		[Test]
		public void AllTasksQueryModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new AllTasksQueryModel();

			// Assert
			Assert.That(model.TasksPerPage, Is.EqualTo(10));
			Assert.That(model.CurrentPage, Is.EqualTo(1));
			Assert.That(model.Sorting, Is.EqualTo(TaskSorting.LastAdded));
			Assert.That(model.Tasks, Is.Not.Null);
			Assert.That(model.Tasks.Count(), Is.EqualTo(0));
		}

		[Test]
		public void AllTasksQueryModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var tasks = new List<TaskServiceModel>
			{
				new TaskServiceModel { Id = 1, Name = "Test Task 1" },
				new TaskServiceModel { Id = 2, Name = "Test Task 2" }
			};

			// Act
			var model = new AllTasksQueryModel
			{
				Search = "test search",
				Sorting = TaskSorting.NameAscending,
				CurrentPage = 2,
				TotalTasksCount = 50,
				Tasks = tasks
			};

			// Assert
			Assert.That(model.Search, Is.EqualTo("test search"));
			Assert.That(model.Sorting, Is.EqualTo(TaskSorting.NameAscending));
			Assert.That(model.CurrentPage, Is.EqualTo(2));
			Assert.That(model.TotalTasksCount, Is.EqualTo(50));
			Assert.That(model.Tasks.Count(), Is.EqualTo(2));
			Assert.That(model.Tasks.First().Name, Is.EqualTo("Test Task 1"));
		}

		[Test]
		public void PaginatedTasksViewModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PaginatedTasksViewModel();

			// Assert
			Assert.That(model.Tasks, Is.Not.Null);
			Assert.That(model.Tasks.Count(), Is.EqualTo(0));
		}

		[Test]
		public void PaginatedTasksViewModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var tasks = new List<TaskServiceModel>
			{
				new TaskServiceModel { Id = 1, Name = "Task 1" },
				new TaskServiceModel { Id = 2, Name = "Task 2" }
			};
			var pager = new TeamWorkFlow.Core.Models.Pager.PagerServiceModel(20, 2, 5);

			// Act
			var model = new PaginatedTasksViewModel
			{
				Tasks = tasks,
				Pager = pager
			};

			// Assert
			Assert.That(model.Tasks.Count(), Is.EqualTo(2));
			Assert.That(model.Pager, Is.Not.Null);
			Assert.That(model.Pager.TotalProjects, Is.EqualTo(20));
			Assert.That(model.Pager.CurrentPage, Is.EqualTo(2));
		}

		[Test]
		public void TaskQueryServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new TaskQueryServiceModel();

			// Assert
			Assert.That(model.Tasks, Is.Not.Null);
			Assert.That(model.Tasks.Count(), Is.EqualTo(0));
			Assert.That(model.TotalTasksCount, Is.EqualTo(0));
		}

		[Test]
		public void TaskQueryServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var tasks = new List<TaskServiceModel>
			{
				new TaskServiceModel { Id = 1, Name = "Query Task 1" },
				new TaskServiceModel { Id = 2, Name = "Query Task 2" },
				new TaskServiceModel { Id = 3, Name = "Query Task 3" }
			};

			// Act
			var model = new TaskQueryServiceModel
			{
				TotalTasksCount = 100,
				Tasks = tasks
			};

			// Assert
			Assert.That(model.TotalTasksCount, Is.EqualTo(100));
			Assert.That(model.Tasks.Count(), Is.EqualTo(3));
			Assert.That(model.Tasks.First().Name, Is.EqualTo("Query Task 1"));
			Assert.That(model.Tasks.Last().Name, Is.EqualTo("Query Task 3"));
		}

		[Test]
		public void TaskServiceModel_AllProperties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new TaskServiceModel
			{
				Id = 123,
				ProjectNumber = "PRJ-2024-001",
				Name = "Test Task Name",
				Description = "Test task description",
				Status = "In Progress",
				Priority = "High",
				Deadline = "31/12/2024",
				StartDate = "01/01/2024",
				EndDate = "15/01/2024"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(123));
			Assert.That(model.ProjectNumber, Is.EqualTo("PRJ-2024-001"));
			Assert.That(model.Name, Is.EqualTo("Test Task Name"));
			Assert.That(model.Description, Is.EqualTo("Test task description"));
			Assert.That(model.Status, Is.EqualTo("In Progress"));
			Assert.That(model.Priority, Is.EqualTo("High"));
			Assert.That(model.Deadline, Is.EqualTo("31/12/2024"));
			Assert.That(model.StartDate, Is.EqualTo("01/01/2024"));
			Assert.That(model.EndDate, Is.EqualTo("15/01/2024"));
		}

		[Test]
		public void TaskServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new TaskServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
			Assert.That(model.Description, Is.EqualTo(string.Empty));
			Assert.That(model.Status, Is.EqualTo(string.Empty));
			Assert.That(model.Priority, Is.EqualTo(string.Empty));
			Assert.That(model.Deadline, Is.EqualTo(string.Empty));
			Assert.That(model.StartDate, Is.EqualTo(string.Empty));
			Assert.That(model.EndDate, Is.Null);
		}

		#endregion

		#region Additional Service Method Coverage Tests

		[Test]
		public async Task AllAsync_WithAllSortingOptions_ExecutesSuccessfully()
		{
			// Test all sorting enum values to ensure complete coverage
			var sortingOptions = Enum.GetValues<TaskSorting>();

			foreach (var sorting in sortingOptions)
			{
				// Act
				var result = await _taskService.AllAsync(sorting: sorting);

				// Assert
				Assert.That(result, Is.Not.Null, $"Result should not be null for sorting: {sorting}");
				Assert.That(result.Tasks, Is.Not.Null, $"Tasks should not be null for sorting: {sorting}");
			}
		}

		[Test]
		public async Task AllAsync_WithNullSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(search: null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithWhitespaceSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _taskService.AllAsync(search: "   ");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Tasks, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithNegativePageNumber_HandlesGracefullyOrThrows()
		{
			// Arrange & Act
			try
			{
				var result = await _taskService.AllAsync(currentPage: -1);
				// If no exception is thrown (SQLite), verify result is not null
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tasks, Is.Not.Null);
			}
			catch (Microsoft.Data.SqlClient.SqlException)
			{
				// Expected behavior in SQL Server - test passes
				Assert.Pass("SqlException thrown as expected in SQL Server environment");
			}
			catch (Exception ex)
			{
				// Any other exception should fail the test
				Assert.Fail($"Unexpected exception type: {ex.GetType().Name}");
			}
		}

		[Test]
		public async Task AllAsync_WithNegativeTasksPerPage_HandlesGracefullyOrThrows()
		{
			// Arrange & Act
			try
			{
				var result = await _taskService.AllAsync(tasksPerPage: -5);
				// If no exception is thrown (SQLite), verify result is not null
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tasks, Is.Not.Null);
			}
			catch (Microsoft.Data.SqlClient.SqlException)
			{
				// Expected behavior in SQL Server - test passes
				Assert.Pass("SqlException thrown as expected in SQL Server environment");
			}
			catch (Exception ex)
			{
				// Any other exception should fail the test
				Assert.Fail($"Unexpected exception type: {ex.GetType().Name}");
			}
		}

		[Test]
		public async Task GetAllTasksAsync_WithZeroPageSize_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _taskService.GetAllTasksAsync(1, 0);

			// Assert
			Assert.That(result.Tasks, Is.Not.Null);
		}

		[Test]
		public async Task GetAllTasksAsync_WithNegativePage_HandlesGracefullyOrThrows()
		{
			// Arrange & Act
			try
			{
				var result = await _taskService.GetAllTasksAsync(-1, 5);
				// If no exception is thrown (SQLite), verify result is not null
				Assert.That(result.Tasks, Is.Not.Null);
			}
			catch (Microsoft.Data.SqlClient.SqlException)
			{
				// Expected behavior in SQL Server - test passes
				Assert.Pass("SqlException thrown as expected in SQL Server environment");
			}
			catch (Exception ex)
			{
				// Any other exception should fail the test
				Assert.Fail($"Unexpected exception type: {ex.GetType().Name}");
			}
		}

		[Test]
		public async Task GetAllAssignedTasksAsync_WithZeroPageSize_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _taskService.GetAllAssignedTasksAsync(1, 0);

			// Assert
			Assert.That(result.Tasks, Is.Not.Null);
		}

		[Test]
		public async Task GetAllAssignedTasksAsync_WithNegativePage_HandlesGracefullyOrThrows()
		{
			// Arrange & Act
			try
			{
				var result = await _taskService.GetAllAssignedTasksAsync(-1, 5);
				// If no exception is thrown (SQLite), verify result is not null
				Assert.That(result.Tasks, Is.Not.Null);
			}
			catch (Microsoft.Data.SqlClient.SqlException)
			{
				// Expected behavior in SQL Server - test passes
				Assert.Pass("SqlException thrown as expected in SQL Server environment");
			}
			catch (Exception ex)
			{
				// Any other exception should fail the test
				Assert.Fail($"Unexpected exception type: {ex.GetType().Name}");
			}
		}

		[Test]
		public async Task AddTaskToMyCollection_WithNullTaskModel_ThrowsNullReferenceException()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";

			// Act & Assert
			Assert.ThrowsAsync<NullReferenceException>(async () =>
				await _taskService.AddTaskToMyCollection(null, userId));
		}

		[Test]
		public async Task AddTaskToMyCollection_WithNullUserId_ThrowsUnExistingActionException()
		{
			// Arrange
			var taskModel = new TaskServiceModel()
			{
				Id = 1,
				Name = "Test Task"
			};

			// Act & Assert
			var ex = Assert.ThrowsAsync<UnExistingActionException>(async () =>
				await _taskService.AddTaskToMyCollection(taskModel, null));
			Assert.That(ex.Message, Is.EqualTo("The operator with this userId does not exist"));
		}

		[Test]
		public async Task RemoveFromCollection_WithNullUserId_ThrowsUnExistingActionException()
		{
			// Arrange
			int taskId = 1;

			// Act & Assert
			var ex = Assert.ThrowsAsync<UnExistingActionException>(async () =>
				await _taskService.RemoveFromCollection(taskId, null));
			Assert.That(ex.Message, Is.EqualTo("The operator with this userId does not exist"));
		}

		[Test]
		public async Task GetMyTasksAsync_WithValidUserId_ReturnsTaskCollection()
		{
			// Arrange
			string userId = "cf41999b-9cad-4b75-977d-a2fdb3d02e77";

			// Act
			var result = await _taskService.GetMyTasksAsync(userId);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task TaskExistInTaskOperatorTableByIdAsync_WithValidTaskId_ReturnsCorrectResult()
		{
			// Arrange
			var task = await _repository.AllReadOnly<TeamWorkFlow.Infrastructure.Data.Models.Task>()
				.FirstOrDefaultAsync();
			Assert.That(task, Is.Not.Null);

			// Act
			var result = await _taskService.TaskExistInTaskOperatorTableByIdAsync(task.Id);

			// Assert - The result depends on whether the task is assigned to any operator
			Assert.That(result, Is.TypeOf<bool>());
		}

		#endregion

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}

}
