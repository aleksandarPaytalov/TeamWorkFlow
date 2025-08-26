using NUnit.Framework;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Exceptions;
using System;

namespace UnitTests
{
    [TestFixture]
    public class CoreEnumerationsAndExceptionsUnitTests
    {
        #region TaskSorting Enumeration Tests

        [Test]
        public void TaskSorting_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.That((int)TaskSorting.LastAdded, Is.EqualTo(0));
            Assert.That((int)TaskSorting.NameAscending, Is.EqualTo(1));
            Assert.That((int)TaskSorting.NameDescending, Is.EqualTo(2));
            Assert.That((int)TaskSorting.ProjectNumberAscending, Is.EqualTo(3));
            Assert.That((int)TaskSorting.ProjectNumberDescending, Is.EqualTo(4));
            Assert.That((int)TaskSorting.StartDateAscending, Is.EqualTo(5));
            Assert.That((int)TaskSorting.StartDateDescending, Is.EqualTo(6));
            Assert.That((int)TaskSorting.DeadlineAscending, Is.EqualTo(7));
            Assert.That((int)TaskSorting.DeadlineDescending, Is.EqualTo(8));
        }

        [Test]
        public void TaskSorting_ShouldHaveAllExpectedValues()
        {
            // Arrange
            var expectedValues = new[]
            {
                TaskSorting.LastAdded,
                TaskSorting.NameAscending,
                TaskSorting.NameDescending,
                TaskSorting.ProjectNumberAscending,
                TaskSorting.ProjectNumberDescending,
                TaskSorting.StartDateAscending,
                TaskSorting.StartDateDescending,
                TaskSorting.DeadlineAscending,
                TaskSorting.DeadlineDescending
            };

            // Act
            var allValues = Enum.GetValues<TaskSorting>();

            // Assert
            Assert.That(allValues.Length, Is.EqualTo(expectedValues.Length));
            foreach (var expectedValue in expectedValues)
            {
                Assert.That(allValues, Does.Contain(expectedValue));
            }
        }

        [Test]
        public void TaskSorting_CanBeConvertedToString()
        {
            // Act & Assert
            Assert.That(TaskSorting.LastAdded.ToString(), Is.EqualTo("LastAdded"));
            Assert.That(TaskSorting.NameAscending.ToString(), Is.EqualTo("NameAscending"));
            Assert.That(TaskSorting.DeadlineDescending.ToString(), Is.EqualTo("DeadlineDescending"));
        }

        #endregion

        #region MachineSorting Enumeration Tests

