using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using TeamWorkFlow.Core.Models.Part;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    public class IQueryablePartExtensionUnitTests
    {
        private TeamWorkFlowDbContext _dbContext = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TeamWorkFlowDbContext(options);

            // Seed test data
            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        private void SeedTestData()
        {
            // Create test project statuses first
            var projectStatus = new ProjectStatus
            {
                Id = 1,
                Name = "In Production"
            };
            _dbContext.ProjectStatusEnumerable.Add(projectStatus);

            // Create test part statuses
            var partStatus = new PartStatus
            {
                Id = 1,
                Name = "Released"
            };
            _dbContext.PartStatusEnumerable.Add(partStatus);

            // Create test projects
            var project1 = new Project
            {
                Id = 1,
                ProjectName = "Test Project 1",
                ProjectNumber = "TP-001",
                ProjectStatusId = 1
            };

            var project2 = new Project
            {
                Id = 2,
                ProjectName = "Test Project 2",
                ProjectNumber = "TP-002",
                ProjectStatusId = 1
            };

            _dbContext.Projects.AddRange(project1, project2);

            // Create test parts
            var part1 = new Part
            {
                Id = 1,
                Name = "Engine Block",
                PartArticleNumber = "EB-001",
                PartClientNumber = "CLIENT-001",
                PartModel = "V8-Engine",
                ToolNumber = 1001,
                ImageUrl = "https://example.com/engine.jpg",
                ProjectId = 1,
                PartStatusId = 1
            };

            var part2 = new Part
            {
                Id = 2,
                Name = "Brake Disc",
                PartArticleNumber = "BD-002",
                PartClientNumber = "CLIENT-002",
                PartModel = "Standard-Brake",
                ToolNumber = 1002,
                ImageUrl = "https://example.com/brake.jpg",
                ProjectId = 2,
                PartStatusId = 1
            };

            var part3 = new Part
            {
                Id = 3,
                Name = "Transmission",
                PartArticleNumber = "TR-003",
                PartClientNumber = "CLIENT-003",
                PartModel = "Auto-Trans",
                ToolNumber = 1003,
                ImageUrl = "https://example.com/transmission.jpg",
                ProjectId = 1,
                PartStatusId = 1
            };

            _dbContext.Parts.AddRange(part1, part2, part3);
            _dbContext.SaveChanges();
        }

        [Test]
        public void ProjectToPartServiceModels_WithValidParts_ReturnsCorrectServiceModels()
        {
            // Arrange
            var partsQuery = _dbContext.Parts.AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels().ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));

            // Verify first part
            var firstPart = result.First(p => p.Id == 1);
            Assert.That(firstPart.Id, Is.EqualTo(1));
            Assert.That(firstPart.Name, Is.EqualTo("Engine Block"));
            Assert.That(firstPart.PartArticleNumber, Is.EqualTo("EB-001"));
            Assert.That(firstPart.PartClientNumber, Is.EqualTo("CLIENT-001"));
            Assert.That(firstPart.PartModel, Is.EqualTo("V8-Engine"));
            Assert.That(firstPart.ToolNumber, Is.EqualTo(1001));
            Assert.That(firstPart.ImageUrl, Is.EqualTo("https://example.com/engine.jpg"));
            Assert.That(firstPart.ProjectNumber, Is.EqualTo("TP-001"));
        }

        [Test]
        public void ProjectToPartServiceModels_WithFilteredParts_ReturnsFilteredResults()
        {
            // Arrange
            var partsQuery = _dbContext.Parts
                .Where(p => p.ProjectId == 1)
                .AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels().ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.ProjectNumber == "TP-001"), Is.True);
        }

        [Test]
        public void ProjectToPartServiceModels_WithSpecificPart_ReturnsCorrectMapping()
        {
            // Arrange
            var partsQuery = _dbContext.Parts
                .Where(p => p.Id == 2)
                .AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels().ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));

            var part = result.First();
            Assert.That(part.Id, Is.EqualTo(2));
            Assert.That(part.Name, Is.EqualTo("Brake Disc"));
            Assert.That(part.PartArticleNumber, Is.EqualTo("BD-002"));
            Assert.That(part.PartClientNumber, Is.EqualTo("CLIENT-002"));
            Assert.That(part.PartModel, Is.EqualTo("Standard-Brake"));
            Assert.That(part.ToolNumber, Is.EqualTo(1002));
            Assert.That(part.ImageUrl, Is.EqualTo("https://example.com/brake.jpg"));
            Assert.That(part.ProjectNumber, Is.EqualTo("TP-002"));
        }

        [Test]
        public void ProjectToPartServiceModels_WithEmptyQuery_ReturnsEmptyResult()
        {
            // Arrange
            var partsQuery = _dbContext.Parts
                .Where(p => p.Id == 999) // Non-existent ID
                .AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels().ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void ProjectToPartServiceModels_WithOrderedQuery_MaintainsOrder()
        {
            // Arrange
            var partsQuery = _dbContext.Parts
                .OrderBy(p => p.Name)
                .AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels().ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            
            // Verify order is maintained (alphabetical by name)
            Assert.That(result[0].Name, Is.EqualTo("Brake Disc"));
            Assert.That(result[1].Name, Is.EqualTo("Engine Block"));
            Assert.That(result[2].Name, Is.EqualTo("Transmission"));
        }

        [Test]
        public void ProjectToPartServiceModels_ReturnsPartServiceModelType()
        {
            // Arrange
            var partsQuery = _dbContext.Parts.AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IQueryable<PartServiceModel>>());
            
            // Verify the actual type when materialized
            var materializedResult = result.ToList();
            Assert.That(materializedResult, Is.All.InstanceOf<PartServiceModel>());
        }

        [Test]
        public void ProjectToPartServiceModels_WithComplexQuery_WorksCorrectly()
        {
            // Arrange
            var partsQuery = _dbContext.Parts
                .Where(p => p.Name.Contains("Engine") || p.Name.Contains("Brake"))
                .OrderByDescending(p => p.Id)
                .AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels().ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            
            // Verify order (descending by ID)
            Assert.That(result[0].Id, Is.EqualTo(2)); // Brake Disc
            Assert.That(result[1].Id, Is.EqualTo(1)); // Engine Block
            
            // Verify content
            Assert.That(result[0].Name, Is.EqualTo("Brake Disc"));
            Assert.That(result[1].Name, Is.EqualTo("Engine Block"));
        }

        [Test]
        public void ProjectToPartServiceModels_IncludesAllRequiredProperties()
        {
            // Arrange
            var partsQuery = _dbContext.Parts
                .Where(p => p.Id == 1)
                .AsQueryable();

            // Act
            var result = partsQuery.ProjectToPartServiceModels().First();

            // Assert - Verify all properties are mapped
            Assert.That(result.Id, Is.Not.EqualTo(0));
            Assert.That(result.ImageUrl, Is.Not.Null);
            Assert.That(result.Name, Is.Not.Null);
            Assert.That(result.PartArticleNumber, Is.Not.Null);
            Assert.That(result.PartClientNumber, Is.Not.Null);
            Assert.That(result.PartModel, Is.Not.Null);
            Assert.That(result.ProjectNumber, Is.Not.Null);
            Assert.That(result.ToolNumber, Is.GreaterThan(0));
        }

        [Test]
        public void ProjectToPartServiceModels_CanBeChainedWithOtherLinqOperations()
        {
            // Arrange
            var partsQuery = _dbContext.Parts.AsQueryable();

            // Act
            var result = partsQuery
                .ProjectToPartServiceModels()
                .Where(p => p.ProjectNumber == "TP-001")
                .OrderBy(p => p.Name)
                .ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.ProjectNumber == "TP-001"), Is.True);
            
            // Verify order
            Assert.That(result[0].Name, Is.EqualTo("Engine Block"));
            Assert.That(result[1].Name, Is.EqualTo("Transmission"));
        }
    }
}
