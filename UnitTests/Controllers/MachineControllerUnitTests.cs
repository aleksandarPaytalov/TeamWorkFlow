using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using TeamWorkFlow.Controllers;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Machine;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class MachineControllerUnitTests
    {
        private MachineController _controller = null!;
        private Mock<IMachineService> _mockMachineService = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<ClaimsPrincipal> _mockUser = null!;
        private Mock<ITempDataDictionary> _mockTempData = null!;

        [SetUp]
        public void SetUp()
        {
            _mockMachineService = new Mock<IMachineService>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockUser = new Mock<ClaimsPrincipal>();
            _mockTempData = new Mock<ITempDataDictionary>();

            _controller = new MachineController(_mockMachineService.Object);

            var controllerContext = new ControllerContext()
            {
                HttpContext = _mockHttpContext.Object
            };

            _controller.ControllerContext = controllerContext;
            _controller.TempData = _mockTempData.Object;
            _mockHttpContext.Setup(x => x.User).Returns(_mockUser.Object);
        }

        [Test]
        public void MachineController_ShouldInheritFromBaseController()
        {
            // Assert
            Assert.That(_controller, Is.InstanceOf<BaseController>());
        }

        [Test]
        public void MachineController_Constructor_WithValidService_ShouldCreateInstance()
        {
            // Act & Assert
            Assert.That(_controller, Is.Not.Null);
        }

        // Note: MachineController constructor does not validate null parameters

        [Test]
        public async Task All_WithUnauthenticatedUser_ShouldReturnChallenge()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var model = new AllMachinesQueryModel();

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public async Task All_WithAuthenticatedAdminUser_ShouldReturnViewWithMachines()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(false);

            var model = new AllMachinesQueryModel();
            var machineQueryResult = new MachineQueryServiceModel
            {
                TotalMachinesCount = 3,
                Machines = new List<MachineServiceModel>
                {
                    new MachineServiceModel { Id = 1, Name = "CNC Machine 1" },
                    new MachineServiceModel { Id = 2, Name = "CNC Machine 2" }
                }
            };

            _mockMachineService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.MachineSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(machineQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllMachinesQueryModel>());
            
            var resultModel = (AllMachinesQueryModel)viewResult.Model!;
            Assert.That(resultModel.TotalMachinesCount, Is.EqualTo(3));
            Assert.That(resultModel.Machines.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task All_WithAuthenticatedOperatorUser_ShouldReturnViewWithMachines()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(true);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(false);

            var model = new AllMachinesQueryModel();
            var machineQueryResult = new MachineQueryServiceModel
            {
                TotalMachinesCount = 2,
                Machines = new List<MachineServiceModel>
                {
                    new MachineServiceModel { Id = 1, Name = "CNC Machine 1" }
                }
            };

            _mockMachineService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.MachineSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(machineQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllMachinesQueryModel>());
            
            var resultModel = (AllMachinesQueryModel)viewResult.Model!;
            Assert.That(resultModel.TotalMachinesCount, Is.EqualTo(2));
        }

        [Test]
        public async Task All_WithAuthenticatedGuestUser_ShouldReturnViewWithMachines()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Operator")).Returns(false);
            _mockUser.Setup(x => x.IsInRole("Guest")).Returns(true);

            var model = new AllMachinesQueryModel();
            var machineQueryResult = new MachineQueryServiceModel
            {
                TotalMachinesCount = 1,
                Machines = new List<MachineServiceModel>
                {
                    new MachineServiceModel { Id = 1, Name = "CNC Machine 1" }
                }
            };

            _mockMachineService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.MachineSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(machineQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllMachinesQueryModel>());
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

            var model = new AllMachinesQueryModel();

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public void Add_WithNonAdminUser_ShouldReturnUnauthorized()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(false);

            // Act
            var result = _controller.Add();

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public void Add_WithAdminUser_ShouldReturnViewWithMachineFormModel()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            // Act
            var result = _controller.Add();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<MachineFormModel>());
        }

        [Test]
        public async Task All_CallsMachineServiceWithCorrectParameters()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            var model = new AllMachinesQueryModel
            {
                Sorting = TeamWorkFlow.Core.Enumerations.MachineSorting.NameAscending,
                Search = "CNC",
                CurrentPage = 2
            };

            var machineQueryResult = new MachineQueryServiceModel
            {
                TotalMachinesCount = 0,
                Machines = new List<MachineServiceModel>()
            };

            _mockMachineService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.MachineSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(machineQueryResult);

            // Act
            await _controller.All(model);

            // Assert
            _mockMachineService.Verify(x => x.AllAsync(
                model.Sorting,
                model.Search,
                model.MachinesPerPage,
                model.CurrentPage
            ), Times.Once);
        }

        [Test]
        public async Task All_WithNullIdentity_ShouldReturnChallenge()
        {
            // Arrange
            _mockUser.Setup(x => x.Identity).Returns((ClaimsIdentity?)null);
            var model = new AllMachinesQueryModel();

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public void Add_ReturnsNewMachineFormModel()
        {
            // Arrange
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            // Act
            var result = _controller.Add();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            var model = (MachineFormModel)viewResult.Model!;
            
            // Verify it's a new model with default values
            Assert.That(model.Id, Is.EqualTo(0));
            Assert.That(model.Name, Is.EqualTo(string.Empty));
        }

        [Test]
        public void MachineController_ShouldHaveCorrectNamespace()
        {
            // Act
            var controllerNamespace = typeof(MachineController).Namespace;

            // Assert
            Assert.That(controllerNamespace, Is.EqualTo("TeamWorkFlow.Controllers"));
        }

        [Test]
        public void MachineController_AllPublicMethods_ShouldBeAccessible()
        {
            // Arrange
            var publicMethods = typeof(MachineController).GetMethods()
                .Where(m => m.IsPublic && m.DeclaringType == typeof(MachineController))
                .Select(m => m.Name)
                .ToList();

            // Assert
            Assert.That(publicMethods, Does.Contain("All"));
            Assert.That(publicMethods, Does.Contain("Add"));
        }

        [Test]
        public async Task All_WithEmptySearchAndDefaultPaging_ShouldWork()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            var model = new AllMachinesQueryModel(); // Default values

            var machineQueryResult = new MachineQueryServiceModel
            {
                TotalMachinesCount = 10,
                Machines = new List<MachineServiceModel>()
            };

            _mockMachineService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.MachineSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(machineQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            var resultModel = (AllMachinesQueryModel)viewResult.Model!;
            Assert.That(resultModel.TotalMachinesCount, Is.EqualTo(10));
        }

        [Test]
        public async Task All_SetsModelPropertiesCorrectly()
        {
            // Arrange
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            _mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);
            _mockUser.Setup(x => x.IsInRole("Administrator")).Returns(true);

            var model = new AllMachinesQueryModel
            {
                Search = "test",
                Sorting = TeamWorkFlow.Core.Enumerations.MachineSorting.NameDescending
            };

            var machines = new List<MachineServiceModel>
            {
                new MachineServiceModel { Id = 1, Name = "Machine 1" },
                new MachineServiceModel { Id = 2, Name = "Machine 2" }
            };

            var machineQueryResult = new MachineQueryServiceModel
            {
                TotalMachinesCount = 15,
                Machines = machines
            };

            _mockMachineService.Setup(x => x.AllAsync(
                It.IsAny<TeamWorkFlow.Core.Enumerations.MachineSorting>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(machineQueryResult);

            // Act
            var result = await _controller.All(model);

            // Assert
            var viewResult = (ViewResult)result;
            var resultModel = (AllMachinesQueryModel)viewResult.Model!;
            
            Assert.That(resultModel.TotalMachinesCount, Is.EqualTo(15));
            Assert.That(resultModel.Machines, Is.EqualTo(machines));
            Assert.That(resultModel.Search, Is.EqualTo("test"));
            Assert.That(resultModel.Sorting, Is.EqualTo(TeamWorkFlow.Core.Enumerations.MachineSorting.NameDescending));
        }
    }
}
