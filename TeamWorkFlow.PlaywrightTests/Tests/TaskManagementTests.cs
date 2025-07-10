using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class TaskManagementTests : BaseTest
{
    [SetUp]
    public async Task TaskTestSetUp()
    {
        // Login as admin for task management tests
        await LoginAsAdminAsync();
    }

    [Test]
    public async Task TasksList_ShouldLoadCorrectly()
    {
        // Act
        await TasksPage.NavigateToListAsync();

        // Assert
        Assert.That(await TasksPage.IsOnTasksListPageAsync(), Is.True, "Should be on tasks list page");
        await AssertPageTitleContains("Task");
    }

    [Test]
    public async Task CreateTask_WithValidData_ShouldSucceed()
    {
        // Arrange
        await TasksPage.NavigateToListAsync();
        var initialTaskCount = await TasksPage.GetTasksCountAsync();

        // Act
        await TasksPage.ClickCreateNewTaskAsync();
        await TasksPage.CreateSampleTaskAsync();

        // Assert
        Assert.That(await TasksPage.HasSuccessMessageAsync(), Is.True, "Should show success message");
        
        // Navigate back to list and verify task was created
        await TasksPage.NavigateToListAsync();
        var finalTaskCount = await TasksPage.GetTasksCountAsync();
        Assert.That(finalTaskCount, Is.GreaterThan(initialTaskCount), "Task count should increase");
        
        // Verify the task exists in the list
        var taskExists = await TasksPage.TaskExistsAsync(Config.SampleTask.Name);
        Assert.That(taskExists, Is.True, "Created task should appear in the list");
    }

    [Test]
    public async Task CreateTask_WithEmptyName_ShouldShowValidationError()
    {
        // Arrange
        await TasksPage.NavigateToCreateAsync();

        // Act
        await TasksPage.CreateTaskAsync("", "Description", "PROJ001", 
            DateTime.Now.ToString("dd/MM/yyyy"), 
            DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"));

        // Assert
        Assert.That(await TasksPage.HasValidationErrorsAsync(), Is.True, "Should show validation errors");
        Assert.That(await TasksPage.IsTaskFormValidAsync(), Is.False, "Form should be invalid");
    }

    [Test]
    public async Task CreateTask_WithInvalidDateFormat_ShouldShowValidationError()
    {
        // Arrange
        await TasksPage.NavigateToCreateAsync();

        // Act
        await TasksPage.CreateTaskAsync("Test Task", "Description", "PROJ001", 
            "invalid-date", "another-invalid-date");

        // Assert
        Assert.That(await TasksPage.HasValidationErrorsAsync(), Is.True, "Should show validation errors for invalid dates");
    }

    [Test]
    public async Task TaskDetails_ShouldDisplayCorrectInformation()
    {
        // Arrange - Create a task first
        await TasksPage.NavigateToCreateAsync();
        await TasksPage.CreateSampleTaskAsync();
        await TasksPage.NavigateToListAsync();

        // Act
        await TasksPage.ClickFirstTaskDetailsAsync();

        // Assert
        Assert.That(await TasksPage.IsOnTaskDetailsPageAsync(), Is.True, "Should be on task details page");
        
        var taskName = await TasksPage.GetTaskNameFromDetailsAsync();
        Assert.That(taskName, Is.Not.Empty, "Task name should be displayed");
        
        var taskDescription = await TasksPage.GetTaskDescriptionFromDetailsAsync();
        Assert.That(taskDescription, Is.Not.Empty, "Task description should be displayed");
    }

    [Test]
    public async Task EditTask_WithValidData_ShouldUpdateTask()
    {
        // Arrange - Create a task first
        await TasksPage.NavigateToCreateAsync();
        await TasksPage.CreateSampleTaskAsync();
        await TasksPage.NavigateToListAsync();

        // Act
        await TasksPage.ClickFirstTaskEditAsync();
        
        var updatedName = "Updated Task Name";
        var updatedDescription = "Updated task description";
        await TasksPage.EditTaskAsync(updatedName, updatedDescription);

        // Assert
        Assert.That(await TasksPage.HasSuccessMessageAsync(), Is.True, "Should show success message after edit");
        
        // Verify the changes
        await TasksPage.ClickFirstTaskDetailsAsync();
        var displayedName = await TasksPage.GetTaskNameFromDetailsAsync();
        var displayedDescription = await TasksPage.GetTaskDescriptionFromDetailsAsync();
        
        Assert.That(displayedName, Does.Contain(updatedName), "Task name should be updated");
        Assert.That(displayedDescription, Does.Contain(updatedDescription), "Task description should be updated");
    }

    [Test]
    public async Task DeleteTask_ShouldRemoveTaskFromList()
    {
        // Arrange - Create a task first
        await TasksPage.NavigateToCreateAsync();
        await TasksPage.CreateSampleTaskAsync();
        await TasksPage.NavigateToListAsync();
        var initialTaskCount = await TasksPage.GetTasksCountAsync();

        // Act
        await TasksPage.ClickFirstTaskDeleteAsync();
        await TasksPage.ConfirmDeleteAsync();

        // Assert
        Assert.That(await TasksPage.HasSuccessMessageAsync(), Is.True, "Should show success message after deletion");
        
        // Verify task count decreased
        await TasksPage.NavigateToListAsync();
        var finalTaskCount = await TasksPage.GetTasksCountAsync();
        Assert.That(finalTaskCount, Is.LessThan(initialTaskCount), "Task count should decrease after deletion");
    }

    [Test]
    public async Task SearchTasks_WithValidTerm_ShouldFilterResults()
    {
        // Arrange - Create multiple tasks
        await TasksPage.NavigateToCreateAsync();
        await TasksPage.CreateTaskAsync("Searchable Task 1", "Description 1", "PROJ001", 
            DateTime.Now.ToString("dd/MM/yyyy"), 
            DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"));
        
        await TasksPage.NavigateToCreateAsync();
        await TasksPage.CreateTaskAsync("Different Task", "Description 2", "PROJ002", 
            DateTime.Now.ToString("dd/MM/yyyy"), 
            DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"));

        await TasksPage.NavigateToListAsync();

        // Act
        await TasksPage.SearchTasksAsync("Searchable");

        // Assert
        var taskNames = await TasksPage.GetTaskNamesAsync();
        Assert.That(taskNames.Any(name => name.Contains("Searchable")), Is.True, 
            "Search results should contain tasks with search term");
        Assert.That(taskNames.Any(name => name.Contains("Different")), Is.False, 
            "Search results should not contain tasks without search term");
    }

    [Test]
    public async Task TasksList_ShouldBeResponsive()
    {
        // Arrange
        await TasksPage.NavigateToListAsync();

        // Act & Assert
        var isResponsive = await TasksPage.IsResponsiveDesignWorkingAsync();
        Assert.That(isResponsive, Is.True, "Tasks list should be responsive on different screen sizes");
    }

    [Test]
    public async Task TaskForm_ShouldHaveProperValidation()
    {
        // Arrange
        await TasksPage.NavigateToCreateAsync();

        // Act - Submit empty form
        var submitButton = Page.Locator("button[type='submit'], input[type='submit']");
        await submitButton.ClickAsync();

        // Assert
        Assert.That(await TasksPage.HasValidationErrorsAsync(), Is.True, 
            "Empty form should show validation errors");
        
        var validationErrors = await TasksPage.GetValidationErrorsAsync();
        Assert.That(validationErrors.Length, Is.GreaterThan(0), 
            "Should have specific validation error messages");
    }

    [Test]
    public async Task TaskNavigation_ShouldWorkCorrectly()
    {
        // Act & Assert - Test navigation between different task pages
        await TasksPage.NavigateToListAsync();
        Assert.That(await TasksPage.IsOnTasksListPageAsync(), Is.True, "Should navigate to tasks list");

        await TasksPage.NavigateToCreateAsync();
        await AssertUrlContains("Create");

        // Navigate back to list
        await TasksPage.NavigateToListAsync();
        Assert.That(await TasksPage.IsOnTasksListPageAsync(), Is.True, "Should navigate back to tasks list");
    }
}
