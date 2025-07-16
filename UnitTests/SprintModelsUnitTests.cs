using NUnit.Framework;
using TeamWorkFlow.Core.Models.Sprint;

namespace UnitTests
{
    [TestFixture]
    public class SprintModelsUnitTests
    {
        #region SprintPlanningViewModel Tests

        [Test]
        public void SprintPlanningViewModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var model = new SprintPlanningViewModel();

            // Assert
            Assert.That(model.SprintTasks, Is.Not.Null);
            Assert.That(model.BacklogTasks, Is.Not.Null);
            Assert.That(model.Capacity, Is.Not.Null);
            Assert.That(model.Resources, Is.Not.Null);
            Assert.That(model.Summary, Is.Not.Null);
            Assert.That(model.Timeline, Is.Not.Null);
            Assert.That(model.SearchTerm, Is.EqualTo(string.Empty));
            Assert.That(model.StatusFilter, Is.EqualTo(string.Empty));
            Assert.That(model.PriorityFilter, Is.EqualTo(string.Empty));
            Assert.That(model.ProjectFilter, Is.EqualTo(string.Empty));
            Assert.That(model.OperatorFilter, Is.EqualTo(string.Empty));
            Assert.That(model.MachineFilter, Is.EqualTo(string.Empty));
            Assert.That(model.CurrentPage, Is.EqualTo(1));
            Assert.That(model.PageSize, Is.EqualTo(20));
            Assert.That(model.TotalTasks, Is.EqualTo(0));
            Assert.That(model.SprintStartDate, Is.EqualTo(DateTime.Today));
            Assert.That(model.SprintEndDate, Is.EqualTo(DateTime.Today.AddDays(14)));
            Assert.That(model.IsDragDropEnabled, Is.True);
            Assert.That(model.LastUpdatedBy, Is.EqualTo(string.Empty));
        }

        [Test]
        public void SprintPlanningViewModel_CalculatedProperties_WorkCorrectly()
        {
            // Arrange
            var model = new SprintPlanningViewModel
            {
                TotalTasks = 100,
                PageSize = 20,
                SprintStartDate = DateTime.Today,
                SprintEndDate = DateTime.Today.AddDays(13) // 14 days total
            };

            // Act & Assert
            Assert.That(model.TotalPages, Is.EqualTo(5)); // 100 / 20 = 5
            Assert.That(model.SprintDurationDays, Is.EqualTo(14)); // 13 days + 1 = 14
            Assert.That(model.SprintWorkingHours, Is.EqualTo(112)); // 14 * 8 = 112
        }

        [Test]
        public void SprintPlanningViewModel_TotalPagesCalculation_HandlesRemainder()
        {
            // Arrange
            var model = new SprintPlanningViewModel
            {
                TotalTasks = 101,
                PageSize = 20
            };

            // Act & Assert
            Assert.That(model.TotalPages, Is.EqualTo(6)); // Ceiling(101 / 20) = 6
        }

        #endregion

        #region SprintCapacityServiceModel Tests

        [Test]
        public void SprintCapacityServiceModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var model = new SprintCapacityServiceModel();

