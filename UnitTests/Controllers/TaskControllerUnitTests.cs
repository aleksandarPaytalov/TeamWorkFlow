using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Controllers;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Extensions;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class TaskControllerUnitTests
    {
        private TaskController _controller = null!;
        private Mock<ITaskService> _mockTaskService = null!;
        private Mock<IProjectService> _mockProjectService = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<ClaimsPrincipal> _mockUser = null!;
        private Mock<ITempDataDictionary> _mockTempData = null!;

        [SetUp]
        public void SetUp()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockProjectService = new Mock<IProjectService>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockUser = new Mock<ClaimsPrincipal>();
            _mockTempData = new Mock<ITempDataDictionary>();

            _controller = new TaskController(
                _mockTaskService.Object,
                _mockProjectService.Object
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
        public void TaskController_ShouldInheritFromBaseController()
        {
            // Assert
            Assert.That(_controller, Is.InstanceOf<BaseController>());
        }

        [Test]
        public void TaskController_Constructor_WithValidServices_ShouldCreateInstance()
        {
            // Act & Assert
            Assert.That(_controller, Is.Not.Null);
        }

        // Note: TaskController constructor does not validate null parameters

        [Test]
        public async Task All_WithUnauthenticatedUser_ShouldReturnChallenge()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var model = new AllTasksQueryModel();

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public async Task All_WithAuthenticatedAdminUser_ShouldReturnViewWithTasks()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(false);

            var model = new AllTasksQueryModel();
            var taskQueryResult = new TaskQueryServiceModel
            {
                TotalTasksCount = 5,
                Tasks = new List<TaskServiceModel>
                {
                    new TaskServiceModel { Id = 1, Name = "Test Task 1" },
                    new TaskServiceModel { Id = 2, Name = "Test Task 2" }
                }
            };

            _mockTaskService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.TaskSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(taskQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllTasksQueryModel>());
            
            var resultModel = (AllTasksQueryModel)viewResult.Model!;
            Assert.That(resultModel.TotalTasksCount, Is.EqualTo(5));
            Assert.That(resultModel.Tasks.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task All_WithAuthenticatedOperatorUser_ShouldReturnViewWithTasks()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(true);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(false);

            var model = new AllTasksQueryModel();
            var taskQueryResult = new TaskQueryServiceModel
            {
                TotalTasksCount = 3,
                Tasks = new List<TaskServiceModel>
                {
                    new TaskServiceModel { Id = 1, Name = "Test Task 1" }
                }
            };

            _mockTaskService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.TaskSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(taskQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllTasksQueryModel>());
            
            var resultModel = (AllTasksQueryModel)viewResult.Model!;
            Assert.That(resultModel.TotalTasksCount, Is.EqualTo(3));
        }

        [Test]
        public async Task All_WithAuthenticatedGuestUser_ShouldReturnViewWithTasks()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(true);

            var model = new AllTasksQueryModel();
            var taskQueryResult = new TaskQueryServiceModel
            {
                TotalTasksCount = 1,
                Tasks = new List<TaskServiceModel>
                {
                    new TaskServiceModel { Id = 1, Name = "Test Task 1" }
                }
            };

            _mockTaskService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.TaskSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(taskQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllTasksQueryModel>());
        }

        [Test]
        public async Task All_WithNoRoles_ShouldReturnChallenge()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(false);

            var model = new AllTasksQueryModel();

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public async Task Add_WithNonAdminNonOperatorUser_ShouldReturnUnauthorized()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);

            // Act
            var result = await _controller.Add();

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Add_WithAdminUser_ShouldReturnViewWithTaskFormModel()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);

            var priorities = new List<TaskPriorityServiceModel>
            {
                new TaskPriorityServiceModel { Id = 1, Name = "High" },
                new TaskPriorityServiceModel { Id = 2, Name = "Medium" }
            };

            var statuses = new List<TaskStatusServiceModel>
            {
                new TaskStatusServiceModel { Id = 1, Name = "Open" },
                new TaskStatusServiceModel { Id = 2, Name = "In Progress" }
            };

            _mockTaskService.Setup(x => x.GetAllPrioritiesAsync()).ReturnsAsync(priorities);
            _mockTaskService.Setup(x => x.GetAllStatusesAsync()).ReturnsAsync(statuses);

            // Act
            var result = await _controller.Add();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<TaskFormModel>());
            
            var model = (TaskFormModel)viewResult.Model!;
            Assert.That(model.Priorities.Count(), Is.EqualTo(2));
            Assert.That(model.Statuses.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Add_WithOperatorUser_ShouldReturnViewWithTaskFormModel()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(true);

            var priorities = new List<TaskPriorityServiceModel>
            {
                new TaskPriorityServiceModel { Id = 1, Name = "High" }
            };

            var statuses = new List<TaskStatusServiceModel>
            {
                new TaskStatusServiceModel { Id = 1, Name = "Open" }
            };

            _mockTaskService.Setup(x => x.GetAllPrioritiesAsync()).ReturnsAsync(priorities);
            _mockTaskService.Setup(x => x.GetAllStatusesAsync()).ReturnsAsync(statuses);

            // Act
            var result = await _controller.Add();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<TaskFormModel>());
        }

        [Test]
        public async Task All_CallsTaskServiceWithCorrectParameters()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            var model = new AllTasksQueryModel
            {
                Sorting = TeamWorkFlow.Core.Enumerations.TaskSorting.NameAscending,
                Search = "test",
                CurrentPage = 2
            };

            var taskQueryResult = new TaskQueryServiceModel
            {
                TotalTasksCount = 0,
                Tasks = new List<TaskServiceModel>()
            };

            _mockTaskService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.TaskSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(taskQueryResult);

            // Act
            await _controller.All(model);

            // Assert
            _mockTaskService.Verify(x => x.AllAsync(
                model.Sorting,
                model.Search,
                model.TasksPerPage,
                model.CurrentPage
            ), Times.Once);
        }

        [Test]
        public async Task Add_CallsTaskServiceForPrioritiesAndStatuses()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            var priorities = new List<TaskPriorityServiceModel>();
            var statuses = new List<TaskStatusServiceModel>();

            _mockTaskService.Setup(x => x.GetAllPrioritiesAsync()).ReturnsAsync(priorities);
            _mockTaskService.Setup(x => x.GetAllStatusesAsync()).ReturnsAsync(statuses);

            // Act
            await _controller.Add();

            // Assert
            _mockTaskService.Verify(x => x.GetAllPrioritiesAsync(), Times.Once);
            _mockTaskService.Verify(x => x.GetAllStatusesAsync(), Times.Once);
        }

        [Test]
        public void TaskController_ShouldHaveCorrectNamespace()
        {
            // Act
            var controllerNamespace = typeof(TaskController).Namespace;

            // Assert
            Assert.That(controllerNamespace, Is.EqualTo("TeamWorkFlow.Controllers"));
        }

        [Test]
        public void TaskController_AllPublicMethods_ShouldBeAccessible()
        {
            // Arrange
            var publicMethods = typeof(TaskController).GetMethods()
                .Where(m => m.IsPublic && m.DeclaringType == typeof(TaskController))
                .Select(m => m.Name)
                .ToList();

            // Assert
            Assert.That(publicMethods, Does.Contain("All"));
            Assert.That(publicMethods, Does.Contain("Add"));
            Assert.That(publicMethods, Does.Contain("SetEstimatedTime"));
        }

        #region SetEstimatedTime Tests

        [Test]
        public async Task SetEstimatedTime_WithUnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);

            int taskId = 1;
            int estimatedTime = 10;

            // Act
            var result = await _controller.SetEstimatedTime(taskId, estimatedTime);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            Assert.That(resultData, Is.Not.Null);

            // Use reflection to access anonymous object properties
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.False);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Unauthorized"));
        }

        [Test]
        public async Task SetEstimatedTime_WithAdminUser_CallsTaskService()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);

            int taskId = 1;
            int estimatedTime = 10;

            _mockTaskService.Setup(x => x.SetEstimatedTimeAsync(taskId, estimatedTime))
                .ReturnsAsync((true, "Estimated time updated successfully"));

            // Act
            var result = await _controller.SetEstimatedTime(taskId, estimatedTime);

            // Assert
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(taskId, estimatedTime), Times.Once);

            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.True);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Estimated time updated successfully"));
        }

        [Test]
        public async Task SetEstimatedTime_WithOperatorUser_CallsTaskService()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(true);

            int taskId = 1;
            int estimatedTime = 15;

            _mockTaskService.Setup(x => x.SetEstimatedTimeAsync(taskId, estimatedTime))
                .ReturnsAsync((true, "Estimated time updated successfully"));

            // Act
            var result = await _controller.SetEstimatedTime(taskId, estimatedTime);

            // Assert
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(taskId, estimatedTime), Times.Once);

            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.True);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Estimated time updated successfully"));
        }

        [Test]
        public async Task SetEstimatedTime_WithInvalidTimeBelowRange_ReturnsError()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            int taskId = 1;
            int invalidEstimatedTime = 0;

            // Act
            var result = await _controller.SetEstimatedTime(taskId, invalidEstimatedTime);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.False);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Estimated time must be between 1 and 1000 hours"));

            // Verify service was not called
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task SetEstimatedTime_WithInvalidTimeAboveRange_ReturnsError()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            int taskId = 1;
            int invalidEstimatedTime = 1001;

            // Act
            var result = await _controller.SetEstimatedTime(taskId, invalidEstimatedTime);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.False);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Estimated time must be between 1 and 1000 hours"));

            // Verify service was not called
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task SetEstimatedTime_WithValidTimeAtMinimumBoundary_ReturnsSuccess()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            int taskId = 1;
            int minimumValidTime = 1;

            _mockTaskService.Setup(x => x.SetEstimatedTimeAsync(taskId, minimumValidTime))
                .ReturnsAsync((true, "Estimated time updated successfully"));

            // Act
            var result = await _controller.SetEstimatedTime(taskId, minimumValidTime);

            // Assert
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(taskId, minimumValidTime), Times.Once);

            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");

            Assert.That(successProperty!.GetValue(resultData), Is.True);
        }

        [Test]
        public async Task SetEstimatedTime_WithValidTimeAtMaximumBoundary_ReturnsSuccess()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            int taskId = 1;
            int maximumValidTime = 1000;

            _mockTaskService.Setup(x => x.SetEstimatedTimeAsync(taskId, maximumValidTime))
                .ReturnsAsync((true, "Estimated time updated successfully"));

            // Act
            var result = await _controller.SetEstimatedTime(taskId, maximumValidTime);

            // Assert
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(taskId, maximumValidTime), Times.Once);

            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");

            Assert.That(successProperty!.GetValue(resultData), Is.True);
        }

        [Test]
        public async Task SetEstimatedTime_WhenServiceReturnsFailure_ReturnsServiceError()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            int taskId = 999;
            int estimatedTime = 10;
            string serviceErrorMessage = "Task not found";

            _mockTaskService.Setup(x => x.SetEstimatedTimeAsync(taskId, estimatedTime))
                .ReturnsAsync((false, serviceErrorMessage));

            // Act
            var result = await _controller.SetEstimatedTime(taskId, estimatedTime);

            // Assert
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(taskId, estimatedTime), Times.Once);

            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.False);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo(serviceErrorMessage));
        }

        [Test]
        public async Task SetEstimatedTime_WithNegativeTime_ReturnsError()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            int taskId = 1;
            int negativeTime = -5;

            // Act
            var result = await _controller.SetEstimatedTime(taskId, negativeTime);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.False);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Estimated time must be between 1 and 1000 hours"));

            // Verify service was not called
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task SetEstimatedTime_WithValidMidRangeTime_ReturnsSuccess()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            int taskId = 5;
            int midRangeTime = 500;

            _mockTaskService.Setup(x => x.SetEstimatedTimeAsync(taskId, midRangeTime))
                .ReturnsAsync((true, "Estimated time updated successfully"));

            // Act
            var result = await _controller.SetEstimatedTime(taskId, midRangeTime);

            // Assert
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(taskId, midRangeTime), Times.Once);

            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.True);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Estimated time updated successfully"));
        }

        [Test]
        public async Task SetEstimatedTime_WithGuestUser_ReturnsUnauthorized()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(true);

            int taskId = 1;
            int estimatedTime = 10;

            // Act
            var result = await _controller.SetEstimatedTime(taskId, estimatedTime);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);

            var resultData = jsonResult!.Value as dynamic;
            var successProperty = resultData!.GetType().GetProperty("success");
            var messageProperty = resultData.GetType().GetProperty("message");

            Assert.That(successProperty!.GetValue(resultData), Is.False);
            Assert.That(messageProperty!.GetValue(resultData), Is.EqualTo("Unauthorized"));

            // Verify service was not called
            _mockTaskService.Verify(x => x.SetEstimatedTimeAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        #endregion
    }
}
