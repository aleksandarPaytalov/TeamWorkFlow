using NUnit.Framework;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Models.Sprint;
using TeamWorkFlow.Core.Models.Summary;
using TeamWorkFlow.Core.Models.Operator;
using TeamWorkFlow.Core.Models.Pager;

namespace UnitTests
{
    [TestFixture]
    public class CoreConstantsAndModelsUnitTests
    {
        #region Messages Constants Tests

        [Test]
        public void Messages_ShouldHaveCorrectDateFormat()
        {
            // Assert
            Assert.That(Messages.DateFormat, Is.EqualTo("dd/MM/yyyy"));
        }

        [Test]
        public void Messages_ShouldHaveCorrectStringLengthMessage()
        {
            // Assert
            Assert.That(Messages.StringLength, Is.EqualTo("The field {0}, must be between {2} and {1} characters long."));
        }

        [Test]
        public void Messages_ShouldHaveCorrectRequiredMessage()
        {
            // Assert
            Assert.That(Messages.RequiredMessage, Is.EqualTo("The {0} field is required."));
        }

        [Test]
        public void Messages_ShouldHaveCorrectStringNumberRangeMessage()
        {
            // Assert
            Assert.That(Messages.StringNumberRange, Is.EqualTo("The field {0}, must be a positive number between {1} and {2}."));
        }

        [Test]
        public void Messages_ShouldHaveCorrectStatusNotExistingMessage()
        {
            // Assert
            Assert.That(Messages.StatusNotExisting, Is.EqualTo("Selected status does not exist"));
        }

        [Test]
        public void Messages_ShouldHaveCorrectPriorityNotExistingMessage()
        {
            // Assert
            Assert.That(Messages.PriorityNotExisting, Is.EqualTo("Selected status does not exist"));
        }

        [Test]
        public void Messages_ShouldHaveCorrectProjectMessages()
        {
            // Assert
            Assert.That(Messages.ProjectWithGivenNumberDoNotExist, Is.EqualTo("Project with this number do not exist"));
            Assert.That(Messages.ProjectWithThisNumberAlreadyCreated, Is.EqualTo("Project with this number already have been created"));
            Assert.That(Messages.ProjectNotExisting, Is.EqualTo("Project with this id do not exist"));
        }

