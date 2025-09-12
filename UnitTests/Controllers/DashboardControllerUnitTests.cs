using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TeamWorkFlow.Controllers;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Dashboard;
using TeamWorkFlow.Extensions;

namespace UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for DashboardController - testing new dashboard functionalities
    /// </summary>
    [TestFixture]
    public class DashboardControllerUnitTests
    {
        private Mock<ITaskAnalyticsService> _mockAnalyticsService;
        private Mock<IReportService> _mockReportService;
        private Mock<ILogger<DashboardController>> _mockLogger;
        private Mock<ClaimsPrincipal> _mockUser;
        private DashboardController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAnalyticsService = new Mock<ITaskAnalyticsService>();
            _mockReportService = new Mock<IReportService>();
            _mockLogger = new Mock<ILogger<DashboardController>>();
            _mockUser = new Mock<ClaimsPrincipal>();

            _controller = new DashboardController(
                _mockAnalyticsService.Object,
                _mockReportService.Object,
                _mockLogger.Object);

            // Setup controller context
            var httpContext = new DefaultHttpContext();
            httpContext.User = _mockUser.Object;
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void DashboardController_Constructor_WithValidServices_ShouldCreateInstance()
        {
            // Act & Assert
            Assert.That(_controller, Is.Not.Null);
        }

        [Test]
        public void DashboardController_Constructor_WithNullAnalyticsService_ShouldNotThrow()
        {
            // Act & Assert - Constructor doesn't validate null parameters
            Assert.DoesNotThrow(() => new DashboardController(
                null!,
                _mockReportService.Object,
                _mockLogger.Object));
        }

        [Test]
        public void DashboardController_Constructor_WithNullReportService_ShouldNotThrow()
        {
            // Act & Assert - Constructor doesn't validate null parameters
            Assert.DoesNotThrow(() => new DashboardController(
                _mockAnalyticsService.Object,
                null!,
                _mockLogger.Object));
        }

        [Test]
        public void DashboardController_Constructor_WithNullLogger_ShouldNotThrow()
        {
            // Act & Assert - Constructor doesn't validate null parameters
            Assert.DoesNotThrow(() => new DashboardController(
                _mockAnalyticsService.Object,
                _mockReportService.Object,
                null!));
        }

        #endregion

        #region Helper Methods

        private void SetupAuthenticatedAdminUser()
        {
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            // Mock role claims for Admin role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Administrator")
            };
            _mockUser.Setup(x => x.Claims).Returns(claims);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);
        }

        private void SetupAuthenticatedOperatorUser()
        {
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            // Mock role claims for Operator role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Operator")
            };
            _mockUser.Setup(x => x.Claims).Returns(claims);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(true);
        }

        #endregion

        #region Index Action Tests

        [Test]
        public async System.Threading.Tasks.Task Index_WithUnauthenticatedUser_ShouldReturnChallenge()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public async System.Threading.Tasks.Task Index_WithGuestUser_ShouldReturnViewWithDashboardData()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var expectedDashboard = new PerformanceDashboardModel();
            _mockAnalyticsService.Setup(x => x.GetDashboardDataAsync(It.IsAny<ReportFilterModel>()))
                .ReturnsAsync(expectedDashboard);

            // Act
            var result = await _controller.Index(null);

            // Assert - Since role checks are commented out, all authenticated users get access
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Index_WithAdminUser_ShouldReturnViewWithDashboardData()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var expectedDashboard = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 10,
                TotalOperatorsAnalyzed = 3,
                LastUpdated = DateTime.UtcNow
            };

            _mockAnalyticsService.Setup(x => x.GetDashboardDataAsync(It.IsAny<ReportFilterModel>()))
                .ReturnsAsync(expectedDashboard);

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(expectedDashboard));
        }

        [Test]
        public async Task Index_WithOperatorUser_ShouldReturnViewWithDashboardData()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var expectedDashboard = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 5,
                TotalOperatorsAnalyzed = 1,
                LastUpdated = DateTime.UtcNow
            };

            _mockAnalyticsService.Setup(x => x.GetDashboardDataAsync(It.IsAny<ReportFilterModel>()))
                .ReturnsAsync(expectedDashboard);

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(expectedDashboard));
        }

        [Test]
        public async Task Index_WhenAnalyticsServiceThrows_ShouldReturnViewWithEmptyModel()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            // Mock TempData to avoid null reference
            var mockTempData = new Mock<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary>();
            _controller.TempData = mockTempData.Object;

            _mockAnalyticsService.Setup(x => x.GetDashboardDataAsync(It.IsAny<ReportFilterModel>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());

            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<PerformanceDashboardModel>());
        }

        #endregion

        #region RefreshData Action Tests

        [Test]
        public async Task RefreshData_WithUnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var filters = new ReportFilterModel();

            // Act
            var result = await _controller.RefreshData(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task RefreshData_WithValidFilters_ShouldReturnPartialView()
        {
            // Arrange
            SetupAuthenticatedAdminUser();

            var filters = new ReportFilterModel
            {
                FromDate = DateTime.Today.AddDays(-30),
                ToDate = DateTime.Today
            };

            var expectedDashboard = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 15,
                LastUpdated = DateTime.UtcNow
            };

            _mockAnalyticsService.Setup(x => x.GetDashboardDataAsync(filters))
                .ReturnsAsync(expectedDashboard);

            // Act
            var result = await _controller.RefreshData(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<PartialViewResult>());

            var partialViewResult = (PartialViewResult)result;
            Assert.That(partialViewResult.ViewName, Is.EqualTo("_DashboardContent"));
            Assert.That(partialViewResult.Model, Is.EqualTo(expectedDashboard));
        }

        [Test]
        public async Task RefreshData_WhenAnalyticsServiceReturnsValidData_ShouldReturnPartialView()
        {
            // Arrange
            SetupAuthenticatedAdminUser();

            var filters = new ReportFilterModel();
            var dashboardData = new PerformanceDashboardModel();

            _mockAnalyticsService.Setup(x => x.GetDashboardDataAsync(filters))
                .ReturnsAsync(dashboardData);

            // Act
            var result = await _controller.RefreshData(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<PartialViewResult>());

            var partialViewResult = (PartialViewResult)result;
            Assert.That(partialViewResult.ViewName, Is.EqualTo("_DashboardContent"));
            Assert.That(partialViewResult.Model, Is.EqualTo(dashboardData));
        }

        #endregion

        #region GetEfficiencyData Action Tests



        [Test]
        public async Task GetEfficiencyData_WithValidRequest_ShouldReturnJsonWithEfficiencyMetrics()
        {
            // Arrange
            SetupAuthenticatedAdminUser();

            var fromDate = DateTime.Today.AddDays(-30);
            var toDate = DateTime.Today;
            var efficiencyData = new EfficiencyMetricsModel();

            _mockAnalyticsService.Setup(x => x.GetEfficiencyMetricsAsync(fromDate, toDate, null, null))
                .ReturnsAsync(efficiencyData);

            // Act
            var result = await _controller.GetEfficiencyData(fromDate, toDate);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<JsonResult>());

            var jsonResult = (JsonResult)result;
            Assert.That(jsonResult.Value, Is.EqualTo(efficiencyData));
        }

        #endregion

        #region GetOperatorData Action Tests

        [Test]
        public async Task GetOperatorData_WithValidRequest_ShouldReturnJsonWithOperatorData()
        {
            // Arrange
            SetupAuthenticatedAdminUser();

            var fromDate = DateTime.Today.AddDays(-30);
            var toDate = DateTime.Today;
            int[]? projectIds = null;
            var sortBy = "efficiency";

            var expectedOperatorPerformance = new List<OperatorPerformanceModel>
            {
                new OperatorPerformanceModel
                {
                    OperatorId = 1,
                    OperatorName = "John Doe",
                    TasksCompleted = 15,
                    OnTimeCompletionRate = 90.0m,
                    EfficiencyRating = 8.5m
                }
            };

            _mockAnalyticsService.Setup(x => x.GetOperatorPerformanceAsync(fromDate, toDate, projectIds, sortBy))
                .ReturnsAsync(expectedOperatorPerformance);

            // Act
            var result = await _controller.GetOperatorData(fromDate, toDate, projectIds, sortBy);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<JsonResult>());

            var jsonResult = (JsonResult)result;
            Assert.That(jsonResult.Value, Is.EqualTo(expectedOperatorPerformance));
        }

        [Test]
        public async Task GetOperatorData_WithUnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var filters = new ReportFilterModel();

            // Act
            var result = await _controller.GetOperatorData(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        #endregion

        #region GetTrendData Action Tests



        [Test]
        public async Task GetTrendData_WithValidRequest_ShouldReturnJsonWithTrendChartData()
        {
            // Arrange
            SetupAuthenticatedAdminUser();

            var fromDate = DateTime.Today.AddDays(-30);
            var toDate = DateTime.Today;
            int[]? operatorIds = null;
            int[]? projectIds = null;
            var granularity = "weekly";
            var trendData = new TrendChartModel();

            _mockAnalyticsService.Setup(x => x.GetCompletionTrendsAsync(fromDate, toDate, operatorIds, projectIds, granularity))
                .ReturnsAsync(trendData);

            // Act
            var result = await _controller.GetTrendData(fromDate, toDate, operatorIds, projectIds, granularity);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<JsonResult>());

            var jsonResult = (JsonResult)result;
            Assert.That(jsonResult.Value, Is.EqualTo(trendData));
        }

        #endregion
    }
}
