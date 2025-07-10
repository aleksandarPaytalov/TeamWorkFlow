using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class ProjectManagementTests : BaseTest
{
    [SetUp]
    public async Task ProjectTestSetUp()
    {
        // Login as admin for project management tests
        await LoginAsAdminAsync();
    }

    [Test]
    public async Task ProjectsList_ShouldLoadCorrectly()
    {
        // Act
        await ProjectsPage.NavigateToListAsync();

        // Assert
        Assert.That(await ProjectsPage.IsOnProjectsListPageAsync(), Is.True, "Should be on projects list page");
        await AssertPageTitleContains("Project");
    }

    [Test]
    public async Task CreateProject_WithValidData_ShouldSucceed()
    {
        // Arrange
        await ProjectsPage.NavigateToListAsync();
        var initialProjectCount = await ProjectsPage.GetProjectsCountAsync();

        // Act
        await ProjectsPage.ClickCreateNewProjectAsync();
        await ProjectsPage.CreateSampleProjectAsync();

        // Assert
        Assert.That(await ProjectsPage.HasSuccessMessageAsync(), Is.True, "Should show success message");
        
        // Navigate back to list and verify project was created
        await ProjectsPage.NavigateToListAsync();
        var finalProjectCount = await ProjectsPage.GetProjectsCountAsync();
        Assert.That(finalProjectCount, Is.GreaterThan(initialProjectCount), "Project count should increase");
        
        // Verify the project exists in the list
        var projectExists = await ProjectsPage.ProjectExistsAsync(Config.SampleProject.Name);
        Assert.That(projectExists, Is.True, "Created project should appear in the list");
    }

    [Test]
    public async Task CreateProject_WithEmptyName_ShouldShowValidationError()
    {
        // Arrange
        await ProjectsPage.NavigateToCreateAsync();

        // Act
        await ProjectsPage.CreateProjectAsync("", "PROJ001", "Active", 40);

        // Assert
        Assert.That(await ProjectsPage.HasValidationErrorsAsync(), Is.True, "Should show validation errors");
        Assert.That(await ProjectsPage.IsProjectFormValidAsync(), Is.False, "Form should be invalid");
    }

    [Test]
    public async Task CreateProject_WithDuplicateProjectNumber_ShouldShowError()
    {
        // Arrange - Create first project
        await ProjectsPage.NavigateToCreateAsync();
        await ProjectsPage.CreateProjectAsync("First Project", "DUPLICATE001", "Active", 20);
        
        // Act - Try to create second project with same number
        await ProjectsPage.NavigateToCreateAsync();
        await ProjectsPage.CreateProjectAsync("Second Project", "DUPLICATE001", "Active", 30);

        // Assert
        Assert.That(await ProjectsPage.HasValidationErrorsAsync() || await ProjectsPage.HasErrorMessageAsync(), 
            Is.True, "Should show error for duplicate project number");
    }

    [Test]
    public async Task ProjectDetails_ShouldDisplayCorrectInformation()
    {
        // Arrange - Create a project first
        await ProjectsPage.NavigateToCreateAsync();
        await ProjectsPage.CreateSampleProjectAsync();
        await ProjectsPage.NavigateToListAsync();

        // Act
        await ProjectsPage.ClickFirstProjectDetailsAsync();

        // Assert
        Assert.That(await ProjectsPage.IsOnProjectDetailsPageAsync(), Is.True, "Should be on project details page");
        
        var projectName = await ProjectsPage.GetProjectNameFromDetailsAsync();
        Assert.That(projectName, Is.Not.Empty, "Project name should be displayed");
        
        var projectNumber = await ProjectsPage.GetProjectNumberFromDetailsAsync();
        Assert.That(projectNumber, Is.Not.Empty, "Project number should be displayed");
    }

    [Test]
    public async Task EditProject_WithValidData_ShouldUpdateProject()
    {
        // Arrange - Create a project first
        await ProjectsPage.NavigateToCreateAsync();
        await ProjectsPage.CreateSampleProjectAsync();
        await ProjectsPage.NavigateToListAsync();

        // Act
        await ProjectsPage.ClickFirstProjectEditAsync();
        
        var updatedName = "Updated Project Name";
        var updatedNumber = "UPD001";
        await ProjectsPage.EditProjectAsync(updatedName, updatedNumber);

        // Assert
        Assert.That(await ProjectsPage.HasSuccessMessageAsync(), Is.True, "Should show success message after edit");
        
        // Verify the changes
        await ProjectsPage.ClickFirstProjectDetailsAsync();
        var displayedName = await ProjectsPage.GetProjectNameFromDetailsAsync();
        var displayedNumber = await ProjectsPage.GetProjectNumberFromDetailsAsync();
        
        Assert.That(displayedName, Does.Contain(updatedName), "Project name should be updated");
        Assert.That(displayedNumber, Does.Contain(updatedNumber), "Project number should be updated");
    }

    [Test]
    public async Task DeleteProject_ShouldRemoveProjectFromList()
    {
        // Arrange - Create a project first
        await ProjectsPage.NavigateToCreateAsync();
        await ProjectsPage.CreateSampleProjectAsync();
        await ProjectsPage.NavigateToListAsync();
        var initialProjectCount = await ProjectsPage.GetProjectsCountAsync();

        // Act
        await ProjectsPage.ClickFirstProjectDeleteAsync();
        await ProjectsPage.ConfirmDeleteAsync();

        // Assert
        Assert.That(await ProjectsPage.HasSuccessMessageAsync(), Is.True, "Should show success message after deletion");
        
        // Verify project count decreased
        await ProjectsPage.NavigateToListAsync();
        var finalProjectCount = await ProjectsPage.GetProjectsCountAsync();
        Assert.That(finalProjectCount, Is.LessThan(initialProjectCount), "Project count should decrease after deletion");
    }

    [Test]
    public async Task SearchProjects_WithValidTerm_ShouldFilterResults()
    {
        // Arrange - Create multiple projects
        await ProjectsPage.NavigateToCreateAsync();
        await ProjectsPage.CreateProjectAsync("Searchable Project 1", "SEARCH001", "Active", 10);
        
        await ProjectsPage.NavigateToCreateAsync();
        await ProjectsPage.CreateProjectAsync("Different Project", "DIFF001", "Active", 20);

        await ProjectsPage.NavigateToListAsync();

        // Act
        await ProjectsPage.SearchProjectsAsync("Searchable");

        // Assert
        var projectNames = await ProjectsPage.GetProjectNamesAsync();
        Assert.That(projectNames.Any(name => name.Contains("Searchable")), Is.True, 
            "Search results should contain projects with search term");
        Assert.That(projectNames.Any(name => name.Contains("Different")), Is.False, 
            "Search results should not contain projects without search term");
    }

    [Test]
    public async Task ProjectsList_ShouldBeResponsive()
    {
        // Arrange
        await ProjectsPage.NavigateToListAsync();

        // Act & Assert
        var isResponsive = await ProjectsPage.IsResponsiveDesignWorkingAsync();
        Assert.That(isResponsive, Is.True, "Projects list should be responsive on different screen sizes");
    }

    [Test]
    public async Task ProjectForm_ShouldHaveProperValidation()
    {
        // Arrange
        await ProjectsPage.NavigateToCreateAsync();

        // Act - Submit empty form
        var submitButton = Page.Locator("button[type='submit'], input[type='submit']");
        await submitButton.ClickAsync();

        // Assert
        Assert.That(await ProjectsPage.HasValidationErrorsAsync(), Is.True, 
            "Empty form should show validation errors");
        
        var validationErrors = await ProjectsPage.GetValidationErrorsAsync();
        Assert.That(validationErrors.Length, Is.GreaterThan(0), 
            "Should have specific validation error messages");
    }

    [Test]
    public async Task ProjectNavigation_ShouldWorkCorrectly()
    {
        // Act & Assert - Test navigation between different project pages
        await ProjectsPage.NavigateToListAsync();
        Assert.That(await ProjectsPage.IsOnProjectsListPageAsync(), Is.True, "Should navigate to projects list");

        await ProjectsPage.NavigateToCreateAsync();
        await AssertUrlContains("Create");

        // Navigate back to list
        await ProjectsPage.NavigateToListAsync();
        Assert.That(await ProjectsPage.IsOnProjectsListPageAsync(), Is.True, "Should navigate back to projects list");
    }

    [Test]
    public async Task ProjectCardInteraction_ShouldWorkCorrectly()
    {
        // Arrange
        await ProjectsPage.NavigateToListAsync();

        // Act & Assert - Test card-based interactions if using card layout
        var projectCards = Page.Locator(".project-card, .card");
        if (await projectCards.First.IsVisibleAsync())
        {
            // Test hover effects and click interactions
            await projectCards.First.HoverAsync();
            await Page.WaitForTimeoutAsync(500);
            
            // Verify card is interactive
            var isClickable = await projectCards.First.IsEnabledAsync();
            Assert.That(isClickable, Is.True, "Project cards should be interactive");
        }
    }
}