        [Test]
        public void MachineSorting_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.That((int)MachineSorting.LastAdded, Is.EqualTo(0));
            Assert.That((int)MachineSorting.NameAscending, Is.EqualTo(1));
            Assert.That((int)MachineSorting.NameDescending, Is.EqualTo(2));
            Assert.That((int)MachineSorting.CalibrationDateAscending, Is.EqualTo(3));
            Assert.That((int)MachineSorting.CalibrationDateDescending, Is.EqualTo(4));
            Assert.That((int)MachineSorting.CapacityAscending, Is.EqualTo(5));
            Assert.That((int)MachineSorting.CapacityDescending, Is.EqualTo(6));
        }

        [Test]
        public void MachineSorting_ShouldHaveAllExpectedValues()
        {
            // Arrange
            var expectedValues = new[]
            {
                MachineSorting.LastAdded,
                MachineSorting.NameAscending,
                MachineSorting.NameDescending,
                MachineSorting.CalibrationDateAscending,
                MachineSorting.CalibrationDateDescending,
                MachineSorting.CapacityAscending,
                MachineSorting.CapacityDescending
            };

            // Act
            var allValues = Enum.GetValues<MachineSorting>();

            // Assert
            Assert.That(allValues.Length, Is.EqualTo(expectedValues.Length));
            foreach (var expectedValue in expectedValues)
            {
                Assert.That(allValues, Does.Contain(expectedValue));
            }
        }

        #endregion

        #region OperatorSorting Enumeration Tests

        [Test]
        public void OperatorSorting_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.That((int)OperatorSorting.LastAdded, Is.EqualTo(0));
            Assert.That((int)OperatorSorting.NameAscending, Is.EqualTo(1));
            Assert.That((int)OperatorSorting.NameDescending, Is.EqualTo(2));
            Assert.That((int)OperatorSorting.EmailAscending, Is.EqualTo(3));
            Assert.That((int)OperatorSorting.EmailDescending, Is.EqualTo(4));
            Assert.That((int)OperatorSorting.CapacityAscending, Is.EqualTo(5));
            Assert.That((int)OperatorSorting.CapacityDescending, Is.EqualTo(6));
            Assert.That((int)OperatorSorting.StatusAscending, Is.EqualTo(7));
            Assert.That((int)OperatorSorting.StatusDescending, Is.EqualTo(8));
        }

        [Test]
        public void OperatorSorting_ShouldHaveAllExpectedValues()
        {
            // Arrange
            var expectedValues = new[]
            {
                OperatorSorting.LastAdded,
                OperatorSorting.NameAscending,
                OperatorSorting.NameDescending,
                OperatorSorting.EmailAscending,
                OperatorSorting.EmailDescending,
                OperatorSorting.CapacityAscending,
                OperatorSorting.CapacityDescending,
                OperatorSorting.StatusAscending,
                OperatorSorting.StatusDescending
            };

            // Act
            var allValues = Enum.GetValues<OperatorSorting>();

            // Assert
            Assert.That(allValues.Length, Is.EqualTo(expectedValues.Length));
            foreach (var expectedValue in expectedValues)
            {
                Assert.That(allValues, Does.Contain(expectedValue));
            }
        }

        #endregion

        #region ProjectSorting Enumeration Tests

        [Test]
        public void ProjectSorting_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.That((int)ProjectSorting.LastAdded, Is.EqualTo(0));
            Assert.That((int)ProjectSorting.NameAscending, Is.EqualTo(1));
            Assert.That((int)ProjectSorting.NameDescending, Is.EqualTo(2));
            Assert.That((int)ProjectSorting.ProjectNumberAscending, Is.EqualTo(3));
            Assert.That((int)ProjectSorting.ProjectNumberDescending, Is.EqualTo(4));
            Assert.That((int)ProjectSorting.StatusAscending, Is.EqualTo(5));
            Assert.That((int)ProjectSorting.StatusDescending, Is.EqualTo(6));
            Assert.That((int)ProjectSorting.TotalPartsAscending, Is.EqualTo(7));
            Assert.That((int)ProjectSorting.TotalPartsDescending, Is.EqualTo(8));
        }

        [Test]
        public void ProjectSorting_ShouldHaveAllExpectedValues()
        {
            // Arrange
            var expectedValues = new[]
            {
                ProjectSorting.LastAdded,
                ProjectSorting.NameAscending,
                ProjectSorting.NameDescending,
                ProjectSorting.ProjectNumberAscending,
                ProjectSorting.ProjectNumberDescending,
                ProjectSorting.StatusAscending,
                ProjectSorting.StatusDescending,
                ProjectSorting.TotalPartsAscending,
                ProjectSorting.TotalPartsDescending
            };

            // Act
            var allValues = Enum.GetValues<ProjectSorting>();

            // Assert
            Assert.That(allValues.Length, Is.EqualTo(expectedValues.Length));
            foreach (var expectedValue in expectedValues)
            {
                Assert.That(allValues, Does.Contain(expectedValue));
            }
        }

        #endregion

        #region PartSorting Enumeration Tests

        [Test]
        public void PartSorting_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.That((int)PartSorting.LastAdded, Is.EqualTo(0));
            Assert.That((int)PartSorting.ProjectNumberAscending, Is.EqualTo(1));
            Assert.That((int)PartSorting.ProjectNumberDescending, Is.EqualTo(2));
        }

        [Test]
        public void PartSorting_ShouldHaveAllExpectedValues()
        {
            // Arrange
            var expectedValues = new[]
            {
                PartSorting.LastAdded,
                PartSorting.ProjectNumberAscending,
                PartSorting.ProjectNumberDescending
            };

            // Act
            var allValues = Enum.GetValues<PartSorting>();

            // Assert
            Assert.That(allValues.Length, Is.EqualTo(expectedValues.Length));
            foreach (var expectedValue in expectedValues)
            {
                Assert.That(allValues, Does.Contain(expectedValue));
            }
        }

        #endregion

        #region UnExistingActionException Tests

        [Test]
        public void UnExistingActionException_DefaultConstructor_CreatesException()
        {
            // Act
            var exception = new UnExistingActionException();

            // Assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception, Is.InstanceOf<Exception>());
            Assert.That(exception.Message, Is.Not.Null);
        }

        [Test]
        public void UnExistingActionException_WithMessage_CreatesExceptionWithMessage()
        {
            // Arrange
            var expectedMessage = "Test exception message";

            // Act
            var exception = new UnExistingActionException(expectedMessage);

            // Assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception, Is.InstanceOf<Exception>());
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void UnExistingActionException_CanBeThrown()
        {
            // Arrange
            var expectedMessage = "Action does not exist";

            // Act & Assert
            var exception = Assert.Throws<UnExistingActionException>(() =>
            {
                throw new UnExistingActionException(expectedMessage);
            });

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void UnExistingActionException_CanBeCaught()
        {
            // Arrange
            var expectedMessage = "Action not found";
            var exceptionCaught = false;

            // Act
            try
            {
                throw new UnExistingActionException(expectedMessage);
            }
            catch (UnExistingActionException ex)
            {
                exceptionCaught = true;
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }

            // Assert
            Assert.That(exceptionCaught, Is.True);
        }

        [Test]
        public void UnExistingActionException_InheritsFromException()
        {
            // Arrange
            var exception = new UnExistingActionException("Test");

            // Act & Assert
            Assert.That(exception, Is.InstanceOf<Exception>());
            Assert.That(exception.GetType().BaseType, Is.EqualTo(typeof(Exception)));
        }

        #endregion
    }
}
