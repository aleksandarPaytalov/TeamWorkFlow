using NUnit.Framework;
using TeamWorkFlow.Core.Models.TimeTracking;

namespace UnitTests
{
    [TestFixture]
    public class TimeTrackingModelsUnitTests
    {
        #region TaskTimeTrackingModel Tests

        [Test]
        public void TaskTimeTrackingModel_TotalActualTimeFormatted_WithZeroMinutes_ReturnsZeroM()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                TotalActualTimeMinutes = 0
            };

            // Act
            var result = model.TotalActualTimeFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("0m"));
        }

        [Test]
        public void TaskTimeTrackingModel_TotalActualTimeFormatted_WithOnlyMinutes_ReturnsMinutesOnly()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                TotalActualTimeMinutes = 45
            };

            // Act
            var result = model.TotalActualTimeFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("45m"));
        }

        [Test]
        public void TaskTimeTrackingModel_TotalActualTimeFormatted_WithOnlyHours_ReturnsHoursOnly()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                TotalActualTimeMinutes = 120 // 2 hours
            };

            // Act
            var result = model.TotalActualTimeFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("2h"));
        }

        [Test]
        public void TaskTimeTrackingModel_TotalActualTimeFormatted_WithHoursAndMinutes_ReturnsFullFormat()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                TotalActualTimeMinutes = 135 // 2 hours 15 minutes
            };

            // Act
            var result = model.TotalActualTimeFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("2h 15m"));
        }

        [Test]
        public void TaskTimeTrackingModel_EstimatedTimeFormatted_WithDecimalHours_FormatsCorrectly()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                EstimatedTimeHours = 2.5m // 2.5 hours = 150 minutes
            };

            // Act
            var result = model.EstimatedTimeFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("2h 30m"));
        }

        [Test]
        public void TaskTimeTrackingModel_IsOverEstimate_WithActualTimeOverEstimate_ReturnsTrue()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                EstimatedTimeHours = 2, // 2 hours = 120 minutes
                TotalActualTimeMinutes = 150 // 2.5 hours
            };

            // Act
            var result = model.IsOverEstimate;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TaskTimeTrackingModel_IsOverEstimate_WithActualTimeUnderEstimate_ReturnsFalse()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                EstimatedTimeHours = 3, // 3 hours = 180 minutes
                TotalActualTimeMinutes = 120 // 2 hours
            };

            // Act
            var result = model.IsOverEstimate;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TaskTimeTrackingModel_IsSignificantlyOverEstimate_WithOver20Percent_ReturnsTrue()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                EstimatedTimeHours = 2, // 2 hours = 120 minutes
                TotalActualTimeMinutes = 180 // 3 hours (50% over)
            };

            // Act
            var result = model.IsSignificantlyOverEstimate;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TaskTimeTrackingModel_ProgressBarClass_WithNormalProgress_ReturnsBgSuccess()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                EstimatedTimeHours = 4, // 4 hours = 240 minutes
                TotalActualTimeMinutes = 200 // Under estimate
            };

            // Act
            var result = model.ProgressBarClass;

            // Assert
            Assert.That(result, Is.EqualTo("bg-success"));
        }

        [Test]
        public void TaskTimeTrackingModel_ProgressBarClass_WithOverEstimate_ReturnsBgWarning()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                EstimatedTimeHours = 2, // 2 hours = 120 minutes
                TotalActualTimeMinutes = 140 // Slightly over estimate
            };

            // Act
            var result = model.ProgressBarClass;

            // Assert
            Assert.That(result, Is.EqualTo("bg-warning"));
        }

        [Test]
        public void TaskTimeTrackingModel_ProgressBarClass_WithSignificantOverEstimate_ReturnsBgDanger()
        {
            // Arrange
            var model = new TaskTimeTrackingModel
            {
                EstimatedTimeHours = 2, // 2 hours = 120 minutes
                TotalActualTimeMinutes = 180 // 50% over estimate
            };

            // Act
            var result = model.ProgressBarClass;

            // Assert
            Assert.That(result, Is.EqualTo("bg-danger"));
        }

        #endregion

        #region WorkSessionModel Tests

        [Test]
        public void WorkSessionModel_DurationFormatted_WithZeroMinutes_ReturnsZeroM()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                DurationMinutes = 0
            };

            // Act
            var result = model.DurationFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("0m"));
        }

        [Test]
        public void WorkSessionModel_DurationFormatted_WithOnlyMinutes_ReturnsMinutesOnly()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                DurationMinutes = 30
            };

            // Act
            var result = model.DurationFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("30m"));
        }

        [Test]
        public void WorkSessionModel_DurationFormatted_WithHoursAndMinutes_ReturnsFullFormat()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                DurationMinutes = 90 // 1 hour 30 minutes
            };

            // Act
            var result = model.DurationFormatted;

            // Assert
            Assert.That(result, Is.EqualTo("1h 30m"));
        }

        [Test]
        public void WorkSessionModel_IsActive_WithoutEndTime_ReturnsTrue()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            // Act
            var result = model.IsActive;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void WorkSessionModel_IsActive_WithEndTime_ReturnsFalse()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                StartTime = DateTime.UtcNow.AddHours(-1),
                EndTime = DateTime.UtcNow
            };

            // Act
            var result = model.IsActive;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void WorkSessionModel_StatusText_WithActiveSession_ReturnsInProgress()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            // Act
            var result = model.StatusText;

            // Assert
            Assert.That(result, Is.EqualTo("In Progress"));
        }

        [Test]
        public void WorkSessionModel_StatusText_WithCompletedSession_ReturnsCompleted()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                StartTime = DateTime.UtcNow.AddHours(-1),
                EndTime = DateTime.UtcNow
            };

            // Act
            var result = model.StatusText;

            // Assert
            Assert.That(result, Is.EqualTo("Completed"));
        }

        [Test]
        public void WorkSessionModel_DurationClass_WithLongSession_ReturnsTextSuccess()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                DurationMinutes = 500 // Over 8 hours
            };

            // Act
            var result = model.DurationClass;

            // Assert
            Assert.That(result, Is.EqualTo("text-success fw-bold"));
        }

        [Test]
        public void WorkSessionModel_DurationClass_WithMediumSession_ReturnsTextWarning()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                DurationMinutes = 300 // 5 hours
            };

            // Act
            var result = model.DurationClass;

            // Assert
            Assert.That(result, Is.EqualTo("text-warning"));
        }

        [Test]
        public void WorkSessionModel_DurationClass_WithShortSession_ReturnsTextMuted()
        {
            // Arrange
            var model = new WorkSessionModel
            {
                DurationMinutes = 30 // 30 minutes
            };

            // Act
            var result = model.DurationClass;

            // Assert
            Assert.That(result, Is.EqualTo("text-muted"));
        }

        #endregion
    }
}
