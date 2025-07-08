using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
	[TestFixture]
	public class MachineServiceUnitTests
	{
		private IRepository _repository;
		private IMachineService _machineService;
		private TeamWorkFlowDbContext _dbContext;
		private Mock<UserManager<IdentityUser>> _mockUserManager;

		[SetUp]
	public void Setup()
	{
		var mockUserStore = new Mock<IUserStore<IdentityUser>>();
		_mockUserManager = new Mock<UserManager<IdentityUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

		_mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
			.ReturnsAsync((IdentityUser user, string role) => role == "Operator" || role == "Admin");

		// Check if running in CI/CD environment
		bool useInMemoryDatabase = Environment.GetEnvironmentVariable("USE_IN_MEMORY_DATABASE") == "true";

		if (useInMemoryDatabase)
		{
        // Use in-memory database for CI/CD
        var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TeamWorkFlowDbContext(options);
    }
    else
    {
        // Use real SQL Server for local development
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<MachineServiceUnitTests>()
            .Build();

        var connectionString = configuration.GetConnectionString("Test");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Test connection string is not configured. Please set up user secrets with 'Test' connection string.");
        }

        var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        _dbContext = new TeamWorkFlowDbContext(options);
    }

    _repository = new Repository(_dbContext);
    _machineService = new MachineService(_repository);

    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
}

		[Test]
		public async Task GetAllMachinesAsync_ShouldReturnAllMachines()
		{
			var machines = await _repository.AllReadOnly<Machine>().ToListAsync();
			Assert.That(machines, Is.Not.Null, "Machine collection is null");

			// Act
			var result = await _machineService.GetAllMachinesAsync();

			// Assert
			Assert.That(result, Is.Not.Null, "MachineServiceModel collection is null");
			Assert.That(result.Count, Is.EqualTo(machines.Count), "Result is not equal to Machine count");
		}

		[Test]
		public async Task AddNewMachineAsync_ShouldAddNewMachine()
		{
			// Arrange

			bool isCalibrated = true;
			var calibrationDate = DateTime.ParseExact("02/02/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture);
			var expectedMachineCount = await _repository.AllReadOnly<Machine>().CountAsync() + 1;

			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = calibrationDate.ToString("dd/MM/yyyy"),
				Capacity = 10,
				IsCalibrated = isCalibrated.ToString(),
				ImageUrl = "https://www.example.com"
			};

			// Act
			await _machineService.AddNewMachineAsync(model);
			var result = await _repository.AllReadOnly<Machine>().CountAsync();

			// Assert
			Assert.That(result, Is.EqualTo(expectedMachineCount), "Machine count is not equal to expected count");
		}

		[Test]
		public Task AddNewMachineAsync_ShouldThrowArgumentException_WhenInvalidDate()
		{
			// Arrange
			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = "02/02-2024",
				Capacity = 10,
				IsCalibrated = "true",
				ImageUrl = "https://www.example.com"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.AddNewMachineAsync(model));
			return Task.CompletedTask;
		}

		[Test]
		public Task AddNewMachineAsync_ShouldThrowArgumentException_WhenInvalidBoolean()
		{
			// Arrange
			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = "02/02/2024",
				Capacity = 10,
				IsCalibrated = "yes",
				ImageUrl = "https://www.example.com"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.AddNewMachineAsync(model));
			return Task.CompletedTask;
		}

		[Test]
		public Task AddNewMachineAsync_ShouldThrowArgumentException_WhenInvalidCapacity()
		{
			// Arrange
			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = "02/02/2024",
				Capacity = 0,
				IsCalibrated = "true",
				ImageUrl = "https://www.example.com"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.AddNewMachineAsync(model));
			return Task.CompletedTask;
		}

		[Test]
		public async Task GetMachineForEditAsync_ShouldReturnMachineForEdit()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			var expectedMachine = new MachineFormModel
			{
				Id = machine.Id,
				Name = machine.Name,
				CalibrationSchedule = machine.CalibrationSchedule.ToString(Messages.DateFormat, CultureInfo.InvariantCulture),
				Capacity = machine.Capacity,
				IsCalibrated = machine.IsCalibrated.ToString(),
				ImageUrl = machine.ImageUrl
			};

			// Act
			var result = await _machineService.GetMachineForEditAsync(machine.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "MachineFormModel is null");
			Assert.That(result.Id, Is.EqualTo(expectedMachine.Id), "Id is not equal");
			Assert.That(result.Name, Is.EqualTo(expectedMachine.Name), "Name is not equal");
			Assert.That(result.CalibrationSchedule, Is.EqualTo(expectedMachine.CalibrationSchedule), "CalibrationSchedule is not equal");
			Assert.That(result.Capacity, Is.EqualTo(expectedMachine.Capacity), "Capacity is not equal");
			Assert.That(result.IsCalibrated, Is.EqualTo(expectedMachine.IsCalibrated), "IsCalibrated is not equal");
			Assert.That(result.ImageUrl, Is.EqualTo(expectedMachine.ImageUrl), "ImageUrl is not equal");
		}

		[Test]
		public Task GetMachineForEditAsync_ShouldThrowArgumentException_WhenInvalidId()
		{
			// Arrange
			var invalidId = 0;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.GetMachineForEditAsync(invalidId));
			return Task.CompletedTask;
		}

		[Test]
		public Task GetMachineForEditAsync_ShouldThrowArgumentException_WhenMachineIsNull()
		{
			// Arrange
			var invalidId = 100;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.GetMachineForEditAsync(invalidId));
			return Task.CompletedTask;
		}

		[Test]
		public async Task EditMachineAsync_ShouldEditMachine()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = machine.CalibrationSchedule.ToString(Messages.DateFormat, CultureInfo.InvariantCulture),
				Capacity = 10,
				IsCalibrated = "true",
				ImageUrl = "https://www.example.com"
			};

			// Act
			await _machineService.EditMachineAsync(model, machine.Id);
			var result = await _repository.GetByIdAsync<Machine>(machine.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "Machine is null");
			Assert.That(result.Name, Is.EqualTo(model.Name), "Name is not equal");
			Assert.That(result.CalibrationSchedule, Is.EqualTo(machine.CalibrationSchedule), "CalibrationSchedule is not equal");
			Assert.That(result.Capacity, Is.EqualTo(model.Capacity), "Capacity is not equal");
			Assert.That(result.IsCalibrated, Is.EqualTo(true), "IsCalibrated is not equal");
			Assert.That(result.ImageUrl, Is.EqualTo(model.ImageUrl), "ImageUrl is not equal");
		}

		[Test]
		public async Task EditMachineAsync_ShouldThrowArgumentException_WhenInvalidBoolean()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = machine.CalibrationSchedule.ToString(Messages.DateFormat, CultureInfo.InvariantCulture),
				Capacity = 10,
				IsCalibrated = "yes",
				ImageUrl = "https://www.example.com"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.EditMachineAsync(model, machine.Id));
		}
		
		[Test]
		public async Task EditMachineAsync_ShouldThrowArgumentException_WhenInvalidDate()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = "02/02-2024",
				Capacity = 10,
				IsCalibrated = "true",
				ImageUrl = "https://www.example.com"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.EditMachineAsync(model, machine.Id));
		}

		[Test]
		public async Task EditMachineAsync_ShouldThrowArgumentException_WhenInvalidCapacity()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			var model = new MachineFormModel
			{
				Name = "Machine 1",
				CalibrationSchedule = "02/02/2024",
				Capacity = 50,
				IsCalibrated = "true",
				ImageUrl = "https://www.example.com"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.EditMachineAsync(model, machine.Id));
		}


		// Must be Fixed. Now assigned machine to task cannot be deleted
		//[Test]
		//public async Task DeleteMachineAsync_ShouldDeleteMachine()
		//{
		//	// Arrange
		//	var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
		//	var expectedMachineCount = await _repository.AllReadOnly<Machine>().CountAsync() - 1;
		//	Assert.That(machine, Is.Not.Null, "Machine is null");
		//
		//	// Act
		//	await _machineService.DeleteMachineAsync(machine.Id);
		//	var result = await _repository.AllReadOnly<Machine>().CountAsync();
		//
		//	// Assert
		//	Assert.That(result, Is.EqualTo(expectedMachineCount), "Machine count is not equal to expected count");
		//}

		[Test]
		public async Task GetMachineForDeleteByIdAsync_ShouldReturnMachineForDelete()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			Assert.That(machine, Is.Not.Null, "Machine is null");

			// Act
			var result = await _machineService.GetMachineForDeleteByIdAsync(machine.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "MachineFormModel is null");
			Assert.That(result.Id, Is.EqualTo(machine.Id), "Id is not equal");
			Assert.That(result.Name, Is.EqualTo(machine.Name), "Name is not equal");
			Assert.That(result.ImageUrl, Is.EqualTo(machine.ImageUrl), "ImageUrl is not equal");
		}
		
		[Test]
		public async Task MachineExistByIdAsync_ShouldReturnTrue()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			Assert.That(machine, Is.Not.Null, "Machine is null");

			// Act
			var result = await _machineService.MachineExistByIdAsync(machine.Id);

			// Assert
			Assert.That(result, Is.True, "Machine does not exist");
		}

		[Test]
		public async Task MachineExistByIdAsync_ShouldReturnFalse()
		{
			// Arrange
			var invalidId = 100;

			// Act
			var result = await _machineService.MachineExistByIdAsync(invalidId);

			// Assert
			Assert.That(result, Is.False, "Machine does exist");
		}

		[Test]
		public async Task MachineDetailsAsync_ShouldReturnMachineDetails()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			Assert.That(machine, Is.Not.Null, "Machine is null");

			var expectedMachine = new MachineDetailsServiceModel
			{
				Id = machine.Id,
				ImageUrl = machine.ImageUrl,
				Name = machine.Name,
				Capacity = machine.Capacity,
				CalibrationSchedule = machine.CalibrationSchedule.ToString(Messages.DateFormat, CultureInfo.InvariantCulture),
				IsCalibrated = machine.IsCalibrated,
				TotalMachineLoad = machine.TotalMachineLoad
			};

			// Act
			var result = await _machineService.MachineDetailsAsync(machine.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "MachineDetailsServiceModel is null");
			Assert.That(result.Id, Is.EqualTo(expectedMachine.Id), "Id is not equal");
			Assert.That(result.ImageUrl, Is.EqualTo(expectedMachine.ImageUrl), "ImageUrl is not equal");
			Assert.That(result.Name, Is.EqualTo(expectedMachine.Name), "Name is not equal");
			Assert.That(result.Capacity, Is.EqualTo(expectedMachine.Capacity), "Capacity is not equal");
			Assert.That(result.CalibrationSchedule, Is.EqualTo(expectedMachine.CalibrationSchedule), "CalibrationSchedule is not equal");
			Assert.That(result.IsCalibrated, Is.EqualTo(expectedMachine.IsCalibrated), "IsCalibrated is not equal");
			Assert.That(result.TotalMachineLoad, Is.EqualTo(expectedMachine.TotalMachineLoad), "TotalMachineLoad is not equal");
		}




		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
