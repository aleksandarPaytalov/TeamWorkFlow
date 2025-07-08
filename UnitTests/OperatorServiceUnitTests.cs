using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
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
		private IOperatorService _operatorService;
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

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
