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

		// ============================================================================
		// ADDITIONAL TESTS FOR 95%+ COVERAGE
		// ============================================================================

		#region Missing Method Coverage Tests

		[Test]
		public async Task StatusExistAsync_WithExistingStatus_ShouldReturnTrue()
		{
			// Arrange
			var status = await _repository.AllReadOnly<PartStatus>().FirstOrDefaultAsync();
			Assert.That(status, Is.Not.Null, "Status is null");

			// Act
			var result = await _partService.StatusExistAsync(status.Id);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task StatusExistAsync_WithNonExistingStatus_ShouldReturnFalse()
		{
			// Arrange
			int invalidStatusId = 99999;

			// Act
			var result = await _partService.StatusExistAsync(invalidStatusId);

			// Assert
			Assert.That(result, Is.False);
		}

		#endregion

		#region Search Functionality Tests

		[Test]
		public async Task AllAsync_WithSearchByToolNumber_ShouldReturnMatchingParts()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync(searchByName: part.ToolNumber.ToString());

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");

			// Verify that returned parts have the matching tool number
			foreach (var returnedPart in result.Parts)
			{
				Assert.That(returnedPart.ToolNumber, Is.EqualTo(part.ToolNumber),
					$"Part {returnedPart.Name} should have tool number {part.ToolNumber}");
			}
		}

		[Test]
		public async Task AllAsync_WithSearchByPartArticleNumber_ShouldReturnMatchingParts()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync(searchByName: part.PartArticleNumber);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");
		}

		[Test]
		public async Task AllAsync_WithSearchByPartClientNumber_ShouldReturnMatchingParts()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.AllAsync(searchByName: part.PartClientNumber);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.GreaterThan(0), "Parts count is 0");
		}

		[Test]
		public async Task AllAsync_WithSearchByInvalidToolNumber_ShouldReturnEmptyResult()
		{
			// Arrange
			string invalidToolNumber = "99999";

			// Act
			var result = await _partService.AllAsync(searchByName: invalidToolNumber);

			// Assert
			Assert.That(result, Is.Not.Null, "PartQueryServiceModel is null");
			Assert.That(result.Parts, Is.Not.Null, "Parts is null");
			Assert.That(result.Parts.Count, Is.EqualTo(0), "Parts count should be 0");
		}

		[Test]
		public async Task AllAsync_WithAllSortingOptions_ExecutesSuccessfully()
		{
			// Test all sorting enum values to ensure complete coverage
			var sortingOptions = Enum.GetValues<PartSorting>();

			foreach (var sorting in sortingOptions)
			{
				// Act
				var result = await _partService.AllAsync(sorting: sorting);

				// Assert
				Assert.That(result, Is.Not.Null, $"Result should not be null for sorting: {sorting}");
				Assert.That(result.Parts, Is.Not.Null, $"Parts should not be null for sorting: {sorting}");
			}
		}

		[Test]
		public async Task AllAsync_WithProjectNumberDescendingSorting_ReturnsSortedParts()
		{
			// Arrange & Act
			var result = await _partService.AllAsync(sorting: PartSorting.ProjectNumberDescending);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parts, Is.Not.Null);
			if (result.Parts.Count() > 1)
			{
				var projectNumbers = result.Parts.Select(p => p.ProjectNumber).ToList();
				var sortedProjectNumbers = projectNumbers.OrderByDescending(pn => pn).ToList();
				Assert.That(projectNumbers, Is.EqualTo(sortedProjectNumbers),
					"Parts should be sorted by project number descending");
			}
		}

		[Test]
		public async Task AllAsync_WithNullSearchAndStatus_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _partService.AllAsync(searchByName: null, status: null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parts, Is.Not.Null);
		}

		[Test]
		public async Task AllAsync_WithEmptySearchAndStatus_HandlesGracefully()
		{
			// Arrange & Act
			var result = await _partService.AllAsync(searchByName: "", status: "");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parts, Is.Not.Null);
		}

		#endregion

		#region Error Handling and Edge Cases

		[Test]
		public async Task PartDetailsByIdAsync_WithInvalidId_ThrowsException()
		{
			// Arrange
			int invalidPartId = 99999;

			// Act & Assert
			Assert.ThrowsAsync<InvalidOperationException>(async () =>
				await _partService.PartDetailsByIdAsync(invalidPartId));
		}

		[Test]
		public async Task GetPartFormModelForEditAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidPartId = 99999;

			// Act
			var result = await _partService.GetPartFormModelForEditAsync(invalidPartId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetPartForDeletingByIdAsync_WithInvalidId_ReturnsNull()
		{
			// Arrange
			int invalidPartId = 99999;

			// Act
			var result = await _partService.GetPartForDeletingByIdAsync(invalidPartId);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task DeletePartByIdAsync_WithInvalidId_DoesNotThrowException()
		{
			// Arrange
			int invalidPartId = 99999;

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
				await _partService.DeletePartByIdAsync(invalidPartId));
		}

		[Test]
		public async Task EditAsync_WithInvalidId_DoesNotThrowException()
		{
			// Arrange
			int invalidPartId = 99999;
			var partFormModel = new PartFormModel()
			{
				Name = "Test Name",
				ImageUrl = "Test Image",
				PartArticleNumber = "Test Article",
				PartClientNumber = "Test Client",
				PartModel = "Test Model",
				ToolNumber = 1000,
				PartStatusId = 1
			};

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
				await _partService.EditAsync(invalidPartId, partFormModel, 1, 1));
		}

		[Test]
		public async Task GetPartFormModelForEditAsync_WithValidId_PopulatesStatuses()
		{
			// Arrange
			var part = await _repository.AllReadOnly<Part>().FirstOrDefaultAsync();
			Assert.That(part, Is.Not.Null, "Part is null");

			// Act
			var result = await _partService.GetPartFormModelForEditAsync(part.Id);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Statuses, Is.Not.Null);
			Assert.That(result.Statuses.Count(), Is.GreaterThan(0));
		}

		[Test]
		public async Task AddNewPartAsync_ReturnsCorrectPartId()
		{
			// Arrange
			var partFormModel = new PartFormModel()
			{
				Name = "Test Part for ID Return",
				ImageUrl = "TestImageUrl",
				PartArticleNumber = "TEST.ID.001",
				PartClientNumber = "TID-001",
				PartModel = "TestIDModel",
				ToolNumber = 9999,
				PartStatusId = 1,
				ProjectNumber = "249100"
			};

			// Act
			var returnedId = await _partService.AddNewPartAsync(partFormModel, 1);

			// Assert
			Assert.That(returnedId, Is.GreaterThan(0));

			// Verify the part was actually created with the returned ID
			var createdPart = await _repository.GetByIdAsync<Part>(returnedId);
			Assert.That(createdPart, Is.Not.Null);
			Assert.That(createdPart.Name, Is.EqualTo(partFormModel.Name));
		}

		#endregion

		#region Model Coverage Tests

		[Test]
		public void PartQueryServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PartQueryServiceModel();

			// Assert
			Assert.That(model.Parts, Is.Not.Null);
			Assert.That(model.Parts.Count(), Is.EqualTo(0));
			Assert.That(model.TotalPartsCount, Is.EqualTo(0));
		}

		[Test]
		public void PartQueryServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange
			var parts = new List<PartServiceModel>
			{
				new PartServiceModel { Id = 1, Name = "Part 1" },
				new PartServiceModel { Id = 2, Name = "Part 2" }
			};

			// Act
			var model = new PartQueryServiceModel
			{
				TotalPartsCount = 100,
				Parts = parts
			};

			// Assert
			Assert.That(model.TotalPartsCount, Is.EqualTo(100));
			Assert.That(model.Parts.Count(), Is.EqualTo(2));
			Assert.That(model.Parts.First().Name, Is.EqualTo("Part 1"));
			Assert.That(model.Parts.Last().Name, Is.EqualTo("Part 2"));
		}

		[Test]
		public void PartServiceModel_AllProperties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new PartServiceModel
			{
				Id = 123,
				Name = "Test Part Name",
				PartArticleNumber = "TEST.123.456",
				PartClientNumber = "TC-123",
				ToolNumber = 2500,
				ImageUrl = "https://test.com/image.jpg",
				PartModel = "TestModel",
				ProjectNumber = "PRJ-123"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(123));
			Assert.That(model.Name, Is.EqualTo("Test Part Name"));
			Assert.That(model.PartArticleNumber, Is.EqualTo("TEST.123.456"));
			Assert.That(model.PartClientNumber, Is.EqualTo("TC-123"));
			Assert.That(model.ToolNumber, Is.EqualTo(2500));
			Assert.That(model.ImageUrl, Is.EqualTo("https://test.com/image.jpg"));
			Assert.That(model.PartModel, Is.EqualTo("TestModel"));
			Assert.That(model.ProjectNumber, Is.EqualTo("PRJ-123"));
		}

		[Test]
		public void PartServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PartServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
			Assert.That(model.PartArticleNumber, Is.EqualTo(string.Empty));
			Assert.That(model.PartClientNumber, Is.EqualTo(string.Empty));
			Assert.That(model.ToolNumber, Is.EqualTo(0));
			Assert.That(model.ImageUrl, Is.EqualTo(string.Empty));
			Assert.That(model.PartModel, Is.EqualTo(string.Empty));
			Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
		}

		[Test]
		public void PartStatusServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new PartStatusServiceModel
			{
				Id = 5,
				Name = "Completed"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(5));
			Assert.That(model.Name, Is.EqualTo("Completed"));
		}

		[Test]
		public void PartStatusServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PartStatusServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
		}

		[Test]
		public void PartDeleteServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new PartDeleteServiceModel
			{
				Id = 10,
				Name = "Part to Delete",
				PartArticleNumber = "DEL.123.456",
				ProjectNumber = "PRJ-DEL"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(10));
			Assert.That(model.Name, Is.EqualTo("Part to Delete"));
			Assert.That(model.PartArticleNumber, Is.EqualTo("DEL.123.456"));
			Assert.That(model.ProjectNumber, Is.EqualTo("PRJ-DEL"));
		}

		[Test]
		public void PartDeleteServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PartDeleteServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
			Assert.That(model.PartArticleNumber, Is.EqualTo(string.Empty));
			Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
		}

		[Test]
		public void PartDetailsServiceModel_Properties_CanBeSetAndRetrieved()
		{
			// Arrange & Act
			var model = new PartDetailsServiceModel
			{
				Id = 15,
				ImageUrl = "https://details.com/image.jpg",
				Name = "Detailed Part",
				PartArticleNumber = "DET.789.012",
				PartClientNumber = "DC-789",
				PartModel = "DetailModel",
				ProjectNumber = "PRJ-DET",
				ToolNumber = 3000,
				Status = "Ready"
			};

			// Assert
			Assert.That(model.Id, Is.EqualTo(15));
			Assert.That(model.ImageUrl, Is.EqualTo("https://details.com/image.jpg"));
			Assert.That(model.Name, Is.EqualTo("Detailed Part"));
			Assert.That(model.PartArticleNumber, Is.EqualTo("DET.789.012"));
			Assert.That(model.PartClientNumber, Is.EqualTo("DC-789"));
			Assert.That(model.PartModel, Is.EqualTo("DetailModel"));
			Assert.That(model.ProjectNumber, Is.EqualTo("PRJ-DET"));
			Assert.That(model.ToolNumber, Is.EqualTo(3000));
			Assert.That(model.Status, Is.EqualTo("Ready"));
		}

		[Test]
		public void PartDetailsServiceModel_DefaultValues_AreSetCorrectly()
		{
			// Arrange & Act
			var model = new PartDetailsServiceModel();

			// Assert
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.ImageUrl, Is.EqualTo(string.Empty));
			Assert.That(model.Name, Is.EqualTo(string.Empty));
			Assert.That(model.PartArticleNumber, Is.EqualTo(string.Empty));
			Assert.That(model.PartClientNumber, Is.EqualTo(string.Empty));
			Assert.That(model.PartModel, Is.EqualTo(string.Empty));
			Assert.That(model.ProjectNumber, Is.EqualTo(string.Empty));
			Assert.That(model.ToolNumber, Is.EqualTo(0));
			Assert.That(model.Status, Is.EqualTo(string.Empty));
		}

		#endregion

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
