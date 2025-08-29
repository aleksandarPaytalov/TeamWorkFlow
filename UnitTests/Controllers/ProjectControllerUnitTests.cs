using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Controllers;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Project;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class ProjectControllerUnitTests
    {
        private ProjectController _controller = null!;
        private Mock<IProjectService> _mockProjectService = null!;
        private Mock<IPartService> _mockPartService = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<ClaimsPrincipal> _mockUser = null!;
        private Mock<ITempDataDictionary> _mockTempData = null!;

        [SetUp]
        public void SetUp()
        {
            _mockProjectService = new Mock<IProjectService>();
            _mockPartService = new Mock<IPartService>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockUser = new Mock<ClaimsPrincipal>();
            _mockTempData = new Mock<ITempDataDictionary>();

            _controller = new ProjectController(
                _mockProjectService.Object,
                _mockPartService.Object
            );

            var controllerContext = new ControllerContext()
            {
                HttpContext = _mockHttpContext.Object
            };

            _controller.ControllerContext = controllerContext;
            _controller.TempData = _mockTempData.Object;
            _mockHttpContext.Setup(x => x.User).Returns(_mockUser.Object);
        }

        [Test]
        public void ProjectController_ShouldInheritFromBaseController()
        {
            // Assert
            Assert.That(_controller, Is.InstanceOf<BaseController>());
        }

        [Test]
        public void ProjectController_ShouldHaveCorrectDependencies()
        {
            // Assert
            Assert.That(_controller, Is.Not.Null);
            // Dependencies are injected through constructor, so if controller is created successfully, dependencies are correct
        }

        #region Cost Calculation Tests

        [Test]
        public async Task CalculateCost_WithValidData_ShouldReturnSuccessJson()
        {
            // Arrange
            var projectId = 1;
            var hourlyRate = 50.00m;
            var currency = "USD";

            var mockCostCalculation = new ProjectCostCalculationModel
            {
                ProjectId = projectId,
                ProjectName = "Test Project",
                ProjectNumber = "TP001",
                CalculatedActualHours = 10.00,
                HourlyRate = hourlyRate
            };

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(true);
            _mockProjectService.Setup(x => x.CalculateProjectCostAsync(projectId, hourlyRate))
                .ReturnsAsync(mockCostCalculation);

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate, currency);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultValue = jsonResult.Value;
            Assert.That(resultValue, Is.Not.Null);

            // Use reflection to check the anonymous object properties
            var successProperty = resultValue.GetType().GetProperty("success");
            var totalLaborCostProperty = resultValue.GetType().GetProperty("totalLaborCost");
            var hourlyRateProperty = resultValue.GetType().GetProperty("hourlyRate");
            var currencyProperty = resultValue.GetType().GetProperty("currency");

            Assert.That(successProperty?.GetValue(resultValue), Is.EqualTo(true));
            Assert.That(totalLaborCostProperty?.GetValue(resultValue), Is.EqualTo("$500.00")); // 10.0 hours * $50.00
            Assert.That(hourlyRateProperty?.GetValue(resultValue), Is.EqualTo("$50.00"));
            Assert.That(currencyProperty?.GetValue(resultValue), Is.EqualTo("USD"));
        }

        [Test]
        public async Task CalculateCost_WithEuroCurrency_ShouldReturnEuroFormattedResult()
        {
            // Arrange
            var projectId = 1;
            var hourlyRate = 45.00m;
            var currency = "EUR";

            var mockCostCalculation = new ProjectCostCalculationModel
            {
                ProjectId = projectId,
                ProjectName = "Test Project",
                ProjectNumber = "TP001",
                CalculatedActualHours = 8.00,
                HourlyRate = hourlyRate
            };

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(true);
            _mockProjectService.Setup(x => x.CalculateProjectCostAsync(projectId, hourlyRate))
                .ReturnsAsync(mockCostCalculation);

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate, currency);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var resultValue = jsonResult?.Value;

            var totalLaborCostProperty = resultValue?.GetType().GetProperty("totalLaborCost");
            var hourlyRateProperty = resultValue?.GetType().GetProperty("hourlyRate");

            Assert.That(totalLaborCostProperty?.GetValue(resultValue), Is.EqualTo("€360.00")); // 8.0 hours * €45.00
            Assert.That(hourlyRateProperty?.GetValue(resultValue), Is.EqualTo("€45.00"));
        }

        [Test]
        public async Task CalculateCost_WithNonExistentProject_ShouldReturnErrorJson()
        {
            // Arrange
            var projectId = 999;
            var hourlyRate = 50.00m;
            var currency = "USD";

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate, currency);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var resultValue = jsonResult?.Value;

            var successProperty = resultValue?.GetType().GetProperty("success");
            var messageProperty = resultValue?.GetType().GetProperty("message");

            Assert.That(successProperty?.GetValue(resultValue), Is.EqualTo(false));
            Assert.That(messageProperty?.GetValue(resultValue), Is.EqualTo("Project not found."));
        }

        [Test]
        public async Task CalculateCost_WithZeroHourlyRate_ShouldReturnErrorJson()
        {
            // Arrange
            var projectId = 1;
            var hourlyRate = 0m;
            var currency = "USD";

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate, currency);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var resultValue = jsonResult?.Value;

            var successProperty = resultValue?.GetType().GetProperty("success");
            var messageProperty = resultValue?.GetType().GetProperty("message");

            Assert.That(successProperty?.GetValue(resultValue), Is.EqualTo(false));
            Assert.That(messageProperty?.GetValue(resultValue), Is.EqualTo("Hourly rate must be greater than 0."));
        }

        [Test]
        public async Task CalculateCost_WithNegativeHourlyRate_ShouldReturnErrorJson()
        {
            // Arrange
            var projectId = 1;
            var hourlyRate = -10m;
            var currency = "USD";

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate, currency);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var resultValue = jsonResult?.Value;

            var successProperty = resultValue?.GetType().GetProperty("success");
            var messageProperty = resultValue?.GetType().GetProperty("message");

            Assert.That(successProperty?.GetValue(resultValue), Is.EqualTo(false));
            Assert.That(messageProperty?.GetValue(resultValue), Is.EqualTo("Hourly rate must be greater than 0."));
        }

        [Test]
        public async Task CalculateCost_WithInvalidCurrency_ShouldReturnErrorJson()
        {
            // Arrange
            var projectId = 1;
            var hourlyRate = 50.00m;
            var currency = "GBP"; // Invalid currency

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate, currency);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var resultValue = jsonResult?.Value;

            var successProperty = resultValue?.GetType().GetProperty("success");
            var messageProperty = resultValue?.GetType().GetProperty("message");

            Assert.That(successProperty?.GetValue(resultValue), Is.EqualTo(false));
            Assert.That(messageProperty?.GetValue(resultValue), Is.EqualTo("Invalid currency selected."));
        }

        [Test]
        public async Task CalculateCost_WithServiceException_ShouldReturnErrorJson()
        {
            // Arrange
            var projectId = 1;
            var hourlyRate = 50.00m;
            var currency = "USD";

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(true);
            _mockProjectService.Setup(x => x.CalculateProjectCostAsync(projectId, hourlyRate))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate, currency);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var resultValue = jsonResult?.Value;

            var successProperty = resultValue?.GetType().GetProperty("success");
            var messageProperty = resultValue?.GetType().GetProperty("message");

            Assert.That(successProperty?.GetValue(resultValue), Is.EqualTo(false));
            Assert.That(messageProperty?.GetValue(resultValue), Is.EqualTo("Service error"));
        }

        [Test]
        public async Task CalculateCost_WithDefaultCurrency_ShouldUseUSD()
        {
            // Arrange
            var projectId = 1;
            var hourlyRate = 50.00m;
            // No currency parameter - should default to USD

            var mockCostCalculation = new ProjectCostCalculationModel
            {
                ProjectId = projectId,
                ProjectName = "Test Project",
                ProjectNumber = "TP001",
                CalculatedActualHours = 10.00,
                HourlyRate = hourlyRate
            };

            _mockProjectService.Setup(x => x.ProjectExistByIdAsync(projectId))
                .ReturnsAsync(true);
            _mockProjectService.Setup(x => x.CalculateProjectCostAsync(projectId, hourlyRate))
                .ReturnsAsync(mockCostCalculation);

            // Act
            var result = await _controller.CalculateCost(projectId, hourlyRate);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var resultValue = jsonResult?.Value;

            var currencyProperty = resultValue?.GetType().GetProperty("currency");
            var totalLaborCostProperty = resultValue?.GetType().GetProperty("totalLaborCost");

            Assert.That(currencyProperty?.GetValue(resultValue), Is.EqualTo("USD"));
            Assert.That(totalLaborCostProperty?.GetValue(resultValue), Is.EqualTo("$500.00")); // 10.0 hours * $50.00
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
    }
}
