using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Operator;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
	[TestFixture]
	public class OperatorServiceUnitTests
	{
		private IRepository _repository;
		private IOperatorService _operatorService = null!;
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
            .AddUserSecrets<OperatorServiceUnitTests>()
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
    _operatorService = new OperatorService(_repository,  _mockUserManager.Object);

    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
}

		[Test]
		public async Task GetAllActiveOperatorsAsync_ShouldReturnAllActiveOperators()
		{
			// Arrange
			var operators = await _repository.AllReadOnly<Operator>().ToListAsync();
			Assert.That(operators, Is.Not.Null, "Operator Collection is null");

			// Act	
			var result = await _operatorService.GetAllActiveOperatorsAsync();	

			//Assert
			Assert.That(result, Is.Not.Null, "Result is null");
			Assert.That(result.Count, Is.EqualTo(operators.Count), "Result count is not equal to operators count");
		}

		[Test]
		public async Task GetAllOperatorStatusesAsync_ShouldReturnAllOperatorStatuses()
		{
			// Arrange
			var statuses = await _repository.AllReadOnly<OperatorAvailabilityStatus>().ToListAsync();
			Assert.That(statuses, Is.Not.Null, "Statuses Collection is null");

			// Act	
			var result = await _operatorService.GetAllOperatorStatusesAsync();	

			//Assert
			Assert.That(result, Is.Not.Null, "Result is null");
			Assert.That(result.Count, Is.EqualTo(statuses.Count), "Result count is not equal to statuses count");
		}

		[Test]
		public async Task AddNewOperatorAsync_ShouldAddNewOperator()
		{
			var operatorsCount = await _repository.AllReadOnly<Operator>().CountAsync();
			var userId = "581bb1e2-1024-41fd-aba0-79bcce53551d";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			// Arrange
			var model = new OperatorFormModel()
			{
				FullName = "Test Operator",
				AvailabilityStatusId = 1,
				Capacity = 8,
				Email = "test@gmail.com",
				PhoneNumber = "0888888000",
				IsActive = "true",
				UserId = "581bb1e2-1024-41fd-aba0-79bcce53551d"
			};

			// Act
			await _operatorService.AddNewOperatorAsync(model, userId);

			// Assert
			var operators = await _repository.AllReadOnly<Operator>().ToListAsync();
			Assert.That(operators, Is.Not.Null, "Operators Collection is null");
			Assert.That(operators.Count, Is.EqualTo(operatorsCount + 1), "Operators count is not increased by 1"); 
		}

		[Test]
		public async Task GetOperatorForEditAsync_ShouldReturnOperatorForEdit()
		{
			var operatorClass = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorClass, Is.Not.Null, "Operator Model is null");

			// Arrange
			var result = await _operatorService.GetOperatorForEditAsync(operatorClass.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
		}

		[Test]
		public async Task EditOperatorAsync_ShouldEditOperator()
		{
			var userId = "581bb1e2-1024-41fd-aba0-79bcce53551d";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			// Arrange
			var operatorModel = new OperatorFormModel()
			{
				FullName = "Test Operator",
				AvailabilityStatusId = 1,
				Capacity = 8,
				Email = "a.paytalov@gmail.com",
				PhoneNumber = "0888888000",
				IsActive = "true",
				UserId = userId
			};

			var expectedOperatorCount = await _repository.AllReadOnly<Operator>().CountAsync();
			// Act
			await _operatorService.AddNewOperatorAsync(operatorModel, userId);

			var operatorForEdit = await _repository.AllReadOnly<Operator>()
				.Where(o => o.UserId == "581bb1e2-1024-41fd-aba0-79bcce53551d")
				.FirstOrDefaultAsync();
			Assert.That(operatorForEdit, Is.Not.Null, "Operator Model is null");

			await _operatorService.EditOperatorAsync(operatorModel, operatorForEdit.Id);

			// Assert
			var actualOperatorsCount = await _repository.AllReadOnly<Operator>().CountAsync();
			var editedOperator = await _repository.GetByIdAsync<Operator>(operatorForEdit.Id);
			Assert.That(editedOperator, Is.Not.Null, "Edited Operator is null");
			Assert.That(editedOperator.Email, Is.EqualTo(operatorModel.Email), "Email is not equal to model Email");
			Assert.That(actualOperatorsCount, Is.EqualTo(expectedOperatorCount + 1), "actualOperatorsCount is not equal to expectedOperatorCount");
			Assert.That(editedOperator.FullName, Is.EqualTo(operatorModel.FullName));
			Assert.That(editedOperator.AvailabilityStatusId, Is.EqualTo(operatorModel.AvailabilityStatusId));
			Assert.That(editedOperator.Capacity, Is.EqualTo(operatorModel.Capacity));
			Assert.That(editedOperator.PhoneNumber, Is.EqualTo(operatorModel.PhoneNumber));
			Assert.That(editedOperator.IsActive, Is.EqualTo(true));
			Assert.That(editedOperator.UserId, Is.EqualTo(operatorForEdit.UserId));
		}

		[Test]
		public async Task OperatorStatusExistAsync_ShouldReturnTrue()
		{
			// Arrange
			var operatorStatus = await _repository.AllReadOnly<OperatorAvailabilityStatus>().FirstOrDefaultAsync();
			Assert.That(operatorStatus, Is.Not.Null, "Operator Status is null");

			// Act
			var result = await _operatorService.OperatorStatusExistAsync(operatorStatus.Id);

			// Assert
			Assert.That(result, Is.True, "Result is not true");
		}

		[Test]
		public async Task OperatorExistByIdAsync_ShouldReturnTrue()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			var result = await _operatorService.OperatorExistByIdAsync(operatorModel.Id);

			// Assert
			Assert.That(result, Is.True, "Result is not true");
		}

		[Test]
		public async Task GetOperatorDetailsByIdAsync_ShouldReturnOperatorDetails()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			var result = await _operatorService.GetOperatorDetailsByIdAsync(operatorModel.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
		}

		[Test]
		public async Task GetAllCompletedTasksAssignedToOperatorByIdAsync_ShouldReturnTotalCompletedTasks()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			var result = await _operatorService.GetAllCompletedTasksAssignedToOperatorByIdAsync(operatorModel.Id);

			// Assert
			Assert.That(result, Is.GreaterThanOrEqualTo(0), "Result is less than 0");
		}

		[Test]
		public async Task GetAllActiveAssignedTaskToOperatorByIdAsync_ShouldReturnTotalActiveAssignedTasks()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			var result = await _operatorService.GetAllActiveAssignedTaskToOperatorByIdAsync(operatorModel.Id);

			// Assert
			Assert.That(result, Is.GreaterThanOrEqualTo(0), "Result is less than 0");
		}

		[Test]
		public async Task GetOperatorModelForDeleteByIdAsync_ShouldReturnOperatorModelForDelete()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			var result = await _operatorService.GetOperatorModelForDeleteByIdAsync(operatorModel.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
		}

		[Test]
		public async Task DeleteOperatorByIdAsync_ShouldDeleteOperator()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			await _operatorService.DeleteOperatorByIdAsync(operatorModel.Id);

			// Assert
			var deletedOperator = await _repository.GetByIdAsync<Operator>(operatorModel.Id);
			Assert.That(deletedOperator, Is.Null, "Deleted Operator is not null");
		}

		[Test]
		public async Task GetAllOperatorsAsync_ShouldReturnAllOperators()
		{
			// Arrange
			var operators = await _repository.AllReadOnly<Operator>().ToListAsync();
			Assert.That(operators, Is.Not.Null, "Operators Collection is null");

			// Act
			var result = await _operatorService.GetAllOperatorsAsync();

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
			Assert.That(result.Count, Is.EqualTo(operators.Count), "Result count is not equal to operators count");
		}

		[Test]
		public async Task GetAllUnActiveOperatorsAsync_ShouldReturnAllUnActiveOperators()
		{
			// Arrange
			var operators = await _repository.AllReadOnly<Operator>().Where(o => o.IsActive == false).ToListAsync();
			Assert.That(operators, Is.Not.Null, "Operators Collection is null");

			// Act
			var result = await _operatorService.GetAllUnActiveOperatorsAsync();

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
			Assert.That(result.Count, Is.EqualTo(operators.Count), "Result count is not equal to operators count");
		}

		[Test]
		public async Task ActivateOperatorAsync_ShouldActivateOperator()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			await _operatorService.ActivateOperatorAsync(operatorModel.Id);

			// Assert
			var activatedOperator = await _repository.GetByIdAsync<Operator>(operatorModel.Id);
			Assert.That(activatedOperator, Is.Not.Null, "Activated Operator is null");
			Assert.That(activatedOperator.IsActive, Is.EqualTo(true), "Activated Operator is not active");
		}

		[Test]
		public async Task GetUserIdByEmailAsync_ShouldReturnUserId()
		{
			// Arrange
			_mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((string email) =>
				{
					// Simulate finding the user with the given email address
					return new IdentityUser { Id = "userId", Email = email };
				});

			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null, "Operator Model is null");

			// Act
			var result = await _operatorService.GetUserIdByEmailAsync(operatorModel.Email);

			// Assert
			Assert.That(result, Is.Not.Null, "Result is null");
		}

		// ============================================================================
		// ADDITIONAL TESTS FOR 90%+ COVERAGE
		// ============================================================================

		#region AllAsync Method Tests (Query and Search Functionality)

		[Test]
		public async Task AllAsync_WithNoParameters_ReturnsAllActiveOperators()
		{
			// Arrange
			var expectedOperatorCount = await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive)
				.CountAsync();

			// Act
			var result = await _operatorService.AllAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalOperatorsCount, Is.EqualTo(expectedOperatorCount));
			Assert.That(result.Operators, Is.Not.Null);
			Assert.That(result.Operators.Count(), Is.LessThanOrEqualTo(10)); // Default page size
		}

		[Test]
		public async Task AllAsync_WithSearchParameter_ReturnsFilteredOperators()
		{
			// Arrange
			string searchTerm = "Aleksandar"; // Search for operators containing "Aleksandar"

			// Act
			var result = await _operatorService.AllAsync(search: searchTerm);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
			// Verify that returned operators contain the search term
			foreach (var operatorModel in result.Operators)
			{
				bool containsSearchTerm = operatorModel.FullName.ToLower().Contains(searchTerm.ToLower()) ||
										 operatorModel.Email.ToLower().Contains(searchTerm.ToLower()) ||
										 operatorModel.PhoneNumber.ToLower().Contains(searchTerm.ToLower());
				Assert.That(containsSearchTerm, Is.True, $"Operator {operatorModel.FullName} should contain search term {searchTerm}");
			}
		}

		[Test]
		public async Task AllAsync_WithEmptySearch_ReturnsAllActiveOperators()
		{
			// Arrange
			var expectedOperatorCount = await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive)
				.CountAsync();

			// Act
			var result = await _operatorService.AllAsync(search: "");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalOperatorsCount, Is.EqualTo(expectedOperatorCount));
		}

		[Test]
		public async Task AllAsync_WithNonExistentSearch_ReturnsEmptyResult()
		{
			// Arrange
			string nonExistentSearch = "NonExistentOperatorName12345";

			// Act
			var result = await _operatorService.AllAsync(search: nonExistentSearch);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators.Count(), Is.EqualTo(0));
			Assert.That(result.TotalOperatorsCount, Is.EqualTo(0));
		}

		[Test]
		public async Task AllAsync_WithSortingByNameAscending_ReturnsSortedOperators()
		{
			// Arrange & Act
			var result = await _operatorService.AllAsync(sorting: OperatorSorting.NameAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
			if (result.Operators.Count() > 1)
			{
				var operatorNames = result.Operators.Select(o => o.FullName).ToList();
				var sortedNames = operatorNames.OrderBy(n => n).ToList();
				Assert.That(operatorNames, Is.EqualTo(sortedNames), "Operators should be sorted by name ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByNameDescending_ReturnsSortedOperators()
		{
			// Arrange & Act
			var result = await _operatorService.AllAsync(sorting: OperatorSorting.NameDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
			if (result.Operators.Count() > 1)
			{
				var operatorNames = result.Operators.Select(o => o.FullName).ToList();
				var sortedNames = operatorNames.OrderByDescending(n => n).ToList();
				Assert.That(operatorNames, Is.EqualTo(sortedNames), "Operators should be sorted by name descending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByEmailAscending_ReturnsSortedOperators()
		{
			// Arrange & Act
			var result = await _operatorService.AllAsync(sorting: OperatorSorting.EmailAscending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
			if (result.Operators.Count() > 1)
			{
				var operatorEmails = result.Operators.Select(o => o.Email).ToList();
				var sortedEmails = operatorEmails.OrderBy(e => e).ToList();
				Assert.That(operatorEmails, Is.EqualTo(sortedEmails), "Operators should be sorted by email ascending");
			}
		}

		[Test]
		public async Task AllAsync_WithSortingByCapacityDescending_ReturnsSortedOperators()
		{
			// Arrange & Act
			var result = await _operatorService.AllAsync(sorting: OperatorSorting.CapacityDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
			if (result.Operators.Count() > 1)
			{
				var operatorCapacities = result.Operators.Select(o => o.Capacity).ToList();
				var sortedCapacities = operatorCapacities.OrderByDescending(c => c).ToList();
				Assert.That(operatorCapacities, Is.EqualTo(sortedCapacities), "Operators should be sorted by capacity descending");
			}
		}

		[Test]
		public async Task AllAsync_WithCustomPagination_ReturnsCorrectPage()
		{
			// Arrange
			int operatorsPerPage = 2;
			int currentPage = 1;

			// Act
			var result = await _operatorService.AllAsync(operatorsPerPage: operatorsPerPage, currentPage: currentPage);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
			Assert.That(result.Operators.Count(), Is.LessThanOrEqualTo(operatorsPerPage));
		}

		#endregion

		#region Error Handling and Edge Cases

		[Test]
		public async Task OperatorStatusExistAsync_WithInvalidId_ReturnsFalse()
		{
			// Arrange
			int invalidStatusId = 99999;

			// Act
			var result = await _operatorService.OperatorStatusExistAsync(invalidStatusId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task OperatorExistByIdAsync_WithInvalidId_ReturnsFalse()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act
			var result = await _operatorService.OperatorExistByIdAsync(invalidOperatorId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetOperatorDetailsByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act
			var result = await _operatorService.GetOperatorDetailsByIdAsync(invalidOperatorId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetOperatorForEditAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act
			var result = await _operatorService.GetOperatorForEditAsync(invalidOperatorId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetOperatorModelForDeleteByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act
			var result = await _operatorService.GetOperatorModelForDeleteByIdAsync(invalidOperatorId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void DeleteOperatorByIdAsync_WithInvalidId_DoesNotThrowException()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act & Assert
			Assert.DoesNotThrowAsync(async () => await _operatorService.DeleteOperatorByIdAsync(invalidOperatorId));
		}

		[Test]
		public async Task GetAllCompletedTasksAssignedToOperatorByIdAsync_WithInvalidId_ReturnsZero()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act
			var result = await _operatorService.GetAllCompletedTasksAssignedToOperatorByIdAsync(invalidOperatorId);

			// Assert
			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task GetAllActiveAssignedTaskToOperatorByIdAsync_WithInvalidId_ReturnsZero()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act
			var result = await _operatorService.GetAllActiveAssignedTaskToOperatorByIdAsync(invalidOperatorId);

			// Assert
			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task AddNewOperatorAsync_WithInvalidIsActiveValue_ThrowsArgumentException()
		{
			// Arrange
			var userId = "581bb1e2-1024-41fd-aba0-79bcce53551d";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var model = new OperatorFormModel()
			{
				FullName = "Test Operator",
				AvailabilityStatusId = 1,
				Capacity = 8,
				Email = "test@gmail.com",
				PhoneNumber = "0888888000",
				IsActive = "invalid_boolean_value", // Invalid boolean value
				UserId = userId
			};

			// Act & Assert
			var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
				await _operatorService.AddNewOperatorAsync(model, userId));
			Assert.That(ex.Message, Contains.Substring("Incorrect input. It must be true or false."));
		}

		[Test]
		public async Task EditOperatorAsync_WithInvalidIsActiveValue_ThrowsArgumentException()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null);

			var model = new OperatorFormModel()
			{
				FullName = "Updated Operator",
				AvailabilityStatusId = 1,
				Capacity = 8,
				Email = "updated@gmail.com",
				PhoneNumber = "0888888000",
				IsActive = "invalid_boolean_value" // Invalid boolean value
			};

			// Act & Assert
			var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
				await _operatorService.EditOperatorAsync(model, operatorModel.Id));
			Assert.That(ex.Message, Contains.Substring("Incorrect input. It must be true or false."));
		}

		[Test]
		public void EditOperatorAsync_WithInvalidOperatorId_DoesNotThrowException()
		{
			// Arrange
			int invalidOperatorId = 99999;
			var model = new OperatorFormModel()
			{
				FullName = "Test Operator",
				AvailabilityStatusId = 1,
				Capacity = 8,
				Email = "test@gmail.com",
				PhoneNumber = "0888888000",
				IsActive = "true"
			};

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
				await _operatorService.EditOperatorAsync(model, invalidOperatorId));
		}

		#endregion

		#region Additional Method Coverage Tests

		[Test]
		public async Task AllAsync_WithAllSortingOptions_ExecutesSuccessfully()
		{
			// Test all sorting enum values to ensure complete coverage
			var sortingOptions = Enum.GetValues<OperatorSorting>();

			foreach (var sorting in sortingOptions)
			{
				// Act
				var result = await _operatorService.AllAsync(sorting: sorting);

				// Assert
				Assert.That(result, Is.Not.Null, $"Result should not be null for sorting: {sorting}");
				Assert.That(result.Operators, Is.Not.Null, $"Operators should not be null for sorting: {sorting}");
			}
		}

		[Test]
		public async Task AllAsync_WithNullSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _operatorService.AllAsync(search: null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithWhitespaceSearch_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _operatorService.AllAsync(search: "   ");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Operators, Is.Not.Null);
		}

		[Test]
		public async Task GetAllActiveOperatorsAsync_WithPagination_ReturnsCorrectPage()
		{
			// Arrange
			int page = 1;
			int pageSize = 2;
			var totalActiveOperators = await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive)
				.CountAsync();

			// Act
			var result = await _operatorService.GetAllActiveOperatorsAsync(page, pageSize);

			// Assert
			Assert.That(result.Operators, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalActiveOperators));
			Assert.That(result.Operators.Count(), Is.LessThanOrEqualTo(pageSize));
		}

		[Test]
		public async Task GetAllActiveOperatorsAsync_WithSecondPage_ReturnsCorrectOperators()
		{
			// Arrange
			int page = 2;
			int pageSize = 1;
			var totalActiveOperators = await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive)
				.CountAsync();

			// Act
			var result = await _operatorService.GetAllActiveOperatorsAsync(page, pageSize);

			// Assert
			Assert.That(result.Operators, Is.Not.Null);
			Assert.That(result.TotalCount, Is.EqualTo(totalActiveOperators));
			if (totalActiveOperators > 1)
			{
				Assert.That(result.Operators.Count(), Is.LessThanOrEqualTo(1));
			}
		}

		[Test]
		public async Task ActivateOperatorAsync_WithInactiveOperator_ActivatesOperator()
		{
			// Arrange - First create an inactive operator
			var userId = "test-inactive-user-id";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var inactiveOperator = new Operator()
			{
				FullName = "Inactive Test Operator",
				Email = "inactive@test.com",
				PhoneNumber = "0888888999",
				AvailabilityStatusId = 1,
				Capacity = 8,
				IsActive = false,
				UserId = userId
			};

			await _repository.AddAsync(inactiveOperator);
			await _repository.SaveChangesAsync();

			// Act
			await _operatorService.ActivateOperatorAsync(inactiveOperator.Id);

			// Assert
			var activatedOperator = await _repository.GetByIdAsync<Operator>(inactiveOperator.Id);
			Assert.That(activatedOperator, Is.Not.Null);
			Assert.That(activatedOperator.IsActive, Is.True);
			Assert.That(activatedOperator.AvailabilityStatusId, Is.EqualTo(1)); // Should be set to "at work"
		}

		[Test]
		public async Task ActivateOperatorAsync_WithAlreadyActiveOperator_DoesNotChangeStatus()
		{
			// Arrange
			var activeOperator = await _repository.AllReadOnly<Operator>()
				.Where(o => o.IsActive)
				.FirstOrDefaultAsync();
			Assert.That(activeOperator, Is.Not.Null);

			// Act
			await _operatorService.ActivateOperatorAsync(activeOperator.Id);

			// Assert
			var operatorAfterActivation = await _repository.GetByIdAsync<Operator>(activeOperator.Id);
			Assert.That(operatorAfterActivation, Is.Not.Null);
			Assert.That(operatorAfterActivation.IsActive, Is.True);
		}

		[Test]
		public async Task DeactivateOperatorAsync_WithActiveOperator_DeactivatesOperator()
		{
			// Arrange - First create an active operator
			var userId = "test-deactivate-user-id";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var activeOperator = new Operator()
			{
				FullName = "Active Test Operator",
				Email = "active@test.com",
				PhoneNumber = "0888777666",
				AvailabilityStatusId = 1, // "at work"
				Capacity = 8,
				IsActive = true,
				UserId = userId
			};

			await _repository.AddAsync(activeOperator);
			await _repository.SaveChangesAsync();

			// Act
			await _operatorService.DeactivateOperatorAsync(activeOperator.Id);

			// Assert
			var deactivatedOperator = await _repository.GetByIdAsync<Operator>(activeOperator.Id);
			Assert.That(deactivatedOperator, Is.Not.Null);
			Assert.That(deactivatedOperator.IsActive, Is.False);
		}

		[Test]
		public async Task DeactivateOperatorAsync_WithInactiveOperator_DoesNotChangeStatus()
		{
			// Arrange - First create an inactive operator
			var userId = "test-inactive-deactivate-user-id";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var inactiveOperator = new Operator()
			{
				FullName = "Inactive Test Operator",
				Email = "inactive@test.com",
				PhoneNumber = "0888555444",
				AvailabilityStatusId = 2, // "on vacation"
				Capacity = 8,
				IsActive = false,
				UserId = userId
			};

			await _repository.AddAsync(inactiveOperator);
			await _repository.SaveChangesAsync();

			// Act
			await _operatorService.DeactivateOperatorAsync(inactiveOperator.Id);

			// Assert
			var operatorAfterDeactivation = await _repository.GetByIdAsync<Operator>(inactiveOperator.Id);
			Assert.That(operatorAfterDeactivation, Is.Not.Null);
			Assert.That(operatorAfterDeactivation.IsActive, Is.False);
		}

		[Test]
		public async Task DeactivateOperatorWithStatusAsync_WithValidStatus_DeactivatesOperatorAndSetsStatus()
		{
			// Arrange - First create an active operator
			var userId = "test-deactivate-status-user-id";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var activeOperator = new Operator()
			{
				FullName = "Active Status Test Operator",
				Email = "activestatus@test.com",
				PhoneNumber = "0888333222",
				AvailabilityStatusId = 1, // "at work"
				Capacity = 8,
				IsActive = true,
				UserId = userId
			};

			await _repository.AddAsync(activeOperator);
			await _repository.SaveChangesAsync();

			int newStatusId = 2; // "on vacation" status

			// Act
			await _operatorService.DeactivateOperatorWithStatusAsync(activeOperator.Id, newStatusId);

			// Assert
			var deactivatedOperator = await _repository.GetByIdAsync<Operator>(activeOperator.Id);
			Assert.That(deactivatedOperator, Is.Not.Null);
			Assert.That(deactivatedOperator.IsActive, Is.False);
			Assert.That(deactivatedOperator.AvailabilityStatusId, Is.EqualTo(newStatusId));
		}

		[Test]
		public async Task DeactivateOperatorWithStatusAsync_WithAtWorkStatus_ThrowsInvalidOperationException()
		{
			// Arrange - First create an active operator
			var userId = "test-invalid-status-user-id";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var activeOperator = new Operator()
			{
				FullName = "Invalid Status Test Operator",
				Email = "invalidstatus@test.com",
				PhoneNumber = "0888111000",
				AvailabilityStatusId = 1, // "at work"
				Capacity = 8,
				IsActive = true,
				UserId = userId
			};

			await _repository.AddAsync(activeOperator);
			await _repository.SaveChangesAsync();

			int atWorkStatusId = 1; // "at work" status - should not be allowed for deactivation

			// Act & Assert
			var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
				await _operatorService.DeactivateOperatorWithStatusAsync(activeOperator.Id, atWorkStatusId));
			Assert.That(ex.Message, Contains.Substring("Cannot deactivate operator with 'at work' status"));
		}

		[Test]
		public async Task DeactivateOperatorWithStatusAsync_WithInvalidStatus_ThrowsArgumentException()
		{
			// Arrange - First create an active operator
			var userId = "test-invalid-status-id-user-id";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var activeOperator = new Operator()
			{
				FullName = "Invalid Status ID Test Operator",
				Email = "invalidstatusid@test.com",
				PhoneNumber = "0888999888",
				AvailabilityStatusId = 1, // "at work"
				Capacity = 8,
				IsActive = true,
				UserId = userId
			};

			await _repository.AddAsync(activeOperator);
			await _repository.SaveChangesAsync();

			int invalidStatusId = 99999; // Non-existent status

			// Act & Assert
			var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
				await _operatorService.DeactivateOperatorWithStatusAsync(activeOperator.Id, invalidStatusId));
			Assert.That(ex.Message, Contains.Substring("Invalid availability status selected"));
		}

		[Test]
		public async Task DeactivateOperatorWithStatusAsync_WithInactiveOperator_DoesNotChangeOperator()
		{
			// Arrange - First create an inactive operator
			var userId = "test-inactive-status-user-id";
			_dbContext.Users.Add(new IdentityUser { Id = userId });
			await _dbContext.SaveChangesAsync();

			var inactiveOperator = new Operator()
			{
				FullName = "Inactive Status Test Operator",
				Email = "inactivestatus@test.com",
				PhoneNumber = "0888777555",
				AvailabilityStatusId = 2, // "on vacation"
				Capacity = 8,
				IsActive = false,
				UserId = userId
			};

			await _repository.AddAsync(inactiveOperator);
			await _repository.SaveChangesAsync();

			int originalStatusId = inactiveOperator.AvailabilityStatusId;
			int newStatusId = 3; // "sick leave" status

			// Act
			await _operatorService.DeactivateOperatorWithStatusAsync(inactiveOperator.Id, newStatusId);

			// Assert - Should not change anything since operator is already inactive
			var operatorAfterDeactivation = await _repository.GetByIdAsync<Operator>(inactiveOperator.Id);
			Assert.That(operatorAfterDeactivation, Is.Not.Null);
			Assert.That(operatorAfterDeactivation.IsActive, Is.False);
			Assert.That(operatorAfterDeactivation.AvailabilityStatusId, Is.EqualTo(originalStatusId)); // Should remain unchanged
		}

		[Test]
		public void ActivateOperatorAsync_WithInvalidId_DoesNotThrowException()
		{
			// Arrange
			int invalidOperatorId = 99999;

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
				await _operatorService.ActivateOperatorAsync(invalidOperatorId));
		}

		[Test]
		public async Task GetUserIdByEmailAsync_WithNonExistentEmail_ReturnsNull()
		{
			// Arrange
			_mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))!
				.ReturnsAsync((IdentityUser?)null);

			// Act
			var result = await _operatorService.GetUserIdByEmailAsync("nonexistent@test.com");

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetOperatorFullNameByUserIdAsync_WithValidUserId_ReturnsFullName()
		{
			// Arrange
			var operatorModel = await _repository.AllReadOnly<Operator>().FirstOrDefaultAsync();
			Assert.That(operatorModel, Is.Not.Null);

			// Act
			var result = await _operatorService.GetOperatorFullNameByUserIdAsync(operatorModel.UserId);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.EqualTo(operatorModel.FullName));
		}

		[Test]
		public async Task GetOperatorFullNameByUserIdAsync_WithInvalidUserId_ReturnsNull()
		{
			// Arrange
			string invalidUserId = "invalid-user-id-12345";

			// Act
			var result = await _operatorService.GetOperatorFullNameByUserIdAsync(invalidUserId);

			// Assert
			Assert.That(result, Is.Null);
		}

		#endregion

		#region Model Coverage Tests

		[Test]
		public void AllOperatorsQueryModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new AllOperatorsQueryModel();

			// Assert
			Assert.That(model.OperatorsPerPage, Is.EqualTo(10));
			Assert.That(model.CurrentPage, Is.EqualTo(1));
			Assert.That(model.Sorting, Is.EqualTo(OperatorSorting.LastAdded));
			Assert.That(model.Operators, Is.Not.Null);
			Assert.That(model.Operators.Count(), Is.EqualTo(0));
		}

		[Test]
		public void AllOperatorsQueryModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var operators = new List<OperatorServiceModel>
			{
				new OperatorServiceModel { Id = 1, FullName = "Test Operator 1" },
				new OperatorServiceModel { Id = 2, FullName = "Test Operator 2" }
			};

			// Act
			var model = new AllOperatorsQueryModel
			{
				Search = "test search",
				Sorting = OperatorSorting.NameAscending,
				CurrentPage = 2,
				TotalOperatorsCount = 50,
				Operators = operators
			};

			// Assert
			Assert.That(model.Search, Is.EqualTo("test search"));
			Assert.That(model.Sorting, Is.EqualTo(OperatorSorting.NameAscending));
			Assert.That(model.CurrentPage, Is.EqualTo(2));
			Assert.That(model.TotalOperatorsCount, Is.EqualTo(50));
			Assert.That(model.Operators.Count(), Is.EqualTo(2));
			Assert.That(model.Operators.First().FullName, Is.EqualTo("Test Operator 1"));
		}

		[Test]
		public void PaginatedOperatorsViewModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PaginatedOperatorsViewModel();

			// Assert
			Assert.That(model.Operators, Is.Not.Null);
			Assert.That(model.Operators.Count(), Is.EqualTo(0));
		}

		[Test]
		public void PaginatedOperatorsViewModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var operators = new List<OperatorServiceModel>
			{
				new OperatorServiceModel { Id = 1, FullName = "Operator 1" },
				new OperatorServiceModel { Id = 2, FullName = "Operator 2" }
			};
			var pager = new TeamWorkFlow.Core.Models.Pager.PagerServiceModel(20, 2, 5);

			// Act
			var model = new PaginatedOperatorsViewModel
			{
				Operators = operators,
				Pager = pager
			};

			// Assert
			Assert.That(model.Operators.Count(), Is.EqualTo(2));
			Assert.That(model.Pager, Is.Not.Null);
			Assert.That(model.Pager.TotalProjects, Is.EqualTo(20));
			Assert.That(model.Pager.CurrentPage, Is.EqualTo(2));
		}

		[Test]
		public void OperatorServiceModel_AllProperties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new OperatorServiceModel
			{
				Id = 123,
				FullName = "Test Operator Name",
				AvailabilityStatus = "at work",
				Email = "test@operator.com",
				PhoneNumber = "0888123456",
				IsActive = true,
				Capacity = 8
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(123));
			Assert.That(model.FullName, Is.EqualTo("Test Operator Name"));
			Assert.That(model.AvailabilityStatus, Is.EqualTo("at work"));
			Assert.That(model.Email, Is.EqualTo("test@operator.com"));
			Assert.That(model.PhoneNumber, Is.EqualTo("0888123456"));
			Assert.That(model.IsActive, Is.True);
			Assert.That(model.Capacity, Is.EqualTo(8));
		}

		[Test]
		public void OperatorServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new OperatorServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.FullName, Is.EqualTo(string.Empty));
			Assert.That(model.AvailabilityStatus, Is.EqualTo(string.Empty));
			// Email is declared as = null! so it will be null by default
			Assert.That(model.PhoneNumber, Is.EqualTo(string.Empty));
			Assert.That(model.IsActive, Is.False);
			Assert.That(model.Capacity, Is.EqualTo(0));
		}

		[Test]
		public void OperatorQueryServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new OperatorQueryServiceModel();

			// Assert
			Assert.That(model.Operators, Is.Not.Null);
			Assert.That(model.Operators.Count(), Is.EqualTo(0));
			Assert.That(model.TotalOperatorsCount, Is.EqualTo(0));
		}

		[Test]
		public void OperatorQueryServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var operators = new List<OperatorServiceModel>
			{
				new OperatorServiceModel { Id = 1, FullName = "Query Operator 1" },
				new OperatorServiceModel { Id = 2, FullName = "Query Operator 2" },
				new OperatorServiceModel { Id = 3, FullName = "Query Operator 3" }
			};

			// Act
			var model = new OperatorQueryServiceModel
			{
				TotalOperatorsCount = 100,
				Operators = operators
			};

			// Assert
			Assert.That(model.TotalOperatorsCount, Is.EqualTo(100));
			Assert.That(model.Operators.Count(), Is.EqualTo(3));
			Assert.That(model.Operators.First().FullName, Is.EqualTo("Query Operator 1"));
			Assert.That(model.Operators.Last().FullName, Is.EqualTo("Query Operator 3"));
		}

		#endregion

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
