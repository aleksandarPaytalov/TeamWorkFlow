using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using TaskEntity = TeamWorkFlow.Infrastructure.Data.Models.Task;

namespace UnitTests
{
    [TestFixture]
    public class TimeCalculationBugFixUnitTests
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
            var user = new IdentityUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@test.com"
            };

            var operatorUser = new IdentityUser
            {
                Id = "test-operator-user-id",
                UserName = "operator",
                Email = "test.operator@bugfix.com"
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
                ProjectName = "Bug Fix Test Project",
                ProjectNumber = "BF001",
                ProjectStatusId = 1
            };

            var task = new TaskEntity
            {
                Id = 1,
                Name = "Bug Fix Test Task",
                Description = "Task for testing time calculation bug fix",
                EstimatedTime = 5, // 5 hours
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
                FullName = "Test Operator",
                Email = "test.operator@bugfix.com",
                PhoneNumber = "1234567890",
                IsActive = true,
                AvailabilityStatusId = 1,
                Capacity = 8,
                UserId = "test-operator-user-id"
            };

            _dbContext.Users.AddRange(user, operatorUser);
            _dbContext.ProjectStatusEnumerable.Add(projectStatus);
            _dbContext.TaskStatusEnumerable.Add(taskStatus);
            _dbContext.Priorities.Add(priority);
            _dbContext.OperatorAvailabilityStatusEnumerable.Add(availabilityStatus);
            _dbContext.Projects.Add(project);
            _dbContext.Tasks.Add(task);
            _dbContext.Operators.Add(operator1);
            _dbContext.SaveChanges();
        }

        [Test]
        public async System.Threading.Tasks.Task BugFix_HistoryModal_And_TaskCard_ShowSameTime_WithMoreThan5Sessions()
        {
            // Arrange - This test reproduces the original bug scenario
            int taskId = 1;
            int operatorId = 1;

            // Create 7 time entries to test the bug (more than the 5-session limit)
            var timeEntries = new List<TaskTimeEntry>();
            for (int i = 0; i < 7; i++)
            {
                timeEntries.Add(new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-i - 2),
                    EndTime = DateTime.UtcNow.AddHours(-i - 1),
                    DurationMinutes = 15, // Each session is 15 minutes
                    Notes = $"Bug fix test session {i + 1}",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-i - 2)
                });
            }

            await _dbContext.TaskTimeEntries.AddRangeAsync(timeEntries);
            await _dbContext.SaveChangesAsync();

            // Act - Get time tracking data (used by task card)
            var taskCardData = await _timeTrackingService.GetTaskTimeTrackingAsync(taskId, operatorId);

            // Get session history data (used by history modal)
            var historyData = await _timeTrackingService.GetWorkSessionHistoryAsync(taskId, operatorId);

            // Calculate total from history (simulating what the history modal does)
            var historyTotalMinutes = historyData.Sum(s => s.DurationMinutes);

            // Assert - Both should show the same total time
            Assert.That(taskCardData, Is.Not.Null, "Task card data should not be null");
            Assert.That(taskCardData!.TotalActualTimeMinutes, Is.EqualTo(105),
                "Task card should show total of ALL 7 sessions (7 × 15 = 105 minutes)");

            Assert.That(historyTotalMinutes, Is.EqualTo(105),
                "History modal should show total of ALL 7 sessions (7 × 15 = 105 minutes)");

            Assert.That(taskCardData.TotalActualTimeMinutes, Is.EqualTo(historyTotalMinutes),
                "Task card and history modal should show the SAME total time");

            // Verify that recent sessions are still limited to 5 for display
            Assert.That(taskCardData.RecentSessions.Count, Is.EqualTo(5),
                "Recent sessions should still be limited to 5 for display efficiency");

            // Verify total sessions count includes all sessions
            Assert.That(taskCardData.TotalCompletedSessions, Is.EqualTo(7),
                "Total completed sessions should count ALL sessions");
        }

        [Test]
        public async System.Threading.Tasks.Task BugFix_OriginalScenario_15MinutesVs12Minutes_IsFixed()
        {
            // Arrange - Recreate the exact scenario from the user's report
            int taskId = 1;
            int operatorId = 1;

            // Create sessions that would result in 15m total but only 12m if limited to 5 sessions
            // This simulates having older sessions that were being excluded
            var timeEntries = new List<TaskTimeEntry>
            {
                // Older sessions (these were being excluded in the bug)
                new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddDays(-5),
                    EndTime = DateTime.UtcNow.AddDays(-5).AddMinutes(3),
                    DurationMinutes = 3,
                    Notes = "Old session 1",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },

                // Recent sessions (these 5 would total 12 minutes)
                new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-5),
                    EndTime = DateTime.UtcNow.AddHours(-5).AddMinutes(2),
                    DurationMinutes = 2,
                    Notes = "Recent session 1",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                },
                new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-4),
                    EndTime = DateTime.UtcNow.AddHours(-4).AddMinutes(2),
                    DurationMinutes = 2,
                    Notes = "Recent session 2",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                },
                new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-3),
                    EndTime = DateTime.UtcNow.AddHours(-3).AddMinutes(3),
                    DurationMinutes = 3,
                    Notes = "Recent session 3",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-3)
                },
                new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-2),
                    EndTime = DateTime.UtcNow.AddHours(-2).AddMinutes(2),
                    DurationMinutes = 2,
                    Notes = "Recent session 4",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-1),
                    EndTime = DateTime.UtcNow.AddHours(-1).AddMinutes(3),
                    DurationMinutes = 3,
                    Notes = "Recent session 5",
                    SessionType = "Development",
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                }
            };

            await _dbContext.TaskTimeEntries.AddRangeAsync(timeEntries);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timeTrackingService.GetTaskTimeTrackingAsync(taskId, operatorId);

            // Assert - Should show 15 minutes total (not 12)
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalActualTimeMinutes, Is.EqualTo(15),
                "Should show 15 minutes total (3+2+2+3+2+3), not 12 minutes from just recent sessions");

            // Verify the bug is fixed: recent 5 sessions would only total 12 minutes
            var recentSessionsTotal = result.RecentSessions.Sum(s => s.DurationMinutes);
            Assert.That(recentSessionsTotal, Is.EqualTo(12),
                "Recent 5 sessions should total 12 minutes (2+2+3+2+3)");

            // But total should include ALL sessions
            Assert.That(result.TotalActualTimeMinutes, Is.GreaterThan(recentSessionsTotal),
                "Total time should be greater than recent sessions total, proving the bug is fixed");
        }

        [Test]
        public async System.Threading.Tasks.Task BugFix_Performance_RecentSessionsStillLimited()
        {
            // Arrange - Create many sessions to verify performance optimization is maintained
            int taskId = 1;
            int operatorId = 1;

            var timeEntries = new List<TaskTimeEntry>();
            for (int i = 0; i < 20; i++) // Create 20 sessions
            {
                timeEntries.Add(new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow.AddHours(-i - 1),
                    EndTime = DateTime.UtcNow.AddHours(-i),
                    DurationMinutes = 10,
                    Notes = $"Performance test session {i + 1}",
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
            Assert.That(result!.TotalActualTimeMinutes, Is.EqualTo(200), // 20 sessions × 10 minutes
                "Total should include all 20 sessions");
            Assert.That(result.TotalCompletedSessions, Is.EqualTo(20),
                "Total sessions count should be 20");
            Assert.That(result.RecentSessions.Count, Is.EqualTo(5),
                "Recent sessions should still be limited to 5 for performance");
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}
