using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Controllers;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class BaseControllerUnitTests
    {
        private BaseController _controller = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<ClaimsPrincipal> _mockUser = null!;

        [SetUp]
        public void SetUp()
        {
            _controller = new BaseController();
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
        public void BaseController_ShouldInheritFromController()
        {
            // Assert
            Assert.That(_controller, Is.InstanceOf<Controller>());
        }

        [Test]
        public void BaseController_ShouldHaveAuthorizeAttribute()
        {
            // Arrange
            var controllerType = typeof(BaseController);

            // Act
            var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false);

            // Assert
            Assert.That(authorizeAttribute, Is.Not.Empty);
            Assert.That(authorizeAttribute.Length, Is.EqualTo(1));
            Assert.That(authorizeAttribute[0], Is.InstanceOf<AuthorizeAttribute>());
        }

        [Test]
        public void BaseController_CanBeInstantiated()
        {
            // Act & Assert
            Assert.That(_controller, Is.Not.Null);
            Assert.That(_controller, Is.InstanceOf<BaseController>());
        }

        [Test]
        public void BaseController_HasControllerContext()
        {
            // Assert
            Assert.That(_controller.ControllerContext, Is.Not.Null);
            Assert.That(_controller.ControllerContext.HttpContext, Is.EqualTo(_mockHttpContext.Object));
        }

        [Test]
        public void BaseController_CanAccessUser()
        {
            // Act
            var user = _controller.User;

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user, Is.EqualTo(_mockUser.Object));
        }

        [Test]
        public void BaseController_CanAccessHttpContext()
        {
            // Act
            var httpContext = _controller.HttpContext;

            // Assert
            Assert.That(httpContext, Is.Not.Null);
            Assert.That(httpContext, Is.EqualTo(_mockHttpContext.Object));
        }

        [Test]
        public void BaseController_CanAccessModelState()
        {
            // Act
            var modelState = _controller.ModelState;

            // Assert
            Assert.That(modelState, Is.Not.Null);
        }

        [Test]
        public void BaseController_CanAccessTempData()
        {
            // Arrange
            var mockTempData = new Mock<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary>();
            _controller.TempData = mockTempData.Object;

            // Act
            var tempData = _controller.TempData;

            // Assert
            Assert.That(tempData, Is.Not.Null);
            Assert.That(tempData, Is.EqualTo(mockTempData.Object));
        }

        [Test]
        public void BaseController_CanCreateViewResult()
        {
            // Act
            var result = _controller.View();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void BaseController_CanCreateViewResultWithModel()
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
        public void BaseController_CanCreateRedirectToActionResult()
        {
            // Act
            var result = _controller.RedirectToAction("TestAction");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(result.ActionName, Is.EqualTo("TestAction"));
        }

        [Test]
        public void BaseController_CanCreateRedirectToActionResultWithController()
        {
            // Act
            var result = _controller.RedirectToAction("TestAction", "TestController");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(result.ActionName, Is.EqualTo("TestAction"));
            Assert.That(result.ControllerName, Is.EqualTo("TestController"));
        }

        [Test]
        public void BaseController_CanCreateBadRequestResult()
        {
            // Act
            var result = _controller.BadRequest();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public void BaseController_CanCreateUnauthorizedResult()
        {
            // Act
            var result = _controller.Unauthorized();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public void BaseController_CanCreateNotFoundResult()
        {
            // Act
            var result = _controller.NotFound();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void BaseController_CanCreateChallengeResult()
        {
            // Act
            var result = _controller.Challenge();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public void BaseController_CanCreateOkResult()
        {
            // Act
            var result = _controller.Ok();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public void BaseController_CanCreateOkResultWithValue()
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
        public void BaseController_AuthorizeAttribute_RequiresAuthentication()
        {
            // Arrange
            var controllerType = typeof(BaseController);
            var authorizeAttribute = (AuthorizeAttribute)controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false)[0];

            // Assert
            Assert.That(authorizeAttribute.Policy, Is.Null);
            Assert.That(authorizeAttribute.Roles, Is.Null);
            Assert.That(authorizeAttribute.AuthenticationSchemes, Is.Null);
        }

        [Test]
        public void BaseController_CanHandleActionExecution()
        {
            // Arrange
            var actionContext = new ActionContext(
                _mockHttpContext.Object,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor()
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                _controller
            );

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() =>
            {
                // This tests that the controller can be used in action execution context
                var context = actionExecutingContext;
                Assert.That(context.Controller, Is.EqualTo(_controller));
            });
        }

        [Test]
        public void BaseController_CanAccessRouteData()
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
        public void BaseController_CanAccessRequest()
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
        public void BaseController_CanAccessResponse()
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
    }
}
