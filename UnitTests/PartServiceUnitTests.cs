using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Part;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
	[TestFixture]
	public class PartServiceUnitTests
	{
		private IRepository _repository;
		private IPartService _partService;
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
				.AddUserSecrets<PartServiceUnitTests>()
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
		_partService = new PartService(_repository);

		_dbContext.Database.EnsureDeleted();
		_dbContext.Database.EnsureCreated();
	}

		[Test]
		public async Task PartExistAsync_WithExistingPart_ShouldReturnTrue()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.PartExistAsync(part.Id);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task PartExistAsync_WithNotExistingPart_ShouldReturnFalse()
		{
			// Arrange
			var partsNumber = await _repository.AllReadOnly<Part>().CountAsync();
			Assert.That(partsNumber, Is.GreaterThan(0), "Parts number is 0");

			// Act
			var result = await _partService.PartExistAsync(partsNumber + 1);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public async Task AllAsync_WithExistingPart_ShouldReturnPartQueryServiceModel()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync();

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");
		}

		[Test]
		public async Task AllAsync_WithNotExistingPartAndStatus_ShouldReturnEmptyPartQueryServiceModel()
		{
			// Arrange
			var partsNumber = await _repository.AllReadOnly<Part>().CountAsync();
			Assert.That(partsNumber, Is.GreaterThan(0), "Parts number is 0");

			// Act
			var result = await _partService.AllAsync(status: "NotExistingStatus");

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.EqualTo(0), "Parts count is not 0");
		}

		[Test]
		public async Task AllAsync_WithExistingPartAndSearch_ShouldReturnPartQueryServiceModel()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync(searchByName: part.Name);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");
		}

		[Test]
		public async Task AllAsync_WithNotExistingPartAndSearch_ShouldReturnEmptyPartQueryServiceModel()
		{
			// Arrange
			var partsNumber = await _repository.AllReadOnly<Part>().CountAsync();
			Assert.That(partsNumber, Is.GreaterThan(0), "Parts number is 0");

			// Act
			var result = await _partService.AllAsync(searchByName: "NotExistingName");

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.EqualTo(0), "Parts count is not 0");
		}

		[Test]
		public async Task AllAsync_WithExistingPartAndSorting_ShouldReturnPartQueryServiceModel()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync(sorting: PartSorting.ProjectNumberAscending);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");
		}
		
		[Test]
		public async Task AllAsync_WithExistingPartAndPartsPerPage_ShouldReturnPartQueryServiceModel()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync(partsPerPage: 2);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");
		}

		[Test]
		public async Task AllAsync_WithExistingPartAndCurrentPage_ShouldReturnPartQueryServiceModel()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync(currentPage: 2);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");
		}

		[Test]
		public async Task
			AllAsync_WithNotExistingPartAndStatusAndSearchAndSortingAndPartsPerPageAndCurrentPage_ShouldReturnEmptyPartQueryServiceModel()
		{
			// Arrange
			var partsNumber = await _repository.AllReadOnly<Part>().CountAsync();
			Assert.That(partsNumber, Is.GreaterThan(0), "Parts number is 0");

			// Act
			var result = await _partService.AllAsync(status: "NotExistingStatus", 
				searchByName: "NotExistingName", 
				sorting: PartSorting.ProjectNumberAscending, 
				partsPerPage: 2, 
				currentPage: 2);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.EqualTo(0), "Parts count is not 0");
		}

		[Test]
		public async Task AllStatusNamesAsync()
		{
			// Arrange
			var statusNames = await _partService.AllStatusNamesAsync();

			// Assert
			Assert.That(statusNames, Is.Not.Null, "Status names is null");
			Assert.That(statusNames.Count, Is.GreaterThan(0), "Status names count is 0");
		}

		[Test]
		public async Task AllStatusesAsync()
		{
			// Arrange
			var statuses = await _partService.AllStatusesAsync();

			// Assert
			Assert.That(statuses, Is.Not.Null, "Statuses is null");
			Assert.That(statuses.Count, Is.GreaterThan(0), "Statuses count is 0");
		}

		[Test]
		public async Task PartDetailsByIdAsync()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.PartDetailsByIdAsync(part.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "PartDetails is null");
			Assert.That(result.Id, Is.EqualTo(part.Id), "PartDetails Id is not equal to part Id");
		}

		[Test]
		public async Task GetPartFormModelForEditAsync()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.GetPartFormModelForEditAsync(part.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "PartFormModel is null");
		}

		[Test]
		public async Task GetPartForDeletingByIdAsync()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.GetPartForDeletingByIdAsync(part.Id);

			// Assert
			Assert.That(result, Is.Not.Null, "PartForDeleting is null");
			Assert.That(result.Id, Is.EqualTo(part.Id), "PartForDeleting Id is not equal to part Id");
		}

		[Test]
		public async Task DeletePartByIdAsync()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			var partsNumber = await _repository.AllReadOnly<Part>().CountAsync();

			// Act
			await _partService.DeletePartByIdAsync(part.Id);

			// Assert
			var partAfterDelete = await _repository.AllReadOnly<Part>().CountAsync();
			Assert.That(partAfterDelete, Is.EqualTo(partsNumber - 1), "Part is not deleted");
		}

		[Test]
		public async Task EditAsync_ShouldEditAPart()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			var partFormModel = new PartFormModel()
			{
				Name = "NewName",
				ImageUrl = "NewImageUrl",
				PartArticleNumber = "NewPartArticleNumber",
				PartClientNumber = "NewPartClientNumber",
				PartModel = "NewPartModel",
				ToolNumber = 2750,
				PartStatusId = part.PartStatusId
			};

			// Act
			await _partService.EditAsync(part.Id, partFormModel, part.ProjectId, part.PartStatusId);

			// Assert
			var partAfterEdit = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(partAfterEdit, Is.Not.Null, "Part is null");
			Assert.That(partAfterEdit.Name, Is.EqualTo(partFormModel.Name), "Part name is not equal to new name");
			Assert.That(partAfterEdit.ImageUrl, Is.EqualTo(partFormModel.ImageUrl), "Part image url is not equal to new image url");
			Assert.That(partAfterEdit.PartArticleNumber, Is.EqualTo(partFormModel.PartArticleNumber), "Part article number is not equal to new article number");
			Assert.That(partAfterEdit.PartClientNumber, Is.EqualTo(partFormModel.PartClientNumber), "Part client number is not equal to new client number");
			Assert.That(partAfterEdit.PartModel, Is.EqualTo(partFormModel.PartModel), "Part model is not equal to new model");
			Assert.That(partAfterEdit.ToolNumber, Is.EqualTo(partFormModel.ToolNumber), "Part tool number is not equal to new tool number");
		}

		[Test]
		public async Task EditAsync_ShouldNotEditAPart()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			var partFormModel = new PartFormModel()
			{
				Name = "NewName",
				ImageUrl = "NewImageUrl",
				PartArticleNumber = "NewPartArticleNumber",
				PartClientNumber = "NewPartClientNumber",
				PartModel = "NewPartModel",
				ToolNumber = 2750,
				PartStatusId = part.PartStatusId
			};

			// Act
			await _partService.EditAsync(part.Id, partFormModel, part.ProjectId, part.PartStatusId);

			// Assert
			var partAfterEdit = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(partAfterEdit, Is.Not.Null, "Part is null");
			Assert.That(partAfterEdit.Name, Is.EqualTo(partFormModel.Name), "Part name is not equal to new name");
			Assert.That(partAfterEdit.ImageUrl, Is.EqualTo(partFormModel.ImageUrl), "Part image url is not equal to new image url");
			Assert.That(partAfterEdit.PartArticleNumber, Is.EqualTo(partFormModel.PartArticleNumber), "Part article number is not equal to new article number");
			Assert.That(partAfterEdit.PartClientNumber, Is.EqualTo(partFormModel.PartClientNumber), "Part client number is not equal to new client number");
			Assert.That(partAfterEdit.PartModel, Is.EqualTo(partFormModel.PartModel), "Part model is not equal to new model");
			Assert.That(partAfterEdit.ToolNumber, Is.EqualTo(partFormModel.ToolNumber), "Part tool number is not equal to new tool number");
		}

		[Test]
		public async Task AddNewPartAsync_ShouldAddPartToCollection()
		{
			var partsBeforeAdd = await _repository.AllReadOnly<Part>().CountAsync();
			// Arrange

			var partFormModel = new PartFormModel()
			{
				Name = "NewName",
				ImageUrl = "NewImageUrl",
				PartArticleNumber = "2.4.159.999",
				PartClientNumber = "22.168-20",
				PartModel = "NewPartModel",
				ToolNumber = 2750,
				PartStatusId = 1,
				ProjectNumber = "249100"
			};

			// Act
			await _partService.AddNewPartAsync(partFormModel, 1);
			var partsAfterAdd = await _repository.AllReadOnly<Part>().CountAsync();

			// Assert
			Assert.That(partsAfterAdd, Is.EqualTo(partsBeforeAdd + 1), "Part is not added");
			
		}
		


		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