        [Test]
        public void Messages_ShouldHaveCorrectValidationMessages()
        {
            // Assert
            Assert.That(Messages.InvalidDate, Is.EqualTo("Date is not valid. It must be in format {0}"));
            Assert.That(Messages.CapacityRange, Is.EqualTo("Capacity must be value between {0} and {1}"));
            Assert.That(Messages.BooleanInput, Is.EqualTo("Incorrect input. It must be true or false."));
            Assert.That(Messages.InvalidIdInput, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public void Messages_ShouldHaveCorrectOperatorMessages()
        {
            // Assert
            Assert.That(Messages.OperatorWithIdDoNotExist, Is.EqualTo("Operator with Id do not exist"));
        }

        [Test]
        public void Messages_ShouldHaveCorrectDateValidationMessage()
        {
            // Assert
            Assert.That(Messages.StartDateGreaterThanEndDateOrDeadLine, 
                Is.EqualTo("The start date cannot be greater than the end date or the deadline"));
        }

        [Test]
        public void Messages_ShouldHaveCorrectUserMessages()
        {
            // Assert
            Assert.That(Messages.UserWithEmailNotRegistered, Is.EqualTo("User with email is not registered in the application"));
            Assert.That(Messages.UserMessageSuccess, Is.EqualTo("UserMessageSuccess"));
            Assert.That(Messages.UserMessageError, Is.EqualTo("UserMessageError"));
        }

        [Test]
        public void Messages_ShouldHaveCorrectTotalHoursMessage()
        {
            // Assert
            Assert.That(Messages.TotalHoursNegative, Is.EqualTo("Total hours cannot be negative value"));
        }

        #endregion

        #region UsersConstants Tests

        [Test]
        public void UsersConstants_ShouldHaveCorrectRoleNames()
        {
            // Assert
            Assert.That(UsersConstants.AdminRole, Is.EqualTo("Administrator"));
            Assert.That(UsersConstants.OperatorRole, Is.EqualTo("Operator"));
            Assert.That(UsersConstants.GuestRole, Is.EqualTo("Guest"));
        }

        [Test]
        public void UsersConstants_ShouldHaveCorrectAdminEmail()
        {
            // Assert
            Assert.That(UsersConstants.AdminEmail, Is.EqualTo("ap.softuni@gmail.com"));
        }

        [Test]
        public void UsersConstants_ShouldHaveCorrectAreaName()
        {
            // Assert
            Assert.That(UsersConstants.AdminAreaName, Is.EqualTo("Admin"));
        }

        [Test]
        public void UsersConstants_ShouldHaveCorrectCacheKey()
        {
            // Assert
            Assert.That(UsersConstants.UserCacheKey, Is.EqualTo("UserCacheKey"));
        }

        [Test]
        public void UsersConstants_ShouldHaveCorrectOperatorEmails()
        {
            // Assert
            Assert.That(UsersConstants.OperatorEmails, Is.Not.Null);
            Assert.That(UsersConstants.OperatorEmails.Length, Is.EqualTo(2));
            Assert.That(UsersConstants.OperatorEmails, Does.Contain("jon.doe@softuni.bg"));
            Assert.That(UsersConstants.OperatorEmails, Does.Contain("jane.doe@softuni.bg"));
        }

        #endregion

        #region SprintCapacityServiceModel Tests

        [Test]
        public void SprintCapacityServiceModel_ShouldInitializeWithDefaults()
        {
            // Act
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
        public void SprintCapacityServiceModel_CanCompleteAllTasks_WhenSufficientCapacity_ReturnsTrue()
        {
            // Arrange
            var model = new SprintCapacityServiceModel
            {
                TotalOperatorHours = 100,
                TotalMachineHours = 80,
                RequiredOperatorHours = 90,
                RequiredMachineHours = 70
            };

            // Act & Assert
            Assert.That(model.CanCompleteAllTasks, Is.True);
        }

        [Test]
        public void SprintCapacityServiceModel_CanCompleteAllTasks_WhenInsufficientOperatorHours_ReturnsFalse()
        {
            // Arrange
            var model = new SprintCapacityServiceModel
            {
                TotalOperatorHours = 80,
                TotalMachineHours = 100,
                RequiredOperatorHours = 90,
                RequiredMachineHours = 70
            };

            // Act & Assert
            Assert.That(model.CanCompleteAllTasks, Is.False);
        }

        [Test]
        public void SprintCapacityServiceModel_CanCompleteAllTasks_WhenInsufficientMachineHours_ReturnsFalse()
        {
            // Arrange
            var model = new SprintCapacityServiceModel
            {
                TotalOperatorHours = 100,
                TotalMachineHours = 60,
                RequiredOperatorHours = 90,
                RequiredMachineHours = 70
            };

            // Act & Assert
            Assert.That(model.CanCompleteAllTasks, Is.False);
        }

        [Test]
        public void SprintCapacityServiceModel_CanCompleteAllTasks_WhenExactCapacity_ReturnsTrue()
        {
            // Arrange
            var model = new SprintCapacityServiceModel
            {
                TotalOperatorHours = 90,
                TotalMachineHours = 70,
                RequiredOperatorHours = 90,
                RequiredMachineHours = 70
            };

            // Act & Assert
            Assert.That(model.CanCompleteAllTasks, Is.True);
        }

        #endregion

        #region OperatorCapacityModel Tests

        [Test]
        public void OperatorCapacityModel_ShouldInitializeWithDefaults()
        {
            // Act
            var model = new OperatorCapacityModel();

            // Assert
            Assert.That(model.Id, Is.EqualTo(0));
            Assert.That(model.FullName, Is.EqualTo(string.Empty));
            Assert.That(model.AvailableHours, Is.EqualTo(0));
            Assert.That(model.AssignedHours, Is.EqualTo(0));
            Assert.That(model.IsActive, Is.False);
            Assert.That(model.AvailabilityStatus, Is.EqualTo(string.Empty));
        }

        [Test]
        public void OperatorCapacityModel_UtilizationPercentage_WithValidHours_ReturnsCorrectPercentage()
        {
            // Arrange
            var model = new OperatorCapacityModel
            {
                AvailableHours = 40,
                AssignedHours = 30
            };

            // Act & Assert
            Assert.That(model.UtilizationPercentage, Is.EqualTo(75.0));
        }

        [Test]
        public void OperatorCapacityModel_UtilizationPercentage_WithZeroAvailableHours_ReturnsZero()
        {
            // Arrange
            var model = new OperatorCapacityModel
            {
                AvailableHours = 0,
                AssignedHours = 10
            };

            // Act & Assert
            Assert.That(model.UtilizationPercentage, Is.EqualTo(0.0));
        }

        [Test]
        public void OperatorCapacityModel_RemainingHours_WithPositiveRemaining_ReturnsCorrectValue()
        {
            // Arrange
            var model = new OperatorCapacityModel
            {
                AvailableHours = 40,
                AssignedHours = 25
            };

            // Act & Assert
            Assert.That(model.RemainingHours, Is.EqualTo(15));
        }

        [Test]
        public void OperatorCapacityModel_RemainingHours_WithNegativeRemaining_ReturnsZero()
        {
            // Arrange
            var model = new OperatorCapacityModel
            {
                AvailableHours = 30,
                AssignedHours = 40
            };

            // Act & Assert
            Assert.That(model.RemainingHours, Is.EqualTo(0));
        }

        #endregion

        #region MachineCapacityModel Tests

        [Test]
        public void MachineCapacityModel_ShouldInitializeWithDefaults()
        {
            // Act
            var model = new MachineCapacityModel();

            // Assert
            Assert.That(model.Id, Is.EqualTo(0));
            Assert.That(model.Name, Is.EqualTo(string.Empty));
            Assert.That(model.AvailableHours, Is.EqualTo(0));
            Assert.That(model.AssignedHours, Is.EqualTo(0));
            Assert.That(model.IsActive, Is.False);
        }

        [Test]
        public void MachineCapacityModel_UtilizationPercentage_WithValidHours_ReturnsCorrectPercentage()
        {
            // Arrange
            var model = new MachineCapacityModel
            {
                AvailableHours = 50,
                AssignedHours = 20
            };

            // Act & Assert
            Assert.That(model.UtilizationPercentage, Is.EqualTo(40.0));
        }

        [Test]
        public void MachineCapacityModel_RemainingHours_WithPositiveRemaining_ReturnsCorrectValue()
        {
            // Arrange
            var model = new MachineCapacityModel
            {
                AvailableHours = 60,
                AssignedHours = 35
            };

            // Act & Assert
            Assert.That(model.RemainingHours, Is.EqualTo(25));
        }

        #endregion

        #region SummaryServiceModel Tests

        [Test]
        public void SummaryServiceModel_ShouldInitializeWithDefaults()
        {
            // Act
            var model = new SummaryServiceModel();

            // Assert
            Assert.That(model.TotalTasks, Is.EqualTo(0));
            Assert.That(model.FinishedTasks, Is.EqualTo(0));
            Assert.That(model.TotalProjects, Is.EqualTo(0));
            Assert.That(model.ProjectsInProduction, Is.EqualTo(0));
            Assert.That(model.TotalParts, Is.EqualTo(0));
            Assert.That(model.TotalApprovedParts, Is.EqualTo(0));
            Assert.That(model.TotalWorkers, Is.EqualTo(0));
            Assert.That(model.TotalAvailableWorkers, Is.EqualTo(0));
            Assert.That(model.TotalMachines, Is.EqualTo(0));
            Assert.That(model.TotalAvailableMachines, Is.EqualTo(0));
        }

        [Test]
        public void SummaryServiceModel_CanSetAllProperties()
        {
            // Arrange
            var model = new SummaryServiceModel();

            // Act
            model.TotalTasks = 100;
            model.FinishedTasks = 75;
            model.TotalProjects = 20;
            model.ProjectsInProduction = 15;
            model.TotalParts = 500;
            model.TotalApprovedParts = 450;
            model.TotalWorkers = 25;
            model.TotalAvailableWorkers = 20;
            model.TotalMachines = 10;
            model.TotalAvailableMachines = 8;

            // Assert
            Assert.That(model.TotalTasks, Is.EqualTo(100));
            Assert.That(model.FinishedTasks, Is.EqualTo(75));
            Assert.That(model.TotalProjects, Is.EqualTo(20));
            Assert.That(model.ProjectsInProduction, Is.EqualTo(15));
            Assert.That(model.TotalParts, Is.EqualTo(500));
            Assert.That(model.TotalApprovedParts, Is.EqualTo(450));
            Assert.That(model.TotalWorkers, Is.EqualTo(25));
            Assert.That(model.TotalAvailableWorkers, Is.EqualTo(20));
            Assert.That(model.TotalMachines, Is.EqualTo(10));
            Assert.That(model.TotalAvailableMachines, Is.EqualTo(8));
        }

        #endregion

        #region PaginatedOperatorsViewModel Tests

        [Test]
        public void PaginatedOperatorsViewModel_ShouldInitializeWithDefaults()
        {
            // Act
            var model = new PaginatedOperatorsViewModel();

            // Assert
            Assert.That(model.Operators, Is.Not.Null);
            Assert.That(model.Operators, Is.Empty);
            Assert.That(model.Pager, Is.Null);
        }

        [Test]
        public void PaginatedOperatorsViewModel_CanSetProperties()
        {
            // Arrange
            var operators = new List<OperatorServiceModel>
            {
                new OperatorServiceModel { Id = 1, FullName = "John Doe" },
                new OperatorServiceModel { Id = 2, FullName = "Jane Smith" }
            };
            var pager = new PagerServiceModel();
            var model = new PaginatedOperatorsViewModel();

            // Act
            model.Operators = operators;
            model.Pager = pager;

            // Assert
            Assert.That(model.Operators, Is.EqualTo(operators));
            Assert.That(model.Operators.Count(), Is.EqualTo(2));
            Assert.That(model.Pager, Is.EqualTo(pager));
        }

        #endregion
    }
}
