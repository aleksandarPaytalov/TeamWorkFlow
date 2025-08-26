using NUnit.Framework;
using TeamWorkFlow.Core.Extensions;
using TeamWorkFlow.Core.Models.Operator;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Core.Models.Part;
using TeamWorkFlow.Core.Models.Project;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Core.Contracts;
using System.Text.RegularExpressions;

namespace UnitTests
{
    [TestFixture]
    public class CoreExtensionsUnitTests
    {
        #region OperatorModelExtension Tests

        [Test]
        public void GetOperatorExtension_WithValidOperatorModel_ReturnsCorrectExtension()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                FullName = "John Doe",
                Email = "john.doe@company.com"
            };

            // Act
            var result = operatorModel.GetOperatorExtension();

            // Assert
            Assert.That(result, Is.EqualTo("John-john"));
        }

        [Test]
        public void GetOperatorExtension_WithComplexEmail_ReturnsCorrectExtension()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                FullName = "Jane Smith",
                Email = "jane.smith123@company.co.uk"
            };

            // Act
            var result = operatorModel.GetOperatorExtension();

            // Assert
            Assert.That(result, Is.EqualTo("Jane-jane"));
        }

        [Test]
        public void GetOperatorExtension_WithSpecialCharacters_ReplacesWithDashes()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                FullName = "John O'Connor",
                Email = "john.o'connor@company.com"
            };

            // Act
            var result = operatorModel.GetOperatorExtension();

            // Assert
            Assert.That(result, Does.Match(@"^[a-zA-Z0-9\-]+$"));
            Assert.That(result, Does.Contain("John"));
        }

        [Test]
        public void GetCapacityPercentage_WithActiveOperatorFullCapacity_Returns100Percent()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = true,
                Capacity = 9 // Max working hours
            };

            // Act
            var result = operatorModel.GetCapacityPercentage();

            // Assert
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void GetCapacityPercentage_WithInactiveOperator_ReturnsZeroPercent()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = false,
                Capacity = 9
            };

            // Act
            var result = operatorModel.GetCapacityPercentage();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetCapacityPercentage_WithPartialCapacity_ReturnsCorrectPercentage()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = true,
                Capacity = 4 // 4 out of 9 hours
            };

            // Act
            var result = operatorModel.GetCapacityPercentage();

            // Assert
            Assert.That(result, Is.EqualTo(44)); // 4/9 * 100 = 44.44 rounded to 44
        }

        [Test]
        public void GetCapacityPercentage_WithExcessiveCapacity_ReturnsMaximum100Percent()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = true,
                Capacity = 15 // More than max 9 hours
            };

            // Act
            var result = operatorModel.GetCapacityPercentage();

            // Assert
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void GetCapacityDisplay_WithActiveOperator_ReturnsFormattedPercentage()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = true,
                Capacity = 6
            };

            // Act
            var result = operatorModel.GetCapacityDisplay();

            // Assert
            Assert.That(result, Is.EqualTo("67%"));
        }

        [Test]
        public void GetCapacityLevel_WithZeroCapacity_ReturnsZero()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = false,
                Capacity = 9
            };

            // Act
            var result = operatorModel.GetCapacityLevel();

            // Assert
            Assert.That(result, Is.EqualTo("zero"));
        }

        [Test]
        public void GetCapacityLevel_WithHighCapacity_ReturnsHigh()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = true,
                Capacity = 8 // 89% capacity
            };

            // Act
            var result = operatorModel.GetCapacityLevel();

            // Assert
            Assert.That(result, Is.EqualTo("high"));
        }

        [Test]
        public void GetCapacityLevel_WithMediumCapacity_ReturnsMedium()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = true,
                Capacity = 5 // 56% capacity
            };

            // Act
            var result = operatorModel.GetCapacityLevel();

            // Assert
            Assert.That(result, Is.EqualTo("medium"));
        }

        [Test]
        public void GetCapacityLevel_WithLowCapacity_ReturnsLow()
        {
            // Arrange
            var operatorModel = new OperatorServiceModel
            {
                IsActive = true,
                Capacity = 2 // 22% capacity
            };

            // Act
            var result = operatorModel.GetCapacityLevel();

            // Assert
            Assert.That(result, Is.EqualTo("low"));
        }

        [Test]
        public void GetOperatorExtension_WithOperatorFormModel_ReturnsCorrectExtension()
        {
            // Arrange
            var operatorFormModel = new OperatorFormModel
            {
                FullName = "Alice Johnson",
                Email = "alice.johnson@company.com"
            };

            // Act
            var result = operatorFormModel.GetOperatorExtension();

            // Assert
            Assert.That(result, Is.EqualTo("Alice-alice"));
        }

        #endregion

        #region MachineModelExtension Tests

        [Test]
        public void GetMachineExtension_WithSingleWordName_ReturnsFirstWord()
        {
            // Arrange
            var machineModel = new MachineServiceModel
            {
                Name = "Lathe"
            };

            // Act
            var result = machineModel.GetMachineExtension();

            // Assert
            Assert.That(result, Is.EqualTo("Lathe"));
        }

        [Test]
        public void GetMachineExtension_WithMultipleWords_ReturnsFirstWordOnly()
        {
            // Arrange
            var machineModel = new MachineServiceModel
            {
                Name = "CNC Milling Machine"
            };

            // Act
            var result = machineModel.GetMachineExtension();

            // Assert
            Assert.That(result, Is.EqualTo("CNC"));
        }

        [Test]
        public void GetMachineExtension_WithExtraSpaces_HandlesCorrectly()
        {
            // Arrange
            var machineModel = new MachineServiceModel
            {
                Name = "  Drill   Press  "
            };

            // Act
            var result = machineModel.GetMachineExtension();

            // Assert
            Assert.That(result, Is.EqualTo("Drill"));
        }

        #endregion

        #region TaskModelExtensions Tests

        [Test]
        public void GetTaskExtension_WithValidTaskModel_ReturnsCorrectExtension()
        {
            // Arrange
            var taskModel = new TaskServiceModel
            {
                Name = "Machine Setup",
                Description = "Setup the CNC machine for production run"
            };

            // Act
            var result = taskModel.GetTaskExtension();

            // Assert
            Assert.That(result, Is.EqualTo("Machine-SetupSetup-the"));
        }

        [Test]
        public void GetTaskExtension_WithSpecialCharacters_ReplacesWithDashes()
        {
            // Arrange
            var taskModel = new TaskServiceModel
            {
                Name = "Quality Check & Test",
                Description = "Perform quality check & testing procedures"
            };

            // Act
            var result = taskModel.GetTaskExtension();

            // Assert
            Assert.That(result, Does.Match(@"^[a-zA-Z0-9\-]+$"));
            Assert.That(result, Does.Contain("Quality"));
        }

        [Test]
        public void GetTaskExtension_WithShortDescription_HandlesCorrectly()
        {
            // Arrange
            var taskModel = new TaskServiceModel
            {
                Name = "Test",
                Description = "Quick"
            };

            // Act
            var result = taskModel.GetTaskExtension();

            // Assert
            Assert.That(result, Is.EqualTo("TestQuick"));
        }

        #endregion

        #region PartModelExtension Tests

        [Test]
        public void GetPartExtension_WithValidPartModel_ReturnsCorrectExtension()
        {
            // Arrange
            var partModel = new PartServiceModel
            {
                Name = "Engine Block",
                PartArticleNumber = "EB-001"
            };

            // Act
            var result = partModel.GetPartExtension();

            // Assert
            Assert.That(result, Is.EqualTo("Engine-BlockEB-001"));
        }

        [Test]
        public void GetPartExtension_WithSpecialCharacters_ReplacesWithDashes()
        {
            // Arrange
            var partModel = new PartServiceModel
            {
                Name = "Brake Pad & Rotor",
                PartArticleNumber = "BP&R-123"
            };

            // Act
            var result = partModel.GetPartExtension();

            // Assert
            Assert.That(result, Does.Match(@"^[a-zA-Z0-9\-]+$"));
        }

        #endregion

        #region ProjectModelExtension Tests

        [Test]
        public void GetProjectExtension_WithValidProjectModel_ReturnsCorrectExtension()
        {
            // Arrange
            var projectModel = new ProjectServiceModel
            {
                ProjectName = "New Product Line",
                ProjectNumber = "NPL-2024"
            };

            // Act
            var result = projectModel.GetProjectExtension();

            // Assert
            Assert.That(result, Is.EqualTo("New-Product-LineNPL-2024"));
        }

        [Test]
        public void GetProjectExtension_WithSpecialCharacters_ReplacesWithDashes()
        {
            // Arrange
            var projectModel = new ProjectServiceModel
            {
                ProjectName = "R&D Project #1",
                ProjectNumber = "RD-001"
            };

            // Act
            var result = projectModel.GetProjectExtension();

            // Assert
            Assert.That(result, Does.Match(@"^[a-zA-Z0-9\-]+$"));
        }

        #endregion
    }
}
