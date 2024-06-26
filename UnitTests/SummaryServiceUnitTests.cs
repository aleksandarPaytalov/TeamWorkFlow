﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Services;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace UnitTests
{
	[TestFixture]
	public class SummaryServiceUnitTests
	{
		private IRepository _repository;
		private ISummaryService _summaryService;
		private TeamWorkFlowDbContext _dbContext;
		private Mock<UserManager<IdentityUser>> _mockUserManager;

		[SetUp]
		public void Setup()
		{
			var mockUserStore = new Mock<IUserStore<IdentityUser>>();
			_mockUserManager = new Mock<UserManager<IdentityUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

			_mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
				.ReturnsAsync((IdentityUser user, string role) => role == "Operator" || role == "Admin");

			var configuration = new ConfigurationBuilder()
				.AddUserSecrets<SummaryServiceUnitTests>()
				.Build();

			var connectionString = configuration.GetConnectionString("Test");

			var options = new DbContextOptionsBuilder<TeamWorkFlowDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			_dbContext = new TeamWorkFlowDbContext(options);
			_repository = new Repository(_dbContext);
			_summaryService = new SummaryService(_repository);

			_dbContext.Database.EnsureDeleted();
			_dbContext.Database.EnsureCreated();
		}


		// To be Implemented
		[Test]
		public async Task SummaryAsync_ShouldReturnCorrectSummary()
		{
			
		}



		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}
	}
}
