using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.TimeTracking;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using TaskEntity = TeamWorkFlow.Infrastructure.Data.Models.Task;

namespace UnitTests
{
    [TestFixture]
    public class TaskTimeTrackingServiceUnitTests
    {
        private IRepository _repository = null!;
        private TeamWorkFlowDbContext _dbContext = null!;
        private ITaskTimeTrackingService _timeTrackingService = null!;
        private Mock<ILogger<TaskTimeTrackingService>> _mockLogger = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TeamWorkFlowDbContext(options);
            _repository = new Repository(_dbContext);
            _mockLogger = new Mock<ILogger<TaskTimeTrackingService>>();
            _timeTrackingService = new TaskTimeTrackingService(_repository, _mockLogger.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Seed test data
            var user = new IdentityUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@test.com"
            };

            var operatorUser1 = new IdentityUser
            {
                Id = "operator-user-id-1",
                UserName = "operator1",
                Email = "john.doe@test.com"
            };

            var operatorUser2 = new IdentityUser
            {
                Id = "operator-user-id-2",
                UserName = "operator2",
                Email = "jane.smith@test.com"
            };

            var projectStatus = new ProjectStatus
            {
                Id = 1,
                Name = "Active"
            };

            var taskStatus = new TeamWorkFlow.Infrastructure.Data.Models.TaskStatus
            {
                Id = 1,
                Name = "Open"
            };

            var priority = new Priority
            {
                Id = 1,
                Name = "Medium"
            };

            var availabilityStatus = new OperatorAvailabilityStatus
            {
                Id = 1,
                Name = "Available"
            };

            var project = new Project
            {
                Id = 1,
                ProjectName = "Test Project",
                ProjectNumber = "TP001",
                ProjectStatusId = 1
            };

            var task = new TaskEntity
            {
                Id = 1,
                Name = "Test Task",
                Description = "Test Task Description",
                EstimatedTime = 8, // 8 hours
                ProjectId = 1,
                TaskStatusId = 1,
                PriorityId = 1,
                StartDate = DateTime.Now.AddDays(-1),
                DeadLine = DateTime.Now.AddDays(7),
                CreatorId = "test-user-id"
            };

            var operator1 = new Operator
            {
                Id = 1,
                FullName = "John Doe",
                Email = "john.doe@test.com",
                PhoneNumber = "1234567890",
                IsActive = true,
                AvailabilityStatusId = 1,
                Capacity = 8,
                UserId = "operator-user-id-1"
            };

            var operator2 = new Operator
            {
                Id = 2,
                FullName = "Jane Smith",
                Email = "jane.smith@test.com",
                PhoneNumber = "0987654321",
                IsActive = true,
                AvailabilityStatusId = 1,
                Capacity = 8,
                UserId = "operator-user-id-2"
            };

            _dbContext.Users.AddRange(user, operatorUser1, operatorUser2);
            _dbContext.ProjectStatusEnumerable.Add(projectStatus);
            _dbContext.TaskStatusEnumerable.Add(taskStatus);
            _dbContext.Priorities.Add(priority);
            _dbContext.OperatorAvailabilityStatusEnumerable.Add(availabilityStatus);
            _dbContext.Projects.Add(project);
            _dbContext.Tasks.Add(task);
            _dbContext.Operators.AddRange(operator1, operator2);
            _dbContext.SaveChanges();
        }

        #region StartWorkSessionAsync Tests

        [Test]
        public async System.Threading.Tasks.Task StartWorkSessionAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;
            string sessionType = "Development";

            // Act
            var result = await _timeTrackingService.StartWorkSessionAsync(taskId, operatorId, sessionType);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Work session started successfully."));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.HasActiveSession, Is.True);

