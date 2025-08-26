using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Controllers;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerUnitTests
    {
        private HomeController _controller = null!;
        private Mock<ILogger<HomeController>> _mockLogger = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<ClaimsPrincipal> _mockUser = null!;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
            
            _mockHttpContext = new Mock<HttpContext>();
            _mockUser = new Mock<ClaimsPrincipal>();

            var controllerContext = new ControllerContext()
            {
                HttpContext = _mockHttpContext.Object
            };

            _controller.ControllerContext = controllerContext;
            _mockHttpContext.Setup(x => x.User).Returns(_mockUser.Object);
        }

        [Test]
        public void HomeController_ShouldInheritFromBaseController()
        {
            // Assert
            Assert.That(_controller, Is.InstanceOf<BaseController>());
        }

        [Test]
        public void HomeController_Constructor_WithValidLogger_ShouldCreateInstance()
        {
            // Act & Assert
            Assert.That(_controller, Is.Not.Null);
        }

        // Note: HomeController constructor does not validate null parameters

        [Test]
        public void Index_WithUnauthenticatedUser_ShouldReturnView()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void Index_WithAuthenticatedUser_ShouldRedirectToTaskAll()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            
            var redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo("All"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Task"));
        }

        [Test]
        public void Index_WithNullIdentity_ShouldReturnView()
        {
            // Arrange
            _mockUser.Setup(x => x.Identity).Returns((ClaimsIdentity?)null);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void Index_ShouldHaveAllowAnonymousAttribute()
        {
            // Arrange
            var methodInfo = typeof(HomeController).GetMethod("Index");

            // Act
            var allowAnonymousAttribute = methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);

            // Assert
            Assert.That(allowAnonymousAttribute, Is.Not.Null);
            Assert.That(allowAnonymousAttribute, Is.Not.Empty);
            Assert.That(allowAnonymousAttribute?.Length, Is.EqualTo(1));
        }

        [Test]
        public void Error_ShouldReturnViewResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void Error_ShouldHaveResponseCacheAttribute()
        {
            // Arrange
            var methodInfo = typeof(HomeController).GetMethods()
                .FirstOrDefault(m => m.Name == "Error" && m.GetParameters().Length == 0);

            // Act
            var responseCacheAttribute = methodInfo?.GetCustomAttributes(typeof(ResponseCacheAttribute), false);

            // Assert
            Assert.That(methodInfo, Is.Not.Null);
            Assert.That(responseCacheAttribute, Is.Not.Null);
            Assert.That(responseCacheAttribute, Is.Not.Empty);

            var attribute = (ResponseCacheAttribute)responseCacheAttribute![0];
            Assert.That(attribute.Duration, Is.EqualTo(0));
            Assert.That(attribute.Location, Is.EqualTo(ResponseCacheLocation.None));
            Assert.That(attribute.NoStore, Is.True);
        }

        [Test]
        public void Health_ShouldReturnOkResultWithHealthStatus()
        {
            // Act
            var result = _controller.Health();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            
            // Check if the result contains expected properties
            var resultValue = okResult.Value;
            var resultType = resultValue?.GetType();
            
            var statusProperty = resultType?.GetProperty("status");
            var timestampProperty = resultType?.GetProperty("timestamp");
            
            Assert.That(statusProperty, Is.Not.Null);
            Assert.That(timestampProperty, Is.Not.Null);
            
            var statusValue = statusProperty?.GetValue(resultValue);
            var timestampValue = timestampProperty?.GetValue(resultValue);
            
            Assert.That(statusValue, Is.EqualTo("healthy"));
            Assert.That(timestampValue, Is.InstanceOf<DateTime>());
        }

        [Test]
        public void Health_ShouldHaveAllowAnonymousAttribute()
        {
            // Arrange
            var methodInfo = typeof(HomeController).GetMethod("Health");

            // Act
            var allowAnonymousAttribute = methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);

            // Assert
            Assert.That(allowAnonymousAttribute, Is.Not.Null);
            Assert.That(allowAnonymousAttribute, Is.Not.Empty);
            Assert.That(allowAnonymousAttribute?.Length, Is.EqualTo(1));
        }

        [Test]
        public void Health_ShouldHaveRouteAttribute()
        {
            // Arrange
            var methodInfo = typeof(HomeController).GetMethod("Health");

            // Act
            var routeAttribute = methodInfo?.GetCustomAttributes(typeof(RouteAttribute), false);

            // Assert
            Assert.That(routeAttribute, Is.Not.Null);
            Assert.That(routeAttribute, Is.Not.Empty);
            
            var attribute = (RouteAttribute)routeAttribute![0];
            Assert.That(attribute.Template, Is.EqualTo("health"));
        }

        [Test]
        public void Health_TimestampShouldBeRecent()
        {
            // Arrange
            var beforeCall = DateTime.UtcNow;

            // Act
            var result = _controller.Health();
            var afterCall = DateTime.UtcNow;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            
            var okResult = (OkObjectResult)result;
            var resultValue = okResult.Value;
            var resultType = resultValue?.GetType();
            var timestampProperty = resultType?.GetProperty("timestamp");
            var timestampValue = (DateTime)timestampProperty?.GetValue(resultValue)!;
            
            Assert.That(timestampValue, Is.GreaterThanOrEqualTo(beforeCall));
            Assert.That(timestampValue, Is.LessThanOrEqualTo(afterCall));
        }

        [Test]
        public void Health_ShouldReturnConsistentStatusMessage()
        {
            // Act
            var result1 = _controller.Health();
            var result2 = _controller.Health();

            // Assert
            Assert.That(result1, Is.InstanceOf<OkObjectResult>());
            Assert.That(result2, Is.InstanceOf<OkObjectResult>());
            
            var okResult1 = (OkObjectResult)result1;
            var okResult2 = (OkObjectResult)result2;
            
            var resultValue1 = okResult1.Value;
            var resultValue2 = okResult2.Value;
            
            var resultType = resultValue1?.GetType();
            var statusProperty = resultType?.GetProperty("status");
            
            var statusValue1 = statusProperty?.GetValue(resultValue1);
            var statusValue2 = statusProperty?.GetValue(resultValue2);
            
            Assert.That(statusValue1, Is.EqualTo(statusValue2));
            Assert.That(statusValue1, Is.EqualTo("healthy"));
        }

        [Test]
        public void Index_MultipleCallsWithSameAuthenticationState_ShouldReturnConsistentResults()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            // Act
            var result1 = _controller.Index();
            var result2 = _controller.Index();

            // Assert
            Assert.That(result1, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(result2, Is.InstanceOf<RedirectToActionResult>());
            
            var redirect1 = (RedirectToActionResult)result1;
            var redirect2 = (RedirectToActionResult)result2;
            
            Assert.That(redirect1.ActionName, Is.EqualTo(redirect2.ActionName));
            Assert.That(redirect1.ControllerName, Is.EqualTo(redirect2.ControllerName));
        }

        [Test]
        public void HomeController_AllPublicMethods_ShouldBeAccessible()
        {
            // Arrange
            var publicMethods = typeof(HomeController).GetMethods()
                .Where(m => m.IsPublic && m.DeclaringType == typeof(HomeController))
                .Select(m => m.Name)
                .ToList();

            // Assert
            Assert.That(publicMethods, Does.Contain("Index"));
            Assert.That(publicMethods, Does.Contain("Error"));
            Assert.That(publicMethods, Does.Contain("Health"));
        }

        [Test]
        public void HomeController_ShouldHaveCorrectNamespace()
        {
            // Act
            var controllerNamespace = typeof(HomeController).Namespace;

            // Assert
            Assert.That(controllerNamespace, Is.EqualTo("TeamWorkFlow.Controllers"));
        }
    }
}
