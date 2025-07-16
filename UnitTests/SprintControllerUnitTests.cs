using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Text.Json;
using TeamWorkFlow.Controllers;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Sprint;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace UnitTests
{
    [TestFixture]
    public class SprintControllerUnitTests
    {
        private Mock<ISprintService> _mockSprintService;
        private Mock<ITempDataDictionary> _mockTempData;
        private SprintController _controller;

        [SetUp]
        public void Setup()
        {
            _mockSprintService = new Mock<ISprintService>();
            _mockTempData = new Mock<ITempDataDictionary>();

            _controller = new SprintController(_mockSprintService.Object);
            
            // Setup TempData
            _controller.TempData = _mockTempData.Object;

            // Setup User Claims for Admin role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin@test.com"),
                new Claim(ClaimTypes.Role, AdminRole)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };
        }

        private static (bool success, string message) ParseJsonResult(JsonResult jsonResult)
        {
            var json = JsonSerializer.Serialize(jsonResult.Value);
            var doc = JsonDocument.Parse(json);
            var success = doc.RootElement.GetProperty("success").GetBoolean();
            var message = doc.RootElement.GetProperty("message").GetString() ?? "";
            return (success, message);
        }

        private static (bool success, string message, int count) ParseJsonResultWithCount(JsonResult jsonResult)
        {
            var json = JsonSerializer.Serialize(jsonResult.Value);
            var doc = JsonDocument.Parse(json);
            var success = doc.RootElement.GetProperty("success").GetBoolean();
            var message = doc.RootElement.GetProperty("message").GetString() ?? "";
            var count = doc.RootElement.GetProperty("count").GetInt32();
            return (success, message, count);
        }

        #region Index Tests

        [Test]
        public async Task Index_WithValidParameters_ReturnsViewWithModel()
        {
            // Arrange
            var expectedViewModel = new SprintPlanningViewModel
            {
                SprintTasks = new List<SprintTaskServiceModel>(),
                BacklogTasks = new List<SprintTaskServiceModel>(),
                SearchTerm = "test",
                StatusFilter = "In Progress",
                CurrentPage = 1,
                PageSize = 20
            };

            _mockSprintService.Setup(s => s.GetSprintPlanningDataAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedViewModel);

            // Act
            var result = await _controller.Index("test", "In Progress", "", "", "", "", 1, 20);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult!.Model, Is.EqualTo(expectedViewModel));
            
            _mockSprintService.Verify(s => s.GetSprintPlanningDataAsync(
                "test", "In Progress", "", "", "", "", 1, 20), Times.Once);
        }

        [Test]
        public async Task Index_WithDefaultParameters_ReturnsViewWithModel()
        {
            // Arrange
            var expectedViewModel = new SprintPlanningViewModel();
            _mockSprintService.Setup(s => s.GetSprintPlanningDataAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedViewModel);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult!.Model, Is.InstanceOf<SprintPlanningViewModel>());
            
            _mockSprintService.Verify(s => s.GetSprintPlanningDataAsync(
                "", "", "", "", "", "", 1, 20), Times.Once);
        }

        [Test]
        public async Task Index_WhenServiceThrowsException_ReturnsViewWithEmptyModelAndErrorMessage()
        {
            // Arrange
            _mockSprintService.Setup(s => s.GetSprintPlanningDataAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult!.Model, Is.InstanceOf<SprintPlanningViewModel>());
            
            _mockTempData.VerifySet(td => td[UserMessageError] = "Error loading sprint data. Please try again.", Times.Once);
        }

        #endregion

        #region AddTaskToSprint Tests

        [Test]
        public async Task AddTaskToSprint_WithValidTask_ReturnsSuccessJson()
        {
            // Arrange
            int taskId = 1;
            int sprintOrder = 1;
            
            _mockSprintService.Setup(s => s.ValidateTaskForSprintAsync(taskId))
                .ReturnsAsync((true, "Task can be added"));
            _mockSprintService.Setup(s => s.AddTaskToSprintAsync(taskId, sprintOrder))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddTaskToSprint(taskId, sprintOrder);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Task added to sprint successfully"));

            _mockSprintService.Verify(s => s.ValidateTaskForSprintAsync(taskId), Times.Once);
            _mockSprintService.Verify(s => s.AddTaskToSprintAsync(taskId, sprintOrder), Times.Once);
        }

        [Test]
        public async Task AddTaskToSprint_WithInvalidTask_ReturnsFailureJson()
        {
            // Arrange
            int taskId = 1;
            int sprintOrder = 1;
            string validationReason = "Task cannot be added due to capacity constraints";
            
            _mockSprintService.Setup(s => s.ValidateTaskForSprintAsync(taskId))
                .ReturnsAsync((false, validationReason));

            // Act
            var result = await _controller.AddTaskToSprint(taskId, sprintOrder);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            
            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo(validationReason));
            
            _mockSprintService.Verify(s => s.ValidateTaskForSprintAsync(taskId), Times.Once);
            _mockSprintService.Verify(s => s.AddTaskToSprintAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task AddTaskToSprint_WhenAddFails_ReturnsFailureJson()
        {
            // Arrange
            int taskId = 1;
            int sprintOrder = 1;
            
            _mockSprintService.Setup(s => s.ValidateTaskForSprintAsync(taskId))
                .ReturnsAsync((true, "Task can be added"));
            _mockSprintService.Setup(s => s.AddTaskToSprintAsync(taskId, sprintOrder))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddTaskToSprint(taskId, sprintOrder);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            
            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Failed to add task to sprint"));
        }

        [Test]
        public async Task AddTaskToSprint_WhenExceptionThrown_ReturnsErrorJson()
        {
            // Arrange
            int taskId = 1;
            int sprintOrder = 1;
            
            _mockSprintService.Setup(s => s.ValidateTaskForSprintAsync(taskId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AddTaskToSprint(taskId, sprintOrder);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            
            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("An error occurred while adding the task"));
        }

        #endregion

        #region RemoveTaskFromSprint Tests

        [Test]
        public async Task RemoveTaskFromSprint_WithValidTask_ReturnsSuccessJson()
        {
            // Arrange
            int taskId = 1;
            
            _mockSprintService.Setup(s => s.RemoveTaskFromSprintAsync(taskId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveTaskFromSprint(taskId);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            
            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Task removed from sprint successfully"));
            
            _mockSprintService.Verify(s => s.RemoveTaskFromSprintAsync(taskId), Times.Once);
        }

        [Test]
        public async Task RemoveTaskFromSprint_WhenRemovalFails_ReturnsFailureJson()
        {
            // Arrange
            int taskId = 1;
            
            _mockSprintService.Setup(s => s.RemoveTaskFromSprintAsync(taskId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveTaskFromSprint(taskId);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            
            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Failed to remove task from sprint"));
        }

        [Test]
        public async Task RemoveTaskFromSprint_WhenExceptionThrown_ReturnsErrorJson()
        {
            // Arrange
            int taskId = 1;
            
            _mockSprintService.Setup(s => s.RemoveTaskFromSprintAsync(taskId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.RemoveTaskFromSprint(taskId);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            
            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("An error occurred while removing the task"));
        }

        #endregion

        #region UpdateTaskOrder Tests

        [Test]
        public async Task UpdateTaskOrder_WithValidData_ReturnsSuccessJson()
        {
            // Arrange
            var taskOrders = new Dictionary<int, int> { { 1, 5 } };

            _mockSprintService.Setup(s => s.UpdateSprintTaskOrderAsync(taskOrders))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTaskOrder(taskOrders);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Task order updated successfully"));

            _mockSprintService.Verify(s => s.UpdateSprintTaskOrderAsync(taskOrders), Times.Once);
        }

        [Test]
        public async Task UpdateTaskOrder_WhenUpdateFails_ReturnsFailureJson()
        {
            // Arrange
            var taskOrders = new Dictionary<int, int> { { 1, 5 } };

            _mockSprintService.Setup(s => s.UpdateSprintTaskOrderAsync(taskOrders))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateTaskOrder(taskOrders);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Failed to update task order"));
        }

        [Test]
        public async Task UpdateTaskOrder_WhenExceptionThrown_ReturnsErrorJson()
        {
            // Arrange
            var taskOrders = new Dictionary<int, int> { { 1, 5 } };

            _mockSprintService.Setup(s => s.UpdateSprintTaskOrderAsync(taskOrders))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateTaskOrder(taskOrders);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("An error occurred while updating task order"));
        }

        #endregion

        #region UpdateEstimatedTime Tests

        [Test]
        public async Task UpdateEstimatedTime_WithValidData_ReturnsSuccessJson()
        {
            // Arrange
            int taskId = 1;
            int estimatedTime = 16;

            _mockSprintService.Setup(s => s.UpdateTaskEstimatedTimeAsync(taskId, estimatedTime))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateEstimatedTime(taskId, estimatedTime);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Estimated time updated successfully"));

            _mockSprintService.Verify(s => s.UpdateTaskEstimatedTimeAsync(taskId, estimatedTime), Times.Once);
        }

        [Test]
        public async Task UpdateEstimatedTime_WhenUpdateFails_ReturnsFailureJson()
        {
            // Arrange
            int taskId = 1;
            int estimatedTime = 16;

            _mockSprintService.Setup(s => s.UpdateTaskEstimatedTimeAsync(taskId, estimatedTime))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateEstimatedTime(taskId, estimatedTime);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Failed to update estimated time"));
        }

        [Test]
        public async Task UpdateEstimatedTime_WhenExceptionThrown_ReturnsErrorJson()
        {
            // Arrange
            int taskId = 1;
            int estimatedTime = 16;

            _mockSprintService.Setup(s => s.UpdateTaskEstimatedTimeAsync(taskId, estimatedTime))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateEstimatedTime(taskId, estimatedTime);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("An error occurred while updating estimated time"));
        }

        #endregion

        #region AutoAssignTasks Tests

        [Test]
        public async Task AutoAssignTasks_WithSuccessfulAssignment_ReturnsSuccessJson()
        {
            // Arrange
            int maxTasks = 5;
            int assignedCount = 3;

            _mockSprintService.Setup(s => s.AutoAssignTasksToSprintAsync(maxTasks))
                .ReturnsAsync(assignedCount);

            // Act
            var result = await _controller.AutoAssignTasks(maxTasks);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message, count) = ParseJsonResultWithCount(jsonResult!);
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo($"{assignedCount} tasks assigned successfully"));
            Assert.That(count, Is.EqualTo(assignedCount));

            _mockSprintService.Verify(s => s.AutoAssignTasksToSprintAsync(maxTasks), Times.Once);
            _mockTempData.VerifySet(td => td[UserMessageSuccess] = $"{assignedCount} tasks auto-assigned to sprint", Times.Once);
        }

        [Test]
        public async Task AutoAssignTasks_WithNoTasksAssigned_ReturnsFailureJson()
        {
            // Arrange
            int maxTasks = 5;
            int assignedCount = 0;

            _mockSprintService.Setup(s => s.AutoAssignTasksToSprintAsync(maxTasks))
                .ReturnsAsync(assignedCount);

            // Act
            var result = await _controller.AutoAssignTasks(maxTasks);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message, count) = ParseJsonResultWithCount(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("No tasks could be assigned (capacity or validation constraints)"));
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task AutoAssignTasks_WithDefaultMaxTasks_UsesDefaultValue()
        {
            // Arrange
            int defaultMaxTasks = 10;
            int assignedCount = 2;

            _mockSprintService.Setup(s => s.AutoAssignTasksToSprintAsync(defaultMaxTasks))
                .ReturnsAsync(assignedCount);

            // Act
            var result = await _controller.AutoAssignTasks();

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            _mockSprintService.Verify(s => s.AutoAssignTasksToSprintAsync(defaultMaxTasks), Times.Once);
        }

        [Test]
        public async Task AutoAssignTasks_WhenExceptionThrown_ReturnsErrorJson()
        {
            // Arrange
            int maxTasks = 5;

            _mockSprintService.Setup(s => s.AutoAssignTasksToSprintAsync(maxTasks))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AutoAssignTasks(maxTasks);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var (success, message) = ParseJsonResult(jsonResult!);
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("An error occurred during auto-assignment"));
        }

        #endregion
    }
}
