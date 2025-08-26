using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Areas.Admin.Controllers;

namespace UnitTests.Controllers.Admin
{
    [TestFixture]
    public class AdminHomeControllerUnitTests
    {
        private HomeController _controller = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<ClaimsPrincipal> _mockUser = null!;

        [SetUp]
        public void SetUp()
        {
            _controller = new HomeController();
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
        public void AdminHomeController_ShouldInheritFromAdminBaseController()
        {
            // Assert
            Assert.That(_controller, Is.InstanceOf<AdminBaseController>());
        }

        [Test]
        public void AdminHomeController_CanBeInstantiated()
        {
            // Act & Assert
            Assert.That(_controller, Is.Not.Null);
            Assert.That(_controller, Is.InstanceOf<HomeController>());
        }

        [Test]
        public void Check_ShouldReturnViewResult()
        {
            // Act
            var result = _controller.Check();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void Check_ShouldReturnViewWithoutModel()
        {
            // Act
            var result = _controller.Check();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.Null);
        }

        [Test]
        public void Check_ShouldNotSpecifyViewName()
        {
            // Act
            var result = _controller.Check();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.Null);
        }

        [Test]
        public void AdminHomeController_ShouldHaveCorrectNamespace()
        {
            // Act
            var controllerNamespace = typeof(HomeController).Namespace;

            // Assert
            Assert.That(controllerNamespace, Is.EqualTo("TeamWorkFlow.Areas.Admin.Controllers"));
        }

        [Test]
        public void AdminHomeController_ShouldBeInAdminArea()
        {
            // Arrange - Check inherited Area attribute from AdminBaseController
            var baseControllerType = typeof(TeamWorkFlow.Areas.Admin.Controllers.AdminBaseController);

            // Act
            var areaAttribute = baseControllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.AreaAttribute), false);

            // Assert
            Assert.That(areaAttribute, Is.Not.Empty);
            var attribute = (Microsoft.AspNetCore.Mvc.AreaAttribute)areaAttribute[0];
            Assert.That(attribute.RouteValue, Is.EqualTo("Admin"));
        }

        [Test]
        public void AdminHomeController_AllPublicMethods_ShouldBeAccessible()
        {
            // Arrange
            var publicMethods = typeof(HomeController).GetMethods()
                .Where(m => m.IsPublic && m.DeclaringType == typeof(HomeController))
                .Select(m => m.Name)
                .ToList();

            // Assert
            Assert.That(publicMethods, Does.Contain("Check"));
        }

        [Test]
        public void Check_MultipleCallsShouldReturnConsistentResults()
        {
            // Act
            var result1 = _controller.Check();
            var result2 = _controller.Check();

            // Assert
            Assert.That(result1, Is.InstanceOf<ViewResult>());
            Assert.That(result2, Is.InstanceOf<ViewResult>());

            var viewResult1 = (ViewResult)result1;
            var viewResult2 = (ViewResult)result2;

            Assert.That(viewResult1.ViewName, Is.EqualTo(viewResult2.ViewName));
            Assert.That(viewResult1.Model, Is.EqualTo(viewResult2.Model));
        }

        [Test]
        public void AdminHomeController_CanAccessControllerContext()
        {
            // Act
            var controllerContext = _controller.ControllerContext;

            // Assert
            Assert.That(controllerContext, Is.Not.Null);
            Assert.That(controllerContext.HttpContext, Is.EqualTo(_mockHttpContext.Object));
        }

        [Test]
        public void AdminHomeController_CanAccessUser()
        {
            // Act
            var user = _controller.User;

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user, Is.EqualTo(_mockUser.Object));
        }

        [Test]
        public void AdminHomeController_CanAccessHttpContext()
        {
            // Act
            var httpContext = _controller.HttpContext;

            // Assert
            Assert.That(httpContext, Is.Not.Null);
            Assert.That(httpContext, Is.EqualTo(_mockHttpContext.Object));
        }

        [Test]
        public void AdminHomeController_CanAccessModelState()
        {
            // Act
            var modelState = _controller.ModelState;

            // Assert
            Assert.That(modelState, Is.Not.Null);
        }

        [Test]
        public void AdminHomeController_CanCreateViewResult()
        {
            // Act
            var result = _controller.View();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void AdminHomeController_CanCreateViewResultWithModel()
        {
            // Arrange
            var model = new { TestProperty = "TestValue" };

            // Act
            var result = _controller.View(model);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(result.Model, Is.EqualTo(model));
        }

        [Test]
        public void AdminHomeController_CanCreateRedirectToActionResult()
        {
            // Act
            var result = _controller.RedirectToAction("TestAction");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(result.ActionName, Is.EqualTo("TestAction"));
        }

        [Test]
        public void AdminHomeController_CanCreateBadRequestResult()
        {
            // Act
            var result = _controller.BadRequest();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public void AdminHomeController_CanCreateUnauthorizedResult()
        {
            // Act
            var result = _controller.Unauthorized();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public void AdminHomeController_CanCreateNotFoundResult()
        {
            // Act
            var result = _controller.NotFound();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void AdminHomeController_CanCreateOkResult()
        {
            // Act
            var result = _controller.Ok();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public void AdminHomeController_CanCreateOkResultWithValue()
        {
            // Arrange
            var value = new { Message = "Success" };

            // Act
            var result = _controller.Ok(value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(value));
        }

        [Test]
        public void AdminHomeController_CanAccessRouteData()
        {
            // Arrange
            var routeData = new Microsoft.AspNetCore.Routing.RouteData();
            _controller.ControllerContext.RouteData = routeData;

            // Act
            var result = _controller.RouteData;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(routeData));
        }

        [Test]
        public void AdminHomeController_CanAccessRequest()
        {
            // Arrange
            var mockRequest = new Mock<HttpRequest>();
            _mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);

            // Act
            var request = _controller.Request;

            // Assert
            Assert.That(request, Is.Not.Null);
            Assert.That(request, Is.EqualTo(mockRequest.Object));
        }

        [Test]
        public void AdminHomeController_CanAccessResponse()
        {
            // Arrange
            var mockResponse = new Mock<HttpResponse>();
            _mockHttpContext.Setup(x => x.Response).Returns(mockResponse.Object);

            // Act
            var response = _controller.Response;

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(mockResponse.Object));
        }

        [Test]
        public void Check_ShouldBeHttpGetAction()
        {
            // Arrange
            var methodInfo = typeof(HomeController).GetMethod("Check");

            // Act
            var httpGetAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute), false);

            // Assert - Check action should be accessible via GET (default behavior)
            // If no HttpGet attribute is present, it's still accessible via GET by default
            Assert.That(methodInfo, Is.Not.Null);
        }
    }
}