            // Assert
            Assert.That(model.TotalOperatorHours, Is.EqualTo(0));
            Assert.That(model.TotalMachineHours, Is.EqualTo(0));
            Assert.That(model.RequiredOperatorHours, Is.EqualTo(0));
            Assert.That(model.RequiredMachineHours, Is.EqualTo(0));
            Assert.That(model.AvailableOperators, Is.EqualTo(0));
            Assert.That(model.AvailableMachines, Is.EqualTo(0));
            Assert.That(model.OperatorCapacities, Is.Not.Null);
            Assert.That(model.MachineCapacities, Is.Not.Null);
        }

        [Test]
        public void SprintCapacityServiceModel_CanCompleteAllTasks_CalculatesCorrectly()
        {
            // Arrange & Act - Case 1: Can complete
            var model1 = new SprintCapacityServiceModel
            {
                TotalOperatorHours = 100,
                TotalMachineHours = 80,
                RequiredOperatorHours = 90,
                RequiredMachineHours = 70
            };

            // Assert
            Assert.That(model1.CanCompleteAllTasks, Is.True);

            // Arrange & Act - Case 2: Cannot complete (operator hours)
            var model2 = new SprintCapacityServiceModel
            {
                TotalOperatorHours = 100,
                TotalMachineHours = 80,
                RequiredOperatorHours = 110,
                RequiredMachineHours = 70
            };

            // Assert
            Assert.That(model2.CanCompleteAllTasks, Is.False);

            // Arrange & Act - Case 3: Cannot complete (machine hours)
            var model3 = new SprintCapacityServiceModel
            {
                TotalOperatorHours = 100,
                TotalMachineHours = 80,
                RequiredOperatorHours = 90,
                RequiredMachineHours = 90
            };

            // Assert
            Assert.That(model3.CanCompleteAllTasks, Is.False);
        }

        #endregion

        #region SprintResourceServiceModel Tests

        [Test]
        public void SprintResourceServiceModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var model = new SprintResourceServiceModel();

            // Assert
            Assert.That(model.Operators, Is.Not.Null);
            Assert.That(model.Machines, Is.Not.Null);
            Assert.That(model.TotalOperatorCapacity, Is.EqualTo(0));
            Assert.That(model.TotalMachineCapacity, Is.EqualTo(0));
            Assert.That(model.ActiveOperatorsCount, Is.EqualTo(0));
            Assert.That(model.ActiveMachinesCount, Is.EqualTo(0));
            Assert.That(model.AverageOperatorUtilization, Is.EqualTo(0));
            Assert.That(model.AverageMachineUtilization, Is.EqualTo(0));
        }

        [Test]
        public void SprintResourceServiceModel_CalculatedProperties_WorkCorrectly()
        {
            // Arrange
            var model = new SprintResourceServiceModel();

            // Note: Since UtilizationPercentage is read-only, we'll test with default values
            // In a real scenario, these would be calculated based on actual workload
            model.Operators.AddRange(new[]
            {
                new SprintOperatorModel { IsActive = true, WeeklyCapacity = 40 },
                new SprintOperatorModel { IsActive = true, WeeklyCapacity = 40 },
                new SprintOperatorModel { IsActive = false, WeeklyCapacity = 40 }
            });

            model.Machines.AddRange(new[]
            {
                new SprintMachineModel { IsActive = true, WeeklyCapacity = 168 },
                new SprintMachineModel { IsActive = true, WeeklyCapacity = 168 },
                new SprintMachineModel { IsActive = false, WeeklyCapacity = 168 }
            });

            // Act & Assert
            Assert.That(model.TotalOperatorCapacity, Is.EqualTo(80)); // 40 + 40 (only active)
            Assert.That(model.TotalMachineCapacity, Is.EqualTo(336)); // 168 + 168 (only active)
            Assert.That(model.ActiveOperatorsCount, Is.EqualTo(2));
            Assert.That(model.ActiveMachinesCount, Is.EqualTo(2));
            // Note: Utilization tests would need actual workload data to be meaningful
        }

        [Test]
        public void SprintResourceServiceModel_WithNoActiveResources_HandlesCorrectly()
        {
            // Arrange
            var model = new SprintResourceServiceModel();
            model.Operators.Add(new SprintOperatorModel { IsActive = false, WeeklyCapacity = 40 });
            model.Machines.Add(new SprintMachineModel { IsActive = false, WeeklyCapacity = 168 });

            // Act & Assert
            Assert.That(model.TotalOperatorCapacity, Is.EqualTo(0));
            Assert.That(model.TotalMachineCapacity, Is.EqualTo(0));
            Assert.That(model.ActiveOperatorsCount, Is.EqualTo(0));
            Assert.That(model.ActiveMachinesCount, Is.EqualTo(0));
            Assert.That(model.AverageOperatorUtilization, Is.EqualTo(0));
            Assert.That(model.AverageMachineUtilization, Is.EqualTo(0));
        }

        #endregion

        #region SprintTaskServiceModel Tests

        [Test]
        public void SprintTaskServiceModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var model = new SprintTaskServiceModel();

            // Assert
            Assert.That(model.Id, Is.EqualTo(0));
            Assert.That(model.Name, Is.EqualTo(string.Empty));
            Assert.That(model.Description, Is.EqualTo(string.Empty));
            Assert.That(model.EstimatedTime, Is.EqualTo(0));
            Assert.That(model.TaskStatus, Is.EqualTo(string.Empty));
            Assert.That(model.Priority, Is.EqualTo(string.Empty));
            Assert.That(model.ProjectName, Is.EqualTo(string.Empty));
            Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
            Assert.That(model.AssignedOperators, Is.Not.Null);
            Assert.That(model.SprintOrder, Is.EqualTo(0));
            Assert.That(model.IsInSprint, Is.False);
            Assert.That(model.ActualHours, Is.EqualTo(0));
            Assert.That(model.CreatorName, Is.EqualTo(string.Empty));
            Assert.That(model.CanBeCompleted, Is.True);
            Assert.That(model.StatusReason, Is.EqualTo(string.Empty));
        }

        [Test]
        public void SprintTaskServiceModel_CalculatedProperties_WorkCorrectly()
        {
            // Arrange
            var model = new SprintTaskServiceModel
            {
                EstimatedTime = 24,
                MachineId = 1
            };

            // Act & Assert
            Assert.That(model.RequiredOperatorHours, Is.EqualTo(24));
            Assert.That(model.RequiredMachineHours, Is.EqualTo(24)); // Has machine

            // Test without machine
            model.MachineId = null;
            Assert.That(model.RequiredMachineHours, Is.EqualTo(0)); // No machine
        }

        [Test]
        public void SprintTaskServiceModel_AllProperties_CanBeSetAndRetrieved()
        {
            // Arrange & Act
            var model = new SprintTaskServiceModel
            {
                Id = 1,
                Name = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                DeadLine = DateTime.Today.AddDays(2),
                EstimatedTime = 16,
                TaskStatus = "In Progress",
                Priority = "High",
                ProjectName = "Test Project",
                ProjectNumber = "TP001",
                MachineName = "Test Machine",
                SprintOrder = 5,
                IsInSprint = true,
                PlannedStartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddDays(2),
                ActualHours = 8.5m,
                CreatorName = "Test Creator",
                Comment = "Test Comment",
                Attachment = "test.pdf",
                MachineId = 1,
                CanBeCompleted = false,
                StatusReason = "Capacity constraints"
            };

            model.AssignedOperators.Add("Operator 1");
            model.AssignedOperators.Add("Operator 2");

            // Assert
            Assert.That(model.Id, Is.EqualTo(1));
            Assert.That(model.Name, Is.EqualTo("Test Task"));
            Assert.That(model.Description, Is.EqualTo("Test Description"));
            Assert.That(model.EstimatedTime, Is.EqualTo(16));
            Assert.That(model.TaskStatus, Is.EqualTo("In Progress"));
            Assert.That(model.Priority, Is.EqualTo("High"));
            Assert.That(model.ProjectName, Is.EqualTo("Test Project"));
            Assert.That(model.ProjectNumber, Is.EqualTo("TP001"));
            Assert.That(model.MachineName, Is.EqualTo("Test Machine"));
            Assert.That(model.SprintOrder, Is.EqualTo(5));
            Assert.That(model.IsInSprint, Is.True);
            Assert.That(model.ActualHours, Is.EqualTo(8.5m));
            Assert.That(model.CreatorName, Is.EqualTo("Test Creator"));
            Assert.That(model.Comment, Is.EqualTo("Test Comment"));
            Assert.That(model.Attachment, Is.EqualTo("test.pdf"));
            Assert.That(model.MachineId, Is.EqualTo(1));
            Assert.That(model.CanBeCompleted, Is.False);
            Assert.That(model.StatusReason, Is.EqualTo("Capacity constraints"));
            Assert.That(model.AssignedOperators.Count, Is.EqualTo(2));
            Assert.That(model.AssignedOperators, Contains.Item("Operator 1"));
            Assert.That(model.AssignedOperators, Contains.Item("Operator 2"));
        }

        #endregion

        #region SprintSummaryModel Tests

        [Test]
        public void SprintSummaryModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var model = new SprintSummaryModel();

            // Assert
            Assert.That(model.TotalTasksInSprint, Is.EqualTo(0));
            Assert.That(model.CompletedTasks, Is.EqualTo(0));
            Assert.That(model.InProgressTasks, Is.EqualTo(0));
            Assert.That(model.NotStartedTasks, Is.EqualTo(0));
            Assert.That(model.OverdueTasks, Is.EqualTo(0));
            Assert.That(model.TotalEstimatedHours, Is.EqualTo(0));
            Assert.That(model.CompletionPercentage, Is.EqualTo(0));
        }

        [Test]
        public void SprintSummaryModel_CalculatedProperties_WorkCorrectly()
        {
            // Arrange
            var model = new SprintSummaryModel
            {
                TotalTasksInSprint = 10,
                CompletedTasks = 8,
                OverdueTasks = 0
            };

            // Act & Assert
            Assert.That(model.CompletionPercentage, Is.EqualTo(80)); // 8/10 * 100 = 80%
            Assert.That(model.IsOnTrack, Is.True); // >= 80% and no overdue tasks

            // Test not on track due to low completion
            model.CompletedTasks = 6; // 60% completion
            Assert.That(model.IsOnTrack, Is.False);

            // Test not on track due to overdue tasks
            model.CompletedTasks = 8; // Back to 80%
            model.OverdueTasks = 2;
            Assert.That(model.IsOnTrack, Is.False);
        }

        [Test]
        public void SprintSummaryModel_GetSprintHealth_ReturnsCorrectStatus()
        {
            // Arrange & Act - Excellent (>= 90% and no overdue)
            var model1 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 9, OverdueTasks = 0 };
            Assert.That(model1.GetSprintHealth(), Is.EqualTo("Excellent"));

            // Good (>= 70% and no overdue)
            var model2 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 8, OverdueTasks = 0 };
            Assert.That(model2.GetSprintHealth(), Is.EqualTo("Good"));

            // Fair (>= 50% and no overdue)
            var model3 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 6, OverdueTasks = 0 };
            Assert.That(model3.GetSprintHealth(), Is.EqualTo("Fair"));

            // Poor (< 50% and no overdue)
            var model4 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 4, OverdueTasks = 0 };
            Assert.That(model4.GetSprintHealth(), Is.EqualTo("Poor"));

            // At Risk (any overdue tasks)
            var model5 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 9, OverdueTasks = 1 };
            Assert.That(model5.GetSprintHealth(), Is.EqualTo("At Risk"));
        }

        [Test]
        public void SprintSummaryModel_GetHealthColor_ReturnsCorrectColors()
        {
            // Arrange & Act
            var model1 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 9, OverdueTasks = 0 };
            Assert.That(model1.GetHealthColor(), Is.EqualTo("#10b981")); // Excellent - Green

            var model2 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 8, OverdueTasks = 0 };
            Assert.That(model2.GetHealthColor(), Is.EqualTo("#84cc16")); // Good - Light Green

            var model3 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 6, OverdueTasks = 0 };
            Assert.That(model3.GetHealthColor(), Is.EqualTo("#f59e0b")); // Fair - Yellow

            var model4 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 4, OverdueTasks = 0 };
            Assert.That(model4.GetHealthColor(), Is.EqualTo("#ef4444")); // Poor - Red

            var model5 = new SprintSummaryModel { TotalTasksInSprint = 10, CompletedTasks = 9, OverdueTasks = 1 };
            Assert.That(model5.GetHealthColor(), Is.EqualTo("#dc2626")); // At Risk - Dark Red
        }

        #endregion

        #region SprintTimelineModel Tests

        [Test]
        public void SprintTimelineModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var model = new SprintTimelineModel();

            // Assert
            Assert.That(model.Date, Is.EqualTo(default(DateTime)));
            Assert.That(model.TasksStarting, Is.Not.Null);
            Assert.That(model.TasksEnding, Is.Not.Null);
            Assert.That(model.TasksInProgress, Is.Not.Null);
            Assert.That(model.TotalHoursScheduled, Is.EqualTo(0));
            Assert.That(model.IsOverloaded, Is.False);
        }

        [Test]
        public void SprintTimelineModel_CalculatedProperties_WorkCorrectly()
        {
            // Arrange
            var model = new SprintTimelineModel
            {
                Date = new DateTime(2024, 1, 15), // Monday
                TotalHoursScheduled = 10,
                IsOverloaded = true // This is a property that gets set, not calculated
            };

            // Act & Assert
            Assert.That(model.IsWeekend, Is.False); // Monday
            Assert.That(model.IsOverloaded, Is.True); // Explicitly set

            // Test weekend
            model.Date = new DateTime(2024, 1, 13); // Saturday
            Assert.That(model.IsWeekend, Is.True);

            model.Date = new DateTime(2024, 1, 14); // Sunday
            Assert.That(model.IsWeekend, Is.True);

            // Test not overloaded
            model.IsOverloaded = false;
            Assert.That(model.IsOverloaded, Is.False);
        }

        [Test]
        public void SprintTimelineModel_GetDayStatus_ReturnsCorrectStatus()
        {
            // Arrange & Act
            var model = new SprintTimelineModel();

            // Weekend
            model.Date = new DateTime(2024, 1, 13); // Saturday
            Assert.That(model.GetDayStatus(), Is.EqualTo("Weekend"));

            // Weekday tests
            model.Date = new DateTime(2024, 1, 15); // Monday

            // Overloaded (IsOverloaded property must be set)
            model.IsOverloaded = true;
            Assert.That(model.GetDayStatus(), Is.EqualTo("Overloaded"));

            // Reset overloaded for other tests
            model.IsOverloaded = false;

            // Busy (> 6 hours)
            model.TotalHoursScheduled = 7;
            Assert.That(model.GetDayStatus(), Is.EqualTo("Busy"));

            // Moderate (> 3 hours)
            model.TotalHoursScheduled = 5;
            Assert.That(model.GetDayStatus(), Is.EqualTo("Moderate"));

            // Light (> 0 hours)
            model.TotalHoursScheduled = 2;
            Assert.That(model.GetDayStatus(), Is.EqualTo("Light"));

            // Free (0 hours)
            model.TotalHoursScheduled = 0;
            Assert.That(model.GetDayStatus(), Is.EqualTo("Free"));
        }

        [Test]
        public void SprintTimelineModel_GetStatusColor_ReturnsCorrectColors()
        {
            // Arrange
            var model = new SprintTimelineModel
            {
                Date = new DateTime(2024, 1, 15) // Monday
            };

            // Act & Assert
            model.TotalHoursScheduled = 0;
            model.IsOverloaded = false;
            Assert.That(model.GetStatusColor(), Is.EqualTo("#10b981")); // Free - Green

            model.TotalHoursScheduled = 2;
            Assert.That(model.GetStatusColor(), Is.EqualTo("#84cc16")); // Light - Light Green

            model.TotalHoursScheduled = 5;
            Assert.That(model.GetStatusColor(), Is.EqualTo("#f59e0b")); // Moderate - Yellow

            model.TotalHoursScheduled = 7;
            Assert.That(model.GetStatusColor(), Is.EqualTo("#f97316")); // Busy - Orange

            // For Overloaded, we need to set the IsOverloaded property
            model.IsOverloaded = true;
            Assert.That(model.GetStatusColor(), Is.EqualTo("#ef4444")); // Overloaded - Red

            model.Date = new DateTime(2024, 1, 13); // Saturday
            Assert.That(model.GetStatusColor(), Is.EqualTo("#6b7280")); // Weekend - Gray
        }

        [Test]
        public void SprintTimelineModel_WithTasks_WorksCorrectly()
        {
            // Arrange
            var model = new SprintTimelineModel
            {
                Date = DateTime.Today
            };

            var task1 = new SprintTaskServiceModel { Id = 1, Name = "Task 1", EstimatedTime = 8 };
            var task2 = new SprintTaskServiceModel { Id = 2, Name = "Task 2", EstimatedTime = 4 };

            // Act
            model.TasksStarting.Add(task1);
            model.TasksInProgress.Add(task1);
            model.TasksInProgress.Add(task2);
            model.TasksEnding.Add(task2);

            // Assert
            Assert.That(model.TasksStarting.Count, Is.EqualTo(1));
            Assert.That(model.TasksInProgress.Count, Is.EqualTo(2));
            Assert.That(model.TasksEnding.Count, Is.EqualTo(1));
            Assert.That(model.TasksStarting.First().Name, Is.EqualTo("Task 1"));
            Assert.That(model.TasksEnding.First().Name, Is.EqualTo("Task 2"));
        }

        #endregion
    }
}