            // Verify session was created in database
            var session = await _dbContext.TaskTimeSessions
                .FirstOrDefaultAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);
            Assert.That(session, Is.Not.Null);
            Assert.That(session!.Status, Is.EqualTo("Active"));
        }

        [Test]
        public async System.Threading.Tasks.Task StartWorkSessionAsync_WithInvalidTask_ReturnsFailure()
        {
            // Arrange
            int invalidTaskId = 999;
            int operatorId = 1;

            // Act
            var result = await _timeTrackingService.StartWorkSessionAsync(invalidTaskId, operatorId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Task not found or not available for time tracking."));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async System.Threading.Tasks.Task StartWorkSessionAsync_WithInvalidOperator_ReturnsFailure()
        {
            // Arrange
            int taskId = 1;
            int invalidOperatorId = 999;

            // Act
            var result = await _timeTrackingService.StartWorkSessionAsync(taskId, invalidOperatorId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Operator not found or not active."));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async System.Threading.Tasks.Task StartWorkSessionAsync_WithExistingActiveSession_ReturnsFailure()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;

            // Start first session
            await _timeTrackingService.StartWorkSessionAsync(taskId, operatorId);

            // Act - Try to start another session
            var result = await _timeTrackingService.StartWorkSessionAsync(taskId, operatorId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("An active work session already exists for this task and operator."));
        }

        #endregion

        #region FinishWorkSessionAsync Tests

        [Test]
        public async System.Threading.Tasks.Task FinishWorkSessionAsync_WithValidSession_ReturnsSuccess()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;
            string notes = "Completed development work";

            // Start a session first
            await _timeTrackingService.StartWorkSessionAsync(taskId, operatorId);

            // Manually adjust the session start time to ensure duration > 0
            var session = await _dbContext.TaskTimeSessions
                .FirstOrDefaultAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);
            session!.StartTime = DateTime.UtcNow.AddMinutes(-5); // Set start time to 5 minutes ago
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timeTrackingService.FinishWorkSessionAsync(taskId, operatorId, notes);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("Work session completed"));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.HasActiveSession, Is.False);

            // Verify time entry was created
            var timeEntry = await _dbContext.TaskTimeEntries
                .FirstOrDefaultAsync(e => e.TaskId == taskId && e.OperatorId == operatorId);
            Assert.That(timeEntry, Is.Not.Null);
            Assert.That(timeEntry!.Notes, Is.EqualTo(notes));
            Assert.That(timeEntry.DurationMinutes, Is.GreaterThan(0));

            // Verify session was removed
            var removedSession = await _dbContext.TaskTimeSessions
                .FirstOrDefaultAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);
            Assert.That(removedSession, Is.Null);
        }

        [Test]
        public async System.Threading.Tasks.Task FinishWorkSessionAsync_WithoutActiveSession_ReturnsFailure()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;

            // Act
            var result = await _timeTrackingService.FinishWorkSessionAsync(taskId, operatorId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("No active work session found for this task and operator."));
        }

        #endregion

        #region GetTaskTimeTrackingAsync Tests

        [Test]
        public async System.Threading.Tasks.Task GetTaskTimeTrackingAsync_WithValidData_ReturnsCorrectModel()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;

            // Act
            var result = await _timeTrackingService.GetTaskTimeTrackingAsync(taskId, operatorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TaskId, Is.EqualTo(taskId));
            Assert.That(result.OperatorId, Is.EqualTo(operatorId));
            Assert.That(result.TaskName, Is.EqualTo("Test Task"));
            Assert.That(result.EstimatedTimeHours, Is.EqualTo(8));
            Assert.That(result.HasActiveSession, Is.False);
            Assert.That(result.TotalActualTimeMinutes, Is.EqualTo(0));
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskTimeTrackingAsync_WithInvalidTask_ReturnsNull()
        {
            // Arrange
            int invalidTaskId = 999;
            int operatorId = 1;

            // Act
            var result = await _timeTrackingService.GetTaskTimeTrackingAsync(invalidTaskId, operatorId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskTimeTrackingAsync_WithInvalidOperator_ReturnsNull()
        {
            // Arrange
            int taskId = 1;
            int invalidOperatorId = 999;

            // Act
            var result = await _timeTrackingService.GetTaskTimeTrackingAsync(taskId, invalidOperatorId);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region Time Calculation Bug Fix Tests

        [Test]
        public async System.Threading.Tasks.Task GetTaskTimeTrackingAsync_WithMultipleSessions_CalculatesTotalFromAllSessions()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;

            // Create multiple time entries (more than 5 to test the bug fix)
            var timeEntries = new List<TaskTimeEntry>();
            for (int i = 0; i < 7; i++)
            {
                timeEntries.Add(new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-i - 1),
                    EndTime = DateTime.UtcNow.AddHours(-i),
                    DurationMinutes = 15, // Each session is 15 minutes
                    Notes = $"Session {i + 1}",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-i - 1)
                });
            }

            await _dbContext.TaskTimeEntries.AddRangeAsync(timeEntries);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timeTrackingService.GetTaskTimeTrackingAsync(taskId, operatorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalActualTimeMinutes, Is.EqualTo(105), // 7 sessions × 15 minutes = 105 minutes
                "Total time should include ALL sessions, not just the recent 5");
            Assert.That(result.TotalCompletedSessions, Is.EqualTo(7),
                "Total sessions count should include ALL sessions");
            Assert.That(result.RecentSessions.Count, Is.EqualTo(5),
                "Recent sessions should still be limited to 5 for display");
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskTimeTrackingAsync_WithExactly5Sessions_CalculatesCorrectly()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;

            // Create exactly 5 time entries
            var timeEntries = new List<TaskTimeEntry>();
            for (int i = 0; i < 5; i++)
            {
                timeEntries.Add(new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-i - 1),
                    EndTime = DateTime.UtcNow.AddHours(-i),
                    DurationMinutes = 12, // Each session is 12 minutes
                    Notes = $"Session {i + 1}",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-i - 1)
                });
            }

            await _dbContext.TaskTimeEntries.AddRangeAsync(timeEntries);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timeTrackingService.GetTaskTimeTrackingAsync(taskId, operatorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalActualTimeMinutes, Is.EqualTo(60), // 5 sessions × 12 minutes = 60 minutes
                "Total time should be calculated correctly with exactly 5 sessions");
            Assert.That(result.TotalCompletedSessions, Is.EqualTo(5));
            Assert.That(result.RecentSessions.Count, Is.EqualTo(5));
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskTimeTrackingAsync_WithFewerThan5Sessions_CalculatesCorrectly()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;

            // Create only 3 time entries
            var timeEntries = new List<TaskTimeEntry>();
            for (int i = 0; i < 3; i++)
            {
                timeEntries.Add(new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-i - 1),
                    EndTime = DateTime.UtcNow.AddHours(-i),
                    DurationMinutes = 20, // Each session is 20 minutes
                    Notes = $"Session {i + 1}",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-i - 1)
                });
            }

            await _dbContext.TaskTimeEntries.AddRangeAsync(timeEntries);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timeTrackingService.GetTaskTimeTrackingAsync(taskId, operatorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalActualTimeMinutes, Is.EqualTo(60), // 3 sessions × 20 minutes = 60 minutes
                "Total time should be calculated correctly with fewer than 5 sessions");
            Assert.That(result.TotalCompletedSessions, Is.EqualTo(3));
            Assert.That(result.RecentSessions.Count, Is.EqualTo(3));
        }

        #endregion

        #region GetWorkSessionHistoryAsync Tests

        [Test]
        public async System.Threading.Tasks.Task GetWorkSessionHistoryAsync_WithValidData_ReturnsCorrectHistory()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;
            int limit = 10;

            // Create test time entries
            var timeEntries = new List<TaskTimeEntry>();
            for (int i = 0; i < 5; i++)
            {
                timeEntries.Add(new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-i - 1),
                    EndTime = DateTime.UtcNow.AddHours(-i),
                    DurationMinutes = 30,
                    Notes = $"Work session {i + 1}",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-i - 1)
                });
            }

            await _dbContext.TaskTimeEntries.AddRangeAsync(timeEntries);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timeTrackingService.GetWorkSessionHistoryAsync(taskId, operatorId, limit);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.First().TaskName, Is.EqualTo("Test Task"));
            Assert.That(result.First().OperatorName, Is.EqualTo("John Doe"));
            Assert.That(result.All(s => s.DurationMinutes == 30), Is.True);

            // Verify ordering (most recent first)
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.That(result[i].CreatedAt, Is.GreaterThanOrEqualTo(result[i + 1].CreatedAt),
                    "Sessions should be ordered by most recent first");
            }
        }

        [Test]
        public async System.Threading.Tasks.Task GetWorkSessionHistoryAsync_WithLimit_RespectsLimit()
        {
            // Arrange
            int taskId = 1;
            int operatorId = 1;
            int limit = 3;

            // Create more entries than the limit
            var timeEntries = new List<TaskTimeEntry>();
            for (int i = 0; i < 5; i++)
            {
                timeEntries.Add(new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-i - 1),
                    EndTime = DateTime.UtcNow.AddHours(-i),
                    DurationMinutes = 25,
                    Notes = $"Session {i + 1}",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-i - 1)
                });
            }

            await _dbContext.TaskTimeEntries.AddRangeAsync(timeEntries);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timeTrackingService.GetWorkSessionHistoryAsync(taskId, operatorId, limit);

            // Assert
            Assert.That(result.Count, Is.EqualTo(limit), "Should respect the limit parameter");
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}
