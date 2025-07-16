using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Sprint;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using TaskEntity = TeamWorkFlow.Infrastructure.Data.Models.Task;
using TaskStatusEntity = TeamWorkFlow.Infrastructure.Data.Models.TaskStatus;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
    [TestFixture]
    public class SprintServiceUnitTests
    {
        private ServiceProvider serviceProvider;
        private TeamWorkFlowDbContext _dbContext;
        private IRepository _repository;
        private ISprintService _sprintService;

        [OneTimeSetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            serviceProvider = services
                .AddSingleton(sp => new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
                    .UseInMemoryDatabase("TeamWorkFlowInMemoryDb" + DateTime.Now.Ticks.ToString())
                    .Options)
                .AddSingleton<TeamWorkFlowDbContext>()
                .AddSingleton<IRepository, Repository>()
                .AddSingleton<ISprintService, SprintService>()
                .BuildServiceProvider();

            _repository = serviceProvider.GetService<IRepository>()!;
            _dbContext = serviceProvider.GetService<TeamWorkFlowDbContext>()!;
            _sprintService = serviceProvider.GetService<ISprintService>()!;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            serviceProvider?.Dispose();
        }

        #region GetSprintPlanningDataAsync Tests

        [Test]
        public async Task GetSprintPlanningDataAsync_WithNoFilters_ReturnsValidViewModel()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _sprintService.GetSprintPlanningDataAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SprintTasks, Is.Not.Null);
            Assert.That(result.BacklogTasks, Is.Not.Null);
            Assert.That(result.Capacity, Is.Not.Null);
            Assert.That(result.Resources, Is.Not.Null);
            Assert.That(result.Summary, Is.Not.Null);
            Assert.That(result.Timeline, Is.Not.Null);
        }

        [Test]
        public async Task GetSprintPlanningDataAsync_WithSearchTerm_FiltersCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();
            var searchTerm = "Test Task";

            // Act
            var result = await _sprintService.GetSprintPlanningDataAsync(searchTerm);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchTerm, Is.EqualTo(searchTerm));

            // Verify that filtered tasks contain the search term
            var allTasks = result.SprintTasks.Concat(result.BacklogTasks);
            foreach (var task in allTasks)
            {
                Assert.That(task.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           task.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           task.ProjectNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase),
                           Is.True, $"Task '{task.Name}' should contain search term '{searchTerm}'");
            }
        }

        [Test]
        public async Task GetSprintPlanningDataAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            await SeedTestDataAsync();
            int page = 1;
            int pageSize = 5;

            // Act
            var result = await _sprintService.GetSprintPlanningDataAsync(page: page, pageSize: pageSize);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CurrentPage, Is.EqualTo(page));
            Assert.That(result.PageSize, Is.EqualTo(pageSize));
            Assert.That(result.BacklogTasks.Count, Is.LessThanOrEqualTo(pageSize));
        }

        #endregion

        #region AddTaskToSprintAsync Tests

        [Test]
        public async Task AddTaskToSprintAsync_WithValidTask_ReturnsTrue()
        {
            // Arrange
            await SeedTestDataAsync();
            var task = await _repository.AllReadOnly<TaskEntity>()
                .FirstOrDefaultAsync(t => !t.IsInSprint);
            Assert.That(task, Is.Not.Null, "No non-sprint task found for testing");

            // Act
            var result = await _sprintService.AddTaskToSprintAsync(task!.Id, 1);

            // Assert
            Assert.That(result, Is.True);

            // Verify task is now in sprint
            var updatedTask = await _repository.GetByIdAsync<TaskEntity>(task.Id);
            Assert.That(updatedTask!.IsInSprint, Is.True);
            Assert.That(updatedTask.SprintOrder, Is.EqualTo(1));
            Assert.That(updatedTask.PlannedStartDate, Is.Not.Null);
            Assert.That(updatedTask.PlannedEndDate, Is.Not.Null);
        }

        [Test]
        public async Task AddTaskToSprintAsync_WithInvalidTaskId_ReturnsFalse()
        {
            // Arrange
            int invalidTaskId = 99999;

            // Act
            var result = await _sprintService.AddTaskToSprintAsync(invalidTaskId, 1);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region RemoveTaskFromSprintAsync Tests

        [Test]
        public async Task RemoveTaskFromSprintAsync_WithValidSprintTask_ReturnsTrue()
        {
            // Arrange
            await SeedTestDataAsync();
            var task = await _repository.AllReadOnly<TaskEntity>()
                .FirstOrDefaultAsync();

            // First add task to sprint
            await _sprintService.AddTaskToSprintAsync(task!.Id, 1);

            // Act
            var result = await _sprintService.RemoveTaskFromSprintAsync(task.Id);

            // Assert
            Assert.That(result, Is.True);

            // Verify task is no longer in sprint
            var updatedTask = await _repository.GetByIdAsync<TaskEntity>(task.Id);
            Assert.That(updatedTask!.IsInSprint, Is.False);
            Assert.That(updatedTask.SprintOrder, Is.EqualTo(0));
        }

        [Test]
        public async Task RemoveTaskFromSprintAsync_WithInvalidTaskId_ReturnsFalse()
        {
            // Arrange
            int invalidTaskId = 99999;

            // Act
            var result = await _sprintService.RemoveTaskFromSprintAsync(invalidTaskId);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region UpdateSprintTaskOrderAsync Tests

        [Test]
        public async Task UpdateSprintTaskOrderAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            await SeedTestDataAsync();
            var task = await _repository.AllReadOnly<TaskEntity>()
                .FirstOrDefaultAsync();

            // Add task to sprint first
            await _sprintService.AddTaskToSprintAsync(task!.Id, 1);
            var taskOrders = new Dictionary<int, int> { { task.Id, 5 } };

            // Act
            var result = await _sprintService.UpdateSprintTaskOrderAsync(taskOrders);

            // Assert
            Assert.That(result, Is.True);

            // Verify order was updated
            var updatedTask = await _repository.GetByIdAsync<TaskEntity>(task.Id);
            Assert.That(updatedTask!.SprintOrder, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateSprintTaskOrderAsync_WithInvalidTaskId_ReturnsTrue()
        {
            // Arrange
            int invalidTaskId = 99999;
            var taskOrders = new Dictionary<int, int> { { invalidTaskId, 5 } };

            // Act
            var result = await _sprintService.UpdateSprintTaskOrderAsync(taskOrders);

            // Assert
            // The service implementation skips invalid task IDs and returns true if no exception occurs
            Assert.That(result, Is.True);
        }

        #endregion

        #region UpdateTaskEstimatedTimeAsync Tests

        [Test]
        public async Task UpdateTaskEstimatedTimeAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            await SeedTestDataAsync();
            var task = await _repository.AllReadOnly<TaskEntity>()
                .FirstOrDefaultAsync();
            int newEstimatedTime = 16;

            // Act
            var result = await _sprintService.UpdateTaskEstimatedTimeAsync(task!.Id, newEstimatedTime);

            // Assert
            Assert.That(result, Is.True);

            // Verify estimated time was updated
            var updatedTask = await _repository.GetByIdAsync<TaskEntity>(task.Id);
            Assert.That(updatedTask!.EstimatedTime, Is.EqualTo(newEstimatedTime));
        }

        [Test]
        public async Task UpdateTaskEstimatedTimeAsync_WithInvalidTaskId_ReturnsFalse()
        {
            // Arrange
            int invalidTaskId = 99999;
            int newEstimatedTime = 16;

            // Act
            var result = await _sprintService.UpdateTaskEstimatedTimeAsync(invalidTaskId, newEstimatedTime);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region ValidateTaskForSprintAsync Tests

        [Test]
        public async Task ValidateTaskForSprintAsync_WithValidTask_ReturnsCanAddTrue()
        {
            // Arrange
            await SeedTestDataAsync();

            // Create a task with low estimated time to ensure capacity validation passes
            var lowEstimateTask = new TaskEntity
            {
                Id = 4,
                Name = "Low Estimate Task",
                Description = "Low estimate task for validation",
                EstimatedTime = 1, // Very low estimate to pass capacity check
                StartDate = DateTime.Today,
                PriorityId = 1,
                TaskStatusId = 1,
                ProjectId = 1,
                CreatorId = "test-user",
                IsInSprint = false
            };

            _dbContext.Tasks.Add(lowEstimateTask);
            await _dbContext.SaveChangesAsync();

            // Act
            var (canAdd, reason) = await _sprintService.ValidateTaskForSprintAsync(lowEstimateTask.Id);

            // Assert
            Assert.That(canAdd, Is.True);
            Assert.That(reason, Is.EqualTo("Task can be added to sprint"));
        }

        [Test]
        public async Task ValidateTaskForSprintAsync_WithInvalidTaskId_ReturnsCanAddFalse()
        {
            // Arrange
            int invalidTaskId = 99999;

            // Act
            var (canAdd, reason) = await _sprintService.ValidateTaskForSprintAsync(invalidTaskId);

            // Assert
            Assert.That(canAdd, Is.False);
            Assert.That(reason, Is.EqualTo("Task not found"));
        }

        #endregion

        #region GetSprintCapacityAsync Tests

        [Test]
        public async Task GetSprintCapacityAsync_ReturnsValidCapacityModel()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _sprintService.GetSprintCapacityAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalOperatorHours, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.TotalMachineHours, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.RequiredOperatorHours, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.RequiredMachineHours, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.AvailableOperators, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.AvailableMachines, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.OperatorCapacities, Is.Not.Null);
            Assert.That(result.MachineCapacities, Is.Not.Null);
        }

        [Test]
        public async Task GetSprintCapacityAsync_CalculatesCanCompleteAllTasksCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _sprintService.GetSprintCapacityAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            // CanCompleteAllTasks should be true when required hours <= total hours
            var expectedCanComplete = result.RequiredOperatorHours <= result.TotalOperatorHours &&
                                    result.RequiredMachineHours <= result.TotalMachineHours;
            Assert.That(result.CanCompleteAllTasks, Is.EqualTo(expectedCanComplete));
        }

        #endregion

        #region GetResourceAvailabilityAsync Tests

        [Test]
        public async Task GetResourceAvailabilityAsync_ReturnsValidResourceModel()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _sprintService.GetResourceAvailabilityAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Operators, Is.Not.Null);
            Assert.That(result.Machines, Is.Not.Null);
            Assert.That(result.TotalOperatorCapacity, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.TotalMachineCapacity, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.ActiveOperatorsCount, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.ActiveMachinesCount, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task GetResourceAvailabilityAsync_CalculatesUtilizationCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _sprintService.GetResourceAvailabilityAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AverageOperatorUtilization, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.AverageOperatorUtilization, Is.LessThanOrEqualTo(100));
            Assert.That(result.AverageMachineUtilization, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.AverageMachineUtilization, Is.LessThanOrEqualTo(100));
        }

        #endregion

        #region GetSprintSummaryAsync Tests

        [Test]
        public async Task GetSprintSummaryAsync_ReturnsValidSummaryModel()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _sprintService.GetSprintSummaryAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalTasksInSprint, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.CompletedTasks, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.InProgressTasks, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.NotStartedTasks, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.OverdueTasks, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.TotalEstimatedHours, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.CompletionPercentage, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.CompletionPercentage, Is.LessThanOrEqualTo(100));
        }

        [Test]
        public async Task GetSprintSummaryAsync_CalculatesHealthStatusCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _sprintService.GetSprintSummaryAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            var healthStatus = result.GetSprintHealth();
            Assert.That(healthStatus, Is.Not.Null);
            var validStatuses = new[] { "Excellent", "Good", "Fair", "Poor", "At Risk" };
            Assert.That(validStatuses, Contains.Item(healthStatus));

            var healthColor = result.GetHealthColor();
            Assert.That(healthColor, Is.Not.Null);
            Assert.That(healthColor.StartsWith("#"), Is.True, "Health color should be a hex color code");
        }

        #endregion

        #region GetSprintTimelineAsync Tests

        [Test]
        public async Task GetSprintTimelineAsync_ReturnsValidTimelineData()
        {
            // Arrange
            await SeedTestDataAsync();
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(7);

            // Act
            var result = await _sprintService.GetSprintTimelineAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(8)); // 7 days + 1 (inclusive)

            foreach (var day in result)
            {
                Assert.That(day.Date, Is.GreaterThanOrEqualTo(startDate));
                Assert.That(day.Date, Is.LessThanOrEqualTo(endDate));
                Assert.That(day.TasksStarting, Is.Not.Null);
                Assert.That(day.TasksEnding, Is.Not.Null);
                Assert.That(day.TasksInProgress, Is.Not.Null);
                Assert.That(day.TotalHoursScheduled, Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test]
        public async Task GetSprintTimelineAsync_CalculatesOverloadCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(3);

            // Act
            var result = await _sprintService.GetSprintTimelineAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.Not.Null);

            foreach (var day in result)
            {
                // IsOverloaded should be true when TotalHoursScheduled > 8
                var expectedOverload = day.TotalHoursScheduled > 8;
                Assert.That(day.IsOverloaded, Is.EqualTo(expectedOverload));

                // Test status calculation
                var status = day.GetDayStatus();
                Assert.That(status, Is.Not.Null);

                var statusColor = day.GetStatusColor();
                Assert.That(statusColor, Is.Not.Null);
                Assert.That(statusColor.StartsWith("#"), Is.True);
            }
        }

        #endregion

        #region AutoAssignTasksToSprintAsync Tests

        [Test]
        public async Task AutoAssignTasksToSprintAsync_WithValidTasks_ReturnsAssignedCount()
        {
            // Arrange
            await SeedTestDataAsync();
            int maxTasks = 2;

            // Act
            var result = await _sprintService.AutoAssignTasksToSprintAsync(maxTasks);

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(0));
            Assert.That(result, Is.LessThanOrEqualTo(maxTasks));

            // Verify tasks were actually assigned
            var sprintTasks = await _repository.AllReadOnly<TaskEntity>()
                .Where(t => t.IsInSprint)
                .CountAsync();
            Assert.That(sprintTasks, Is.GreaterThanOrEqualTo(result));
        }

        [Test]
        public async Task AutoAssignTasksToSprintAsync_WithZeroMaxTasks_ReturnsZero()
        {
            // Arrange
            await SeedTestDataAsync();
            int maxTasks = 0;

            // Act
            var result = await _sprintService.AutoAssignTasksToSprintAsync(maxTasks);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion

        #region Helper Methods

        private async Task SeedTestDataAsync()
        {
            // Clear existing data
            _dbContext.Tasks.RemoveRange(_dbContext.Tasks);
            _dbContext.Projects.RemoveRange(_dbContext.Projects);
            _dbContext.Priorities.RemoveRange(_dbContext.Priorities);
            _dbContext.TaskStatusEnumerable.RemoveRange(_dbContext.TaskStatusEnumerable);
            _dbContext.Operators.RemoveRange(_dbContext.Operators);
            _dbContext.Machines.RemoveRange(_dbContext.Machines);
            _dbContext.OperatorAvailabilityStatusEnumerable.RemoveRange(_dbContext.OperatorAvailabilityStatusEnumerable);
            await _dbContext.SaveChangesAsync();

            // Clear change tracker to avoid tracking conflicts
            _dbContext.ChangeTracker.Clear();

            // Seed test data
            var priority = new Priority { Id = 1, Name = "High" };
            var status = new TaskStatusEntity { Id = 1, Name = "In Progress" };
            var project = new Project { Id = 1, ProjectName = "Test Project", ProjectNumber = "TP001" };
            var machine = new Machine { Id = 1, Name = "Test Machine", IsCalibrated = true, Capacity = 100 };

            // Create availability status for operators
            var availabilityStatus = new OperatorAvailabilityStatus { Id = 1, Name = "at work" };

            var operator1 = new Operator
            {
                Id = 1,
                FullName = "Test Operator",
                Email = "test@operator.com",
                IsActive = true,
                AvailabilityStatusId = 1
            };

            var operator2 = new Operator
            {
                Id = 2,
                FullName = "Test Operator 2",
                Email = "test2@operator.com",
                IsActive = true,
                AvailabilityStatusId = 1
            };

            var operator3 = new Operator
            {
                Id = 3,
                FullName = "Test Operator 3",
                Email = "test3@operator.com",
                IsActive = true,
                AvailabilityStatusId = 1
            };

            _dbContext.OperatorAvailabilityStatusEnumerable.Add(availabilityStatus);
            _dbContext.Priorities.Add(priority);
            _dbContext.TaskStatusEnumerable.Add(status);
            _dbContext.Projects.Add(project);
            _dbContext.Machines.Add(machine);
            _dbContext.Operators.AddRange(operator1, operator2, operator3);

            var tasks = new List<TaskEntity>
            {
                new() { Id = 1, Name = "Test Task 1", Description = "Description 1", EstimatedTime = 4,
                       StartDate = DateTime.Today, PriorityId = 1, TaskStatusId = 1, ProjectId = 1,
                       CreatorId = "test-user", IsInSprint = false },
                new() { Id = 2, Name = "Test Task 2", Description = "Description 2", EstimatedTime = 8,
                       StartDate = DateTime.Today, PriorityId = 1, TaskStatusId = 1, ProjectId = 1,
                       CreatorId = "test-user", IsInSprint = true, SprintOrder = 1 },
                new() { Id = 3, Name = "Sprint Task", Description = "Sprint Description", EstimatedTime = 6,
                       StartDate = DateTime.Today, PriorityId = 1, TaskStatusId = 1, ProjectId = 1,
                       CreatorId = "test-user", IsInSprint = true, SprintOrder = 2 }
            };

            _dbContext.Tasks.AddRange(tasks);
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
