using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Areas.Admin.Controllers;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Admin.Operator;
using TeamWorkFlow.Core.Models.Operator;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace UnitTests
{
	[TestFixture]
	public class AdminOperatorControllerUnitTests
	{
		private Mock<IOperatorService> _mockOperatorService;
		private Mock<IMemoryCache> _mockMemoryCache;
		private Mock<ITempDataDictionary> _mockTempData;
		private OperatorController _controller;

		[SetUp]
		public void Setup()
		{
			_mockOperatorService = new Mock<IOperatorService>();
			_mockMemoryCache = new Mock<IMemoryCache>();
			_mockTempData = new Mock<ITempDataDictionary>();

			_controller = new OperatorController(_mockOperatorService.Object, _mockMemoryCache.Object);
			
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

		#region DeactivateWithStatus Tests

		[Test]
		public async Task DeactivateWithStatus_WithValidParameters_DeactivatesOperatorAndRedirects()
		{
			// Arrange
			int operatorId = 1;
			int statusId = 2;
			var operatorDetails = new OperatorDetailsServiceModel
			{
				Id = operatorId,
				FullName = "Test Operator",
				IsActive = true
			};
			var statuses = new List<AvailabilityStatusServiceModel>
			{
				new AvailabilityStatusServiceModel { Id = 2, Name = "On Vacation" }
			};

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync(operatorDetails);
			_mockOperatorService.Setup(s => s.OperatorStatusExistAsync(statusId))
				.ReturnsAsync(true);
			_mockOperatorService.Setup(s => s.GetAllOperatorStatusesAsync())
				.ReturnsAsync(statuses);

			// Act
			var result = await _controller.DeactivateWithStatus(operatorId, statusId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.DeactivateOperatorWithStatusAsync(operatorId, statusId), Times.Once);
			_mockMemoryCache.Verify(c => c.Remove(UserCacheKey), Times.Once);
		}

		[Test]
		public async Task DeactivateWithStatus_WithInvalidOperatorId_SetsErrorMessageAndRedirects()
		{
			// Arrange
			int invalidOperatorId = 0;
			int statusId = 2;

			// Act
			var result = await _controller.DeactivateWithStatus(invalidOperatorId, statusId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.DeactivateOperatorWithStatusAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task DeactivateWithStatus_WithInvalidStatusId_SetsErrorMessageAndRedirects()
		{
			// Arrange
			int operatorId = 1;
			int invalidStatusId = 0;

			// Act
			var result = await _controller.DeactivateWithStatus(operatorId, invalidStatusId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.DeactivateOperatorWithStatusAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task DeactivateWithStatus_WithNonExistentOperator_SetsErrorMessageAndRedirects()
		{
			// Arrange
			int operatorId = 999;
			int statusId = 2;

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync((OperatorDetailsServiceModel?)null);

			// Act
			var result = await _controller.DeactivateWithStatus(operatorId, statusId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.DeactivateOperatorWithStatusAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task DeactivateWithStatus_WithNonExistentStatus_SetsErrorMessageAndRedirects()
		{
			// Arrange
			int operatorId = 1;
			int invalidStatusId = 999;
			var operatorDetails = new OperatorDetailsServiceModel
			{
				Id = operatorId,
				FullName = "Test Operator",
				IsActive = true
			};

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync(operatorDetails);
			_mockOperatorService.Setup(s => s.OperatorStatusExistAsync(invalidStatusId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.DeactivateWithStatus(operatorId, invalidStatusId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.DeactivateOperatorWithStatusAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task DeactivateWithStatus_WithServiceException_SetsErrorMessageAndRedirects()
		{
			// Arrange
			int operatorId = 1;
			int statusId = 2;
			var operatorDetails = new OperatorDetailsServiceModel
			{
				Id = operatorId,
				FullName = "Test Operator",
				IsActive = true
			};

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync(operatorDetails);
			_mockOperatorService.Setup(s => s.OperatorStatusExistAsync(statusId))
				.ReturnsAsync(true);
			_mockOperatorService.Setup(s => s.DeactivateOperatorWithStatusAsync(operatorId, statusId))
				.ThrowsAsync(new Exception("Test exception"));

			// Act
			var result = await _controller.DeactivateWithStatus(operatorId, statusId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));
		}

		#endregion

		#region GetAvailabilityStatuses Tests

		[Test]
		public async Task GetAvailabilityStatuses_ReturnsFilteredStatusesAsJson()
		{
			// Arrange
			var allStatuses = new List<AvailabilityStatusServiceModel>
			{
				new AvailabilityStatusServiceModel { Id = 1, Name = "At Work" },
				new AvailabilityStatusServiceModel { Id = 2, Name = "On Vacation" },
				new AvailabilityStatusServiceModel { Id = 3, Name = "Sick Leave" }
			};

			_mockOperatorService.Setup(s => s.GetAllOperatorStatusesAsync())
				.ReturnsAsync(allStatuses);

			// Act
			var result = await _controller.GetAvailabilityStatuses();

			// Assert
			Assert.That(result, Is.InstanceOf<JsonResult>());
			var jsonResult = result as JsonResult;
			Assert.That(jsonResult, Is.Not.Null);
			var returnedStatuses = jsonResult!.Value as List<AvailabilityStatusServiceModel>;

			Assert.That(returnedStatuses, Is.Not.Null);
			Assert.That(returnedStatuses!.Count, Is.EqualTo(2)); // Should exclude "At Work" (ID = 1)
			Assert.That(returnedStatuses.Any(s => s.Id == 1), Is.False); // "At Work" should be filtered out
			Assert.That(returnedStatuses.Any(s => s.Id == 2), Is.True); // "On Vacation" should be included
			Assert.That(returnedStatuses.Any(s => s.Id == 3), Is.True); // "Sick Leave" should be included
		}

		[Test]
		public async Task GetAvailabilityStatuses_WithServiceException_ReturnsEmptyList()
		{
			// Arrange
			_mockOperatorService.Setup(s => s.GetAllOperatorStatusesAsync())
				.ThrowsAsync(new Exception("Test exception"));

			// Act
			var result = await _controller.GetAvailabilityStatuses();

			// Assert
			Assert.That(result, Is.InstanceOf<JsonResult>());
			var jsonResult = result as JsonResult;
			Assert.That(jsonResult, Is.Not.Null);
			var returnedStatuses = jsonResult!.Value as List<object>;

			Assert.That(returnedStatuses, Is.Not.Null);
			Assert.That(returnedStatuses!.Count, Is.EqualTo(0));
		}

		#endregion

		#region ToggleStatus Tests

		[Test]
		public async Task ToggleStatus_WithActiveOperator_DeactivatesOperator()
		{
			// Arrange
			int operatorId = 1;
			var operatorDetails = new OperatorDetailsServiceModel
			{
				Id = operatorId,
				FullName = "Active Test Operator",
				IsActive = true
			};

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync(operatorDetails);

			// Act
			var result = await _controller.ToggleStatus(operatorId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.DeactivateOperatorAsync(operatorId), Times.Once);
			_mockOperatorService.Verify(s => s.ActivateOperatorAsync(It.IsAny<int>()), Times.Never);
			_mockMemoryCache.Verify(c => c.Remove(UserCacheKey), Times.Once);
		}

		[Test]
		public async Task ToggleStatus_WithInactiveOperator_ActivatesOperator()
		{
			// Arrange
			int operatorId = 1;
			var operatorDetails = new OperatorDetailsServiceModel
			{
				Id = operatorId,
				FullName = "Inactive Test Operator",
				IsActive = false
			};

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync(operatorDetails);

			// Act
			var result = await _controller.ToggleStatus(operatorId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.ActivateOperatorAsync(operatorId), Times.Once);
			_mockOperatorService.Verify(s => s.DeactivateOperatorAsync(It.IsAny<int>()), Times.Never);
			_mockMemoryCache.Verify(c => c.Remove(UserCacheKey), Times.Once);
		}

		[Test]
		public async Task ToggleStatus_WithNonExistentOperator_SetsErrorMessageAndRedirects()
		{
			// Arrange
			int operatorId = 999;

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync((OperatorDetailsServiceModel?)null);

			// Act
			var result = await _controller.ToggleStatus(operatorId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));

			_mockOperatorService.Verify(s => s.ActivateOperatorAsync(It.IsAny<int>()), Times.Never);
			_mockOperatorService.Verify(s => s.DeactivateOperatorAsync(It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task ToggleStatus_WithServiceException_SetsErrorMessageAndRedirects()
		{
			// Arrange
			int operatorId = 1;
			var operatorDetails = new OperatorDetailsServiceModel
			{
				Id = operatorId,
				FullName = "Test Operator",
				IsActive = true
			};

			_mockOperatorService.Setup(s => s.GetOperatorDetailsByIdAsync(operatorId))
				.ReturnsAsync(operatorDetails);
			_mockOperatorService.Setup(s => s.DeactivateOperatorAsync(operatorId))
				.ThrowsAsync(new Exception("Test exception"));

			// Act
			var result = await _controller.ToggleStatus(operatorId);

			// Assert
			Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult!.ActionName, Is.EqualTo(nameof(_controller.All)));
		}

		#endregion
	}
}
