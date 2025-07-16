using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
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
		private IRepository _repository = null!;
		private IMachineService _machineService = null!;
		private TeamWorkFlowDbContext _dbContext = null!;
		private Mock<UserManager<IdentityUser>> _mockUserManager = null!;

		[SetUp]
	public void Setup()
	{
		var mockUserStore = new Mock<IUserStore<IdentityUser>>();
		_mockUserManager = new Mock<UserManager<IdentityUser>>(mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

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

		// ============================================================================
		// ADDITIONAL TESTS FOR 90%+ COVERAGE
		// ============================================================================

		#region AllAsync Method Tests (Query and Search Functionality)

		[Test]
		public async Task AllAsync_WithNoParameters_ReturnsAllMachines()
		{
			// Arrange
			var expectedMachineCount = await _repository.AllReadOnly<Machine>().CountAsync();

			// Act
			var result = await _machineService.AllAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalMachinesCount, Is.EqualTo(expectedMachineCount));
			Assert.That(result.Machines, Is.Not.Null);
			Assert.That(result.Machines.Count(), Is.LessThanOrEqualTo(10)); // Default page size
		}

		[Test]
		public async Task AllAsync_WithSearchParameter_ReturnsFilteredMachines()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			Assert.That(machine, Is.Not.Null, "Machine is null");
			string searchTerm = machine.Name.Substring(0, 3); // Search for part of machine name

			// Act
			var result = await _machineService.AllAsync(search: searchTerm);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
			// Verify that returned machines contain the search term
			foreach (var machineModel in result.Machines)
			{
				bool containsSearchTerm = machineModel.Name.ToLower().Contains(searchTerm.ToLower());
				Assert.That(containsSearchTerm, Is.True, $"Machine {machineModel.Name} should contain search term {searchTerm}");
			}
		}

		[Test]
		public async Task AllAsync_WithNonExistentSearch_ReturnsEmptyResult()
		{
			// Arrange
			string nonExistentSearch = "NonExistentMachineName12345";

			// Act
			var result = await _machineService.AllAsync(search: nonExistentSearch);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines.Count(), Is.EqualTo(0));
			Assert.That(result.TotalMachinesCount, Is.EqualTo(0));
		}

		[Test]
		public async Task AllAsync_WithSortingByNameAscending_ReturnsSortedMachines()
		{
			// Arrange & Act
			var result = await _machineService.AllAsync(sorting: MachineSorting.NameAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
			if (result.Machines.Count() > 1)
			{
				var machineNames = result.Machines.Select(m => m.Name).ToList();
				var sortedNames = machineNames.OrderBy(n => n).ToList();
				Assert.That(machineNames, Is.EqualTo(sortedNames), "Machines should be sorted by name ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByNameDescending_ReturnsSortedMachines()
		{
			// Arrange & Act
			var result = await _machineService.AllAsync(sorting: MachineSorting.NameDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
			if (result.Machines.Count() > 1)
			{
				var machineNames = result.Machines.Select(m => m.Name).ToList();
				var sortedNames = machineNames.OrderByDescending(n => n).ToList();
				Assert.That(machineNames, Is.EqualTo(sortedNames), "Machines should be sorted by name descending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByCapacityAscending_ReturnsSortedMachines()
		{
			// Arrange & Act
			var result = await _machineService.AllAsync(sorting: MachineSorting.CapacityAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
			if (result.Machines.Count() > 1)
			{
				var capacities = result.Machines.Select(m => m.Capacity).ToList();
				var sortedCapacities = capacities.OrderBy(c => c).ToList();
				Assert.That(capacities, Is.EqualTo(sortedCapacities), "Machines should be sorted by capacity ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByCapacityDescending_ReturnsSortedMachines()
		{
			// Arrange & Act
			var result = await _machineService.AllAsync(sorting: MachineSorting.CapacityDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
			if (result.Machines.Count() > 1)
			{
				var capacities = result.Machines.Select(m => m.Capacity).ToList();
				var sortedCapacities = capacities.OrderByDescending(c => c).ToList();
				Assert.That(capacities, Is.EqualTo(sortedCapacities), "Machines should be sorted by capacity descending");
			}
		}

		[Test]
		public async Task AllAsync_WithAllSortingOptions_ExecutesSuccessfully()
		{
			// Test all sorting enum values to ensure complete coverage
			var sortingOptions = Enum.GetValues<MachineSorting>();

			foreach (var sorting in sortingOptions)
			{
				// Act
				var result = await _machineService.AllAsync(sorting: sorting);

				// Assert
				Assert.That(result, Is.Not.Null, $"Result should not be null for sorting: {sorting}");
				Assert.That(result.Machines, Is.Not.Null, $"Machines should not be null for sorting: {sorting}");
			}
		}

		[Test]
		public async Task AllAsync_WithCustomPagination_ReturnsCorrectPage()
		{
			// Arrange
			int machinesPerPage = 2;
			int currentPage = 1;

			// Act
			var result = await _machineService.AllAsync(machinesPerPage: machinesPerPage, currentPage: currentPage);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
			Assert.That(result.Machines.Count(), Is.LessThanOrEqualTo(machinesPerPage));
		}

		[Test]
		public async Task AllAsync_WithNullSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _machineService.AllAsync(search: null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithEmptySearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _machineService.AllAsync(search: "");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithWhitespaceSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _machineService.AllAsync(search: "   ");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Machines, Is.Not.Null);
		}

		#endregion

		#region Pagination Method Tests

		[Test]
		public async Task GetAllMachinesAsync_WithPagination_ReturnsCorrectPage()
		{
			// Arrange
			int page = 1;
			int pageSize = 2;
			var totalMachines = await _repository.AllReadOnly<Machine>().CountAsync();

			// Act
			var result = await _machineService.GetAllMachinesAsync(page, pageSize);

			// Assert
			Assert.That(result.Machines, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalMachines));
			Assert.That(result.Machines.Count(), Is.LessThanOrEqualTo(pageSize));
		}

		[Test]
		public async Task GetAllMachinesAsync_WithSecondPage_ReturnsCorrectMachines()
		{
			// Arrange
			int page = 2;
			int pageSize = 1;
			var totalMachines = await _repository.AllReadOnly<Machine>().CountAsync();

			// Act
			var result = await _machineService.GetAllMachinesAsync(page, pageSize);

			// Assert
			Assert.That(result.Machines, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalMachines));
			if (totalMachines > 1)
			{
				Assert.That(result.Machines.Count(), Is.LessThanOrEqualTo(1));
			}
		}

		[Test]
		public async Task GetAllMachinesAsync_WithLargePageSize_ReturnsAllMachines()
		{
			// Arrange
			int page = 1;
			int pageSize = 100;
			var totalMachines = await _repository.AllReadOnly<Machine>().CountAsync();

			// Act
			var result = await _machineService.GetAllMachinesAsync(page, pageSize);

			// Assert
			Assert.That(result.Machines, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalMachines));
			Assert.That(result.Machines.Count(), Is.EqualTo(totalMachines));
		}

		#endregion

		#region DeleteMachineAsync and GetAllTaskByAssignedMachineId Tests

		[Test]
		public async Task GetAllTaskByAssignedMachineId_WithValidMachineId_ReturnsTasks()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			Assert.That(machine, Is.Not.Null, "Machine is null");

			// Act
			var result = await _machineService.GetAllTaskByAssignedMachineId(machine.Id);

			// Assert
			Assert.That(result, Is.Not.Null);
			// The result can be empty if no tasks are assigned to this machine
		}

		[Test]
		public async Task GetAllTaskByAssignedMachineId_WithInvalidMachineId_ReturnsEmptyList()
		{
			// Arrange
			int invalidMachineId = 99999;

			// Act
			var result = await _machineService.GetAllTaskByAssignedMachineId(invalidMachineId);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task DeleteMachineAsync_WithValidMachineId_DeletesMachine()
		{
			// Arrange
			var machineCountBefore = await _repository.AllReadOnly<Machine>().CountAsync();

			// Create a new machine for deletion to avoid affecting other tests
			var newMachine = new Machine
			{
				Name = "Test Machine for Deletion",
				CalibrationSchedule = DateTime.Now.AddDays(30),
				Capacity = 5,
				IsCalibrated = true,
				ImageUrl = "https://test.com/image.jpg",
				TotalMachineLoad = 0
			};

			await _repository.AddAsync(newMachine);
			await _repository.SaveChangesAsync();

			// Act
			await _machineService.DeleteMachineAsync(newMachine.Id);

			// Assert
			var machineCountAfter = await _repository.AllReadOnly<Machine>().CountAsync();
			Assert.That(machineCountAfter, Is.EqualTo(machineCountBefore));

			var deletedMachine = await _repository.GetByIdAsync<Machine>(newMachine.Id);
			Assert.That(deletedMachine, Is.Null);
		}

		[Test]
		public async Task DeleteMachineAsync_WithInvalidMachineId_DoesNothing()
		{
			// Arrange
			int invalidMachineId = 99999;
			var machineCountBefore = await _repository.AllReadOnly<Machine>().CountAsync();

			// Act
			await _machineService.DeleteMachineAsync(invalidMachineId);

			// Assert
			var machineCountAfter = await _repository.AllReadOnly<Machine>().CountAsync();
			Assert.That(machineCountAfter, Is.EqualTo(machineCountBefore));
		}

		#endregion

		#region Error Handling and Edge Cases

		[Test]
		public async Task MachineDetailsAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidMachineId = 99999;

			// Act
			var result = await _machineService.MachineDetailsAsync(invalidMachineId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetMachineForDeleteByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidMachineId = 99999;

			// Act
			var result = await _machineService.GetMachineForDeleteByIdAsync(invalidMachineId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task EditMachineAsync_WithInvalidMachineId_DoesNothing()
		{
			// Arrange
			int invalidMachineId = 99999;
			var model = new MachineFormModel
			{
				Name = "Updated Machine",
				CalibrationSchedule = "01/01/2025",
				Capacity = 15,
				IsCalibrated = "true",
				ImageUrl = "https://updated.com/image.jpg"
			};

			// Act & Assert - Should not throw exception
			await _machineService.EditMachineAsync(model, invalidMachineId);
			// No exception should be thrown, method should handle gracefully
		}

		[Test]
		public void AddNewMachineAsync_WithNegativeCapacity_ThrowsArgumentException()
		{
			// Arrange
			var model = new MachineFormModel
			{
				Name = "Test Machine",
				CalibrationSchedule = "01/01/2025",
				Capacity = -5,
				IsCalibrated = "true",
				ImageUrl = "https://test.com/image.jpg"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.AddNewMachineAsync(model));
		}

		[Test]
		public void AddNewMachineAsync_WithCapacityAboveLimit_ThrowsArgumentException()
		{
			// Arrange
			var model = new MachineFormModel
			{
				Name = "Test Machine",
				CalibrationSchedule = "01/01/2025",
				Capacity = 25, // Above limit of 24
				IsCalibrated = "true",
				ImageUrl = "https://test.com/image.jpg"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.AddNewMachineAsync(model));
		}

		[Test]
		public async Task EditMachineAsync_WithNegativeCapacity_ThrowsArgumentException()
		{
			// Arrange
			var machine = await _repository.AllReadOnly<Machine>().FirstOrDefaultAsync();
			Assert.That(machine, Is.Not.Null, "Machine is null");

			var model = new MachineFormModel
			{
				Name = "Updated Machine",
				CalibrationSchedule = "01/01/2025",
				Capacity = -10,
				IsCalibrated = "true",
				ImageUrl = "https://updated.com/image.jpg"
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => _machineService.EditMachineAsync(model, machine.Id));
		}

		#endregion

		#region Model Coverage Tests

		[Test]
		public void MachineQueryServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new MachineQueryServiceModel();

			// Assert
			Assert.That(model.Machines, Is.Not.Null);
			Assert.That(model.Machines.Count(), Is.EqualTo(0));
			Assert.That(model.TotalMachinesCount, Is.EqualTo(0));
		}

		[Test]
		public void MachineQueryServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var machines = new List<MachineServiceModel>
			{
				new MachineServiceModel { Id = 1, Name = "Machine 1", Capacity = 10 },
				new MachineServiceModel { Id = 2, Name = "Machine 2", Capacity = 15 }
			};

			// Act
			var model = new MachineQueryServiceModel
			{
				TotalMachinesCount = 50,
				Machines = machines
			};

			// Assert
			Assert.That(model.TotalMachinesCount, Is.EqualTo(50));
			Assert.That(model.Machines.Count(), Is.EqualTo(2));
			Assert.That(model.Machines.First().Name, Is.EqualTo("Machine 1"));
			Assert.That(model.Machines.Last().Name, Is.EqualTo("Machine 2"));
		}

		[Test]
		public void MachineServiceModel_AllProperties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new MachineServiceModel
			{
				Id = 123,
				Name = "Test Machine",
				CalibrationSchedule = "01/01/2025",
				IsCalibrated = true,
				Capacity = 18
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(123));
			Assert.That(model.Name, Is.EqualTo("Test Machine"));
			Assert.That(model.CalibrationSchedule, Is.EqualTo("01/01/2025"));
			Assert.That(model.IsCalibrated, Is.True);
			Assert.That(model.Capacity, Is.EqualTo(18));
		}

		[Test]
		public void MachineServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new MachineServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
			Assert.That(model.CalibrationSchedule, Is.EqualTo(string.Empty));
			Assert.That(model.IsCalibrated, Is.False);
			Assert.That(model.Capacity, Is.EqualTo(0));
		}

		[Test]
		public void PaginatedMachinesViewModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PaginatedMachinesViewModel();

			// Assert
			Assert.That(model.Machines, Is.Not.Null);
			Assert.That(model.Machines.Count(), Is.EqualTo(0));
		}

		[Test]
		public void PaginatedMachinesViewModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var machines = new List<MachineServiceModel>
			{
				new MachineServiceModel { Id = 1, Name = "Machine 1" },
				new MachineServiceModel { Id = 2, Name = "Machine 2" }
			};
			var pager = new TeamWorkFlow.Core.Models.Pager.PagerServiceModel(25, 3, 5);

			// Act
			var model = new PaginatedMachinesViewModel
			{
				Machines = machines,
				Pager = pager
			};

			// Assert
			Assert.That(model.Machines.Count(), Is.EqualTo(2));
			Assert.That(model.Pager, Is.Not.Null);
			Assert.That(model.Pager.TotalProjects, Is.EqualTo(25));
			Assert.That(model.Pager.CurrentPage, Is.EqualTo(3));
		}

		[Test]
		public void MachineDeleteServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new MachineDeleteServiceModel
			{
				Id = 10,
				ImageUrl = "https://test.com/machine.jpg",
				Name = "Machine to Delete"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(10));
			Assert.That(model.ImageUrl, Is.EqualTo("https://test.com/machine.jpg"));
			Assert.That(model.Name, Is.EqualTo("Machine to Delete"));
		}

		[Test]
		public void MachineDeleteServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new MachineDeleteServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.ImageUrl, Is.EqualTo(string.Empty));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
		}

		[Test]
		public void MachineDetailsServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new MachineDetailsServiceModel
			{
				Id = 15,
				Name = "Detailed Machine",
				CalibrationSchedule = "15/06/2025",
				IsCalibrated = true,
				Capacity = 12,
				ImageUrl = "https://test.com/detailed.jpg",
				TotalMachineLoad = 75.5,
				MaintenanceScheduleStartDate = "01/07/2025",
				MaintenanceScheduleEndDate = "05/07/2025"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(15));
			Assert.That(model.Name, Is.EqualTo("Detailed Machine"));
			Assert.That(model.CalibrationSchedule, Is.EqualTo("15/06/2025"));
			Assert.That(model.IsCalibrated, Is.True);
			Assert.That(model.Capacity, Is.EqualTo(12));
			Assert.That(model.ImageUrl, Is.EqualTo("https://test.com/detailed.jpg"));
			Assert.That(model.TotalMachineLoad, Is.EqualTo(75.5));
			Assert.That(model.MaintenanceScheduleStartDate, Is.EqualTo("01/07/2025"));
			Assert.That(model.MaintenanceScheduleEndDate, Is.EqualTo("05/07/2025"));
		}

		[Test]
		public void MachineDetailsServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new MachineDetailsServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
			Assert.That(model.CalibrationSchedule, Is.EqualTo(string.Empty));
			Assert.That(model.IsCalibrated, Is.False);
			Assert.That(model.Capacity, Is.EqualTo(0));
			Assert.That(model.ImageUrl, Is.EqualTo(string.Empty));
			Assert.That(model.TotalMachineLoad, Is.EqualTo(0));
			Assert.That(model.MaintenanceScheduleStartDate, Is.Null);
			Assert.That(model.MaintenanceScheduleEndDate, Is.Null);
		}

		[Test]
		public void MachineFormModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new MachineFormModel
			{
				Id = 20,
				Name = "Form Machine",
				CalibrationSchedule = "20/08/2025",
				Capacity = 8,
				IsCalibrated = "true",
				ImageUrl = "https://test.com/form.jpg"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(20));
			Assert.That(model.Name, Is.EqualTo("Form Machine"));
			Assert.That(model.CalibrationSchedule, Is.EqualTo("20/08/2025"));
			Assert.That(model.Capacity, Is.EqualTo(8));
			Assert.That(model.IsCalibrated, Is.EqualTo("true"));
			Assert.That(model.ImageUrl, Is.EqualTo("https://test.com/form.jpg"));
		}

		[Test]
		public void MachineFormModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new MachineFormModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
			Assert.That(model.CalibrationSchedule, Is.EqualTo(string.Empty));
			Assert.That(model.Capacity, Is.EqualTo(0));
			Assert.That(model.IsCalibrated, Is.EqualTo(string.Empty));
			Assert.That(model.ImageUrl, Is.EqualTo(string.Empty));
		}

		#endregion

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
