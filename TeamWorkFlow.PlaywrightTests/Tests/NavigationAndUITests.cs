using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class NavigationAndUITests : BaseTest
{
    [Test]
    public async Task HomePage_ShouldLoadCorrectly()
    {
        // Act
        await HomePage.NavigateAsync();

        // Assert
        Assert.That(await HomePage.IsOnHomePageAsync(), Is.True, "Should be on home page");
        Assert.That(await HomePage.IsDashboardLoadedAsync(), Is.True, "Dashboard should be loaded");
    }

    [Test]
    public async Task Navigation_WhenLoggedInAsAdmin_ShouldShowAllMenuItems()
    {
        // Arrange
        await LoginAsAdminAsync();

        // Act
        await HomePage.NavigateAsync();

        // Assert
        await AssertElementVisible(TasksLink, "Tasks navigation link");
        await AssertElementVisible(ProjectsLink, "Projects navigation link");
        await AssertElementVisible(MachinesLink, "Machines navigation link");
        await AssertElementVisible(OperatorsLink, "Operators navigation link");
        await AssertElementVisible(PartsLink, "Parts navigation link");
        await AssertElementVisible(AdminLink, "Admin navigation link");
    }

    [Test]
    public async Task Navigation_WhenLoggedInAsOperator_ShouldHideAdminMenuItems()
    {
        // Arrange
        await LoginAsOperatorAsync();

        // Act
        await HomePage.NavigateAsync();

        // Assert
        await AssertElementVisible(TasksLink, "Tasks navigation link");
        await AssertElementVisible(ProjectsLink, "Projects navigation link");
        await AssertElementVisible(MachinesLink, "Machines navigation link");
        
        // Admin-specific links should not be visible for operators
        var adminLink = Page.Locator("a[href*='Admin'], a:has-text('Admin')");
        var isAdminLinkVisible = await adminLink.IsVisibleAsync();
        Assert.That(isAdminLinkVisible, Is.False, "Admin link should not be visible for operators");
    }

    [Test]
    public async Task Navigation_BetweenPages_ShouldWorkCorrectly()
    {
        // Arrange
        await LoginAsAdminAsync();

        // Act & Assert - Navigate through different pages
        await HomePage.ClickViewAllTasksAsync();
        await AssertUrlContains("Task");

        await ClickNavigationLinkAsync("Projects");
        await AssertUrlContains("Project");

        await ClickNavigationLinkAsync("Machines");
        await AssertUrlContains("Machine");

        await ClickNavigationLinkAsync("Home");
        await AssertUrlContains("/");
    }

    [Test]
    public async Task ResponsiveDesign_ShouldWorkOnMobileViewport()
    {
        // Arrange
        await LoginAsAdminAsync();
        await HomePage.NavigateAsync();

        // Act - Switch to mobile viewport
        await Page.SetViewportSizeAsync(375, 667);
        await Page.WaitForTimeoutAsync(1000);

        // Assert
        var navigationBar = Page.Locator("nav.navbar, .navbar");
        await AssertElementVisible(navigationBar, "Navigation bar should be visible on mobile");

        // Check if mobile menu toggle is present
        var mobileMenuToggle = Page.Locator(".navbar-toggler, .mobile-menu-toggle, .hamburger");
        if (await mobileMenuToggle.IsVisibleAsync())
        {
            await mobileMenuToggle.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        // Navigation links should still be accessible
        var tasksLink = Page.Locator("a[href*='Task'], a:has-text('Tasks')");
        var isTasksLinkAccessible = await tasksLink.IsVisibleAsync();
        Assert.That(isTasksLinkAccessible, Is.True, "Tasks link should be accessible on mobile");
    }

    [Test]
    public async Task ResponsiveDesign_ShouldWorkOnTabletViewport()
    {
        // Arrange
        await LoginAsAdminAsync();
        await HomePage.NavigateAsync();

        // Act - Switch to tablet viewport
        await Page.SetViewportSizeAsync(768, 1024);
        await Page.WaitForTimeoutAsync(1000);

        // Assert
        var dashboardCards = Page.Locator(".summary-card, .dashboard-card");
        var cardCount = await dashboardCards.CountAsync();
        Assert.That(cardCount, Is.GreaterThan(0), "Dashboard cards should be visible on tablet");

        // Check layout adaptation
        var isResponsive = await HomePage.IsDashboardLoadedAsync();
        Assert.That(isResponsive, Is.True, "Dashboard should adapt to tablet viewport");
    }

    [Test]
    public async Task UserGreeting_ShouldDisplayCorrectUserName()
    {
        // Arrange & Act
        await LoginAsAdminAsync();

        // Assert
        var greetingText = await HomePage.GetUserGreetingTextAsync();
        Assert.That(greetingText, Does.Contain(Config.AdminUser.FirstName).Or.Contain(Config.AdminUser.LastName), 
            "User greeting should contain the logged-in user's name");
    }

    [Test]
    public async Task DashboardSummaryCards_ShouldDisplayData()
    {
        // Arrange
        await LoginAsAdminAsync();

        // Act
        await HomePage.NavigateAsync();

        // Assert
        var summaryCardsCount = await HomePage.GetSummaryCardsCountAsync();
        Assert.That(summaryCardsCount, Is.GreaterThan(0), "Should display summary cards");

        // Check individual summary cards
        var tasksSummary = await HomePage.GetTasksSummaryAsync();
        var projectsSummary = await HomePage.GetProjectsSummaryAsync();
        
        Assert.That(tasksSummary, Is.Not.Empty, "Tasks summary should not be empty");
        Assert.That(projectsSummary, Is.Not.Empty, "Projects summary should not be empty");
    }

    [Test]
    public async Task QuickActions_ShouldBeAccessible()
    {
        // Arrange
        await LoginAsAdminAsync();

        // Act
        await HomePage.NavigateAsync();

        // Assert
        if (await HomePage.HasQuickActionsAsync())
        {
            // Test quick action buttons
            var createTaskButton = Page.Locator("a:has-text('Create Task'), button:has-text('Create Task')");
            if (await createTaskButton.IsVisibleAsync())
            {
                await createTaskButton.ClickAsync();
                await WaitForPageLoad();
                await AssertUrlContains("Create");
            }
        }
    }

    [Test]
    public async Task SearchFunctionality_ShouldWorkAcrossPages()
    {
        // Arrange
        await LoginAsAdminAsync();

        // Test search on Tasks page
        await TasksPage.NavigateToListAsync();
        var searchInput = Page.Locator("input[name='searchTerm'], input[placeholder*='Search']");
        
        if (await searchInput.IsVisibleAsync())
        {
            await searchInput.FillAsync("test");
            var searchButton = Page.Locator("button:has-text('Search'), input[value='Search']");
            if (await searchButton.IsVisibleAsync())
            {
                await searchButton.ClickAsync();
                await WaitForPageLoad();
                
                // Verify search was performed
                var currentUrl = Page.Url;
                Assert.That(currentUrl, Does.Contain("search").Or.Contain("test"), 
                    "URL should reflect search parameters");
            }
        }
    }

    [Test]
    public async Task SortingFunctionality_ShouldWorkCorrectly()
    {
        // Arrange
        await LoginAsAdminAsync();
        await TasksPage.NavigateToListAsync();

        // Act
        var sortDropdown = Page.Locator("select[name*='sort'], select[name*='Sort']");
        if (await sortDropdown.IsVisibleAsync())
        {
            await sortDropdown.SelectOptionAsync("Name");
            await WaitForPageLoad();

            // Assert
            var taskNames = await TasksPage.GetTaskNamesAsync();
            if (taskNames.Length > 1)
            {
                // Check if names are sorted (basic check)
                var isSorted = taskNames.SequenceEqual(taskNames.OrderBy(x => x));
                Assert.That(isSorted, Is.True, "Tasks should be sorted by name");
            }
        }
    }

    [Test]
    public async Task PaginationControls_ShouldWorkCorrectly()
    {
        // Arrange
        await LoginAsAdminAsync();
        await TasksPage.NavigateToListAsync();

        // Act & Assert
        var paginationContainer = Page.Locator(".pagination, [data-testid='pagination']");
        if (await paginationContainer.IsVisibleAsync())
        {
            var nextButton = Page.Locator(".pagination .next, a:has-text('Next')");
            if (await nextButton.IsVisibleAsync() && await nextButton.IsEnabledAsync())
            {
                await nextButton.ClickAsync();
                await WaitForPageLoad();
                
                // Verify pagination worked
                var currentUrl = Page.Url;
                Assert.That(currentUrl, Does.Contain("page").Or.Contain("Page"), 
                    "URL should reflect pagination");
            }
        }
    }

    [Test]
    public async Task ErrorHandling_ShouldDisplayUserFriendlyMessages()
    {
        // Arrange
        await LoginAsAdminAsync();

        // Act - Try to access a non-existent page
        await Page.GotoAsync($"{Config.BaseUrl}/NonExistentPage");

        // Assert
        var errorMessage = Page.Locator(".error-message, .alert-danger, h1:has-text('Error')");
        var isErrorDisplayed = await errorMessage.IsVisibleAsync();
        
        if (isErrorDisplayed)
        {
            var errorText = await errorMessage.TextContentAsync();
            Assert.That(errorText, Is.Not.Empty, "Error message should not be empty");
        }
        
        // Should show 404 or similar error
        var pageTitle = await Page.TitleAsync();
        Assert.That(pageTitle, Does.Contain("Error").Or.Contain("404").Or.Contain("Not Found"), 
            "Page title should indicate error state");
    }

    [Test]
    public async Task AccessibilityFeatures_ShouldBePresent()
    {
        // Arrange
        await LoginAsAdminAsync();
        await HomePage.NavigateAsync();

        // Assert - Check for basic accessibility features
        var mainContent = Page.Locator("main, [role='main'], .main-content");
        if (await mainContent.IsVisibleAsync())
        {
            var hasMainLandmark = await mainContent.IsVisibleAsync();
            Assert.That(hasMainLandmark, Is.True, "Page should have main content landmark");
        }

        // Check for navigation landmarks
        var navigation = Page.Locator("nav, [role='navigation']");
        await AssertElementVisible(navigation, "Navigation landmark");

        // Check for proper heading structure
        var headings = Page.Locator("h1, h2, h3");
        var headingCount = await headings.CountAsync();
        Assert.That(headingCount, Is.GreaterThan(0), "Page should have proper heading structure");
    }

    // Helper methods
    private async Task ClickNavigationLinkAsync(string linkText)
    {
        var link = Page.Locator($"nav a:has-text('{linkText}')");
        await link.ClickAsync();
        await WaitForPageLoad();
    }

    private ILocator TasksLink => Page.Locator("a[href*='Task'], a:has-text('Tasks')");
    private ILocator ProjectsLink => Page.Locator("a[href*='Project'], a:has-text('Projects')");
    private ILocator MachinesLink => Page.Locator("a[href*='Machine'], a:has-text('Machines')");
    private ILocator OperatorsLink => Page.Locator("a[href*='Operator'], a:has-text('Operators')");
    private ILocator PartsLink => Page.Locator("a[href*='Part'], a:has-text('Parts')");
    private ILocator AdminLink => Page.Locator("a[href*='Admin'], a:has-text('Admin')");
}
