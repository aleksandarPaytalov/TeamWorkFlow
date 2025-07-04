using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
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

			var configuration = new ConfigurationBuilder()
				.AddUserSecrets<TaskUnitTests>()
				.Build();

			var connectionString = configuration.GetConnectionString("Test");

			var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			_dbContext = new TeamWorkFlowDbContext(options);
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
			Assert.That(result.Count, Is.EqualTo(expectedResult));
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
			Assert.That(result.Count, Is.EqualTo(expectedResult.Count));
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
			Assert.That(result.Count, Is.EqualTo(expectedResult.Count));
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
			Assert.That(result.Tasks.Count, Is.EqualTo(task));
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
			Assert.That(result.Count, Is.EqualTo(assignedTasks));
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

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}	
	
}