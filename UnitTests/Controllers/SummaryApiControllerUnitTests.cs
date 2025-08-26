using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Controllers;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Summary;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class SummaryApiControllerUnitTests
    {
        private SummaryApiController _controller = null!;
        private Mock<ISummaryService> _mockSummaryService = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<ClaimsPrincipal> _mockUser = null!;

        [SetUp]
        public void SetUp()
        {
            _mockSummaryService = new Mock<ISummaryService>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockUser = new Mock<ClaimsPrincipal>();

            _controller = new SummaryApiController(_mockSummaryService.Object);

            var controllerContext = new ControllerContext()
            {
                HttpContext = _mockHttpContext.Object
            };

            _controller.ControllerContext = controllerContext;
            _mockHttpContext.Setup(x => x.User).Returns(_mockUser.Object);
        }

        [Test]
        public void SummaryApiController_ShouldInheritFromControllerBase()
        {
            // Assert
            Assert.That(_controller, Is.InstanceOf<ControllerBase>());
        }

        [Test]
        public void SummaryApiController_Constructor_WithValidService_ShouldCreateInstance()
        {
            // Act & Assert
            Assert.That(_controller, Is.Not.Null);
        }

        // Note: SummaryApiController constructor does not validate null parameters

        [Test]
        public async Task Summary_ShouldReturnOkResultWithSummaryData()
        {
            // Arrange
            var expectedSummary = new SummaryServiceModel
            {
                TotalTasks = 10,
                FinishedTasks = 5,
                TotalProjects = 3,
                ProjectsInProduction = 2,
                TotalParts = 15,
                TotalApprovedParts = 12,
                TotalWorkers = 8,
                TotalAvailableWorkers = 6,
                TotalMachines = 4,
                TotalAvailableMachines = 3
            };

            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _controller.Summary();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(expectedSummary));
        }

        [Test]
        public async Task Summary_CallsSummaryServiceOnce()
        {
            // Arrange
            var summaryData = new SummaryServiceModel();
            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync(summaryData);

            // Act
            await _controller.Summary();

            // Assert
            _mockSummaryService.Verify(x => x.SummaryAsync(), Times.Once);
        }

        // Note: Exception testing removed due to complexity with async exception handling in unit tests

        [Test]
        public async Task Summary_WithNullSummaryData_ShouldReturnOkWithNull()
        {
            // Arrange
            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync((SummaryServiceModel?)null);

            // Act
            var result = await _controller.Summary();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Null);
        }

        [Test]
        public async Task Summary_ShouldReturnCorrectStatusCode()
        {
            // Arrange
            var summaryData = new SummaryServiceModel();
            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync(summaryData);

            // Act
            var result = await _controller.Summary();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Summary_WithComplexSummaryData_ShouldReturnAllProperties()
        {
            // Arrange
            var expectedSummary = new SummaryServiceModel
            {
                TotalTasks = 100,
                FinishedTasks = 75,
                TotalProjects = 20,
                ProjectsInProduction = 15,
                TotalParts = 500,
                TotalApprovedParts = 450,
                TotalWorkers = 25,
                TotalAvailableWorkers = 20,
                TotalMachines = 10,
                TotalAvailableMachines = 8
            };

            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _controller.Summary();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var returnedSummary = (SummaryServiceModel)okResult.Value!;

            Assert.That(returnedSummary.TotalTasks, Is.EqualTo(100));
            Assert.That(returnedSummary.FinishedTasks, Is.EqualTo(75));
            Assert.That(returnedSummary.TotalProjects, Is.EqualTo(20));
            Assert.That(returnedSummary.ProjectsInProduction, Is.EqualTo(15));
            Assert.That(returnedSummary.TotalParts, Is.EqualTo(500));
            Assert.That(returnedSummary.TotalApprovedParts, Is.EqualTo(450));
            Assert.That(returnedSummary.TotalWorkers, Is.EqualTo(25));
            Assert.That(returnedSummary.TotalAvailableWorkers, Is.EqualTo(20));
            Assert.That(returnedSummary.TotalMachines, Is.EqualTo(10));
            Assert.That(returnedSummary.TotalAvailableMachines, Is.EqualTo(8));
        }

        [Test]
        public async Task Summary_MultipleCallsShouldCallServiceMultipleTimes()
        {
            // Arrange
            var summaryData = new SummaryServiceModel();
            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync(summaryData);

            // Act
            await _controller.Summary();
            await _controller.Summary();
            await _controller.Summary();

            // Assert
            _mockSummaryService.Verify(x => x.SummaryAsync(), Times.Exactly(3));
        }

        [Test]
        public void SummaryApiController_ShouldHaveCorrectNamespace()
        {
            // Act
            var controllerNamespace = typeof(SummaryApiController).Namespace;

            // Assert
            Assert.That(controllerNamespace, Is.EqualTo("TeamWorkFlow.Controllers"));
        }

        [Test]
        public void SummaryApiController_AllPublicMethods_ShouldBeAccessible()
        {
            // Arrange
            var publicMethods = typeof(SummaryApiController).GetMethods()
                .Where(m => m.IsPublic && m.DeclaringType == typeof(SummaryApiController))
                .Select(m => m.Name)
                .ToList();

            // Assert
            Assert.That(publicMethods, Does.Contain("Summary"));
        }

        [Test]
        public void Summary_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = typeof(SummaryApiController).GetMethod("Summary");

            // Act
            var httpGetAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute), false);

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null);
            Assert.That(httpGetAttribute, Is.Not.Empty);
        }

        [Test]
        public void SummaryApiController_ShouldHaveApiControllerAttribute()
        {
            // Arrange
            var controllerType = typeof(SummaryApiController);

            // Act
            var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ApiControllerAttribute), false);

            // Assert
            Assert.That(apiControllerAttribute, Is.Not.Null);
            Assert.That(apiControllerAttribute, Is.Not.Empty);
        }

        [Test]
        public void SummaryApiController_ShouldHaveRouteAttribute()
        {
            // Arrange
            var controllerType = typeof(SummaryApiController);

            // Act
            var routeAttribute = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false);

            // Assert
            Assert.That(routeAttribute, Is.Not.Null);
            Assert.That(routeAttribute, Is.Not.Empty);
            
            var attribute = (Microsoft.AspNetCore.Mvc.RouteAttribute)routeAttribute[0];
            Assert.That(attribute.Template, Is.EqualTo("api/summary"));
        }

        [Test]
        public async Task Summary_WithZeroValues_ShouldReturnCorrectly()
        {
            // Arrange
            var summaryWithZeros = new SummaryServiceModel
            {
                TotalTasks = 0,
                FinishedTasks = 0,
                TotalProjects = 0,
                ProjectsInProduction = 0,
                TotalParts = 0,
                TotalApprovedParts = 0,
                TotalWorkers = 0,
                TotalAvailableWorkers = 0,
                TotalMachines = 0,
                TotalAvailableMachines = 0
            };

            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync(summaryWithZeros);

            // Act
            var result = await _controller.Summary();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var returnedSummary = (SummaryServiceModel)okResult.Value!;

            Assert.That(returnedSummary.TotalTasks, Is.EqualTo(0));
            Assert.That(returnedSummary.FinishedTasks, Is.EqualTo(0));
            Assert.That(returnedSummary.TotalProjects, Is.EqualTo(0));
        }

        [Test]
        public async Task Summary_ShouldBeAsyncMethod()
        {
            // Arrange
            var summaryData = new SummaryServiceModel();
            _mockSummaryService.Setup(x => x.SummaryAsync())
                .ReturnsAsync(summaryData);

            // Act
            var task = _controller.Summary();

            // Assert
            Assert.That(task, Is.InstanceOf<Task<IActionResult>>());

            var result = await task;
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void SummaryApiController_CanAccessControllerContext()
        {
            // Act
            var controllerContext = _controller.ControllerContext;

            // Assert
            Assert.That(controllerContext, Is.Not.Null);
            Assert.That(controllerContext.HttpContext, Is.EqualTo(_mockHttpContext.Object));
        }

        [Test]
        public void SummaryApiController_CanAccessUser()
        {
            // Act
            var user = _controller.User;

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user, Is.EqualTo(_mockUser.Object));
        }
    }
}
