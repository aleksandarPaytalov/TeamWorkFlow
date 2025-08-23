using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public abstract class BaseTest : PageTest
{
    protected TestConfiguration Config { get; private set; } = null!;
    protected LoginPage LoginPage { get; private set; } = null!;
    protected HomePage HomePage { get; private set; } = null!;
    protected TasksPage TasksPage { get; private set; } = null!;
    protected ProjectsPage ProjectsPage { get; private set; } = null!;
    protected ErrorPage ErrorPage { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Config = TestConfiguration.Instance;
    }

    [SetUp]
    public async Task SetUp()
    {
        // Initialize page objects
        LoginPage = new LoginPage(Page);
        HomePage = new HomePage(Page);
        TasksPage = new TasksPage(Page);
        ProjectsPage = new ProjectsPage(Page);
        ErrorPage = new ErrorPage(Page);

        // Set up browser context options
        Page.SetDefaultTimeout(Config.Timeout);

        // Only navigate to base URL if the test requires application connection
        // This allows tests to run without the application being started
        if (RequiresApplicationConnection())
        {
            await EnsureApplicationIsRunningAsync();
        }
    }

    /// <summary>
    /// Override this method in derived test classes to specify if the test requires application connection
    /// </summary>
    protected virtual bool RequiresApplicationConnection()
    {
        // By default, assume tests require application connection
        // Individual test classes can override this to return false for standalone tests
        return true;
    }

    /// <summary>
    /// Ensures the application is running and accessible
    /// </summary>
    protected async Task EnsureApplicationIsRunningAsync()
    {
        try
        {
            await Page.GotoAsync(Config.BaseUrl, new() { Timeout = 10000 });
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to connect to application at {Config.BaseUrl}. Make sure the TeamWorkFlow application is running. Error: {ex.Message}");
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        // Take screenshot on failure
        if (TestContext.CurrentContext.Result.Outcome.Status.ToString() == "Failed" && Config.ScreenshotOnFailure)
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var screenshotPath = $"screenshots/failed_{testName}_{timestamp}.png";
            
            Directory.CreateDirectory("screenshots");
            await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
            
            TestContext.WriteLine($"Screenshot saved: {screenshotPath}");
        }

        // Logout if logged in
        try
        {
            if (await IsLoggedInAsync())
            {
                await LogoutAsync();
            }
        }
        catch
        {
            // Ignore logout errors during cleanup
        }
    }

    protected async Task<bool> IsLoggedInAsync()
    {
        try
        {
            // Wait for page to load completely
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 5000 });

            // Check for logout button or user greeting as indicators of being logged in
            var logoutButton = Page.Locator("form[action*='Logout'] button[type='submit'], button:has-text('Logout')");
            var userGreeting = Page.Locator("a[title='Manage Account']:has-text('Hi '), a.nav-link:has-text('Hi ')");

            // Wait for either logout button or user greeting with longer timeout
            try
            {
                await Page.WaitForSelectorAsync("a[title='Manage Account']:has-text('Hi '), form[action*='Logout'] button", new() { Timeout = 5000 });
            }
            catch
            {
                // If specific elements not found, check URL-based approach
                var currentUrl = Page.Url;
                return !currentUrl.Contains("Login") && !currentUrl.Contains("login") && !currentUrl.Contains("Register");
            }

            return await logoutButton.IsVisibleAsync() || await userGreeting.IsVisibleAsync();
        }
        catch
        {
            // Fallback: Check if we're on a protected page (not login page)
            var currentUrl = Page.Url;
            return !currentUrl.Contains("Login") && !currentUrl.Contains("login") && !currentUrl.Contains("Register");
        }
    }

    protected async Task LogoutAsync()
    {
        var logoutButton = Page.Locator("a[href*='logout'], button:has-text('Logout')");
        if (await logoutButton.IsVisibleAsync())
        {
            await logoutButton.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    protected async Task LoginAsAdminAsync()
    {
        if (!await LoginPage.IsOnLoginPageAsync())
        {
            await LoginPage.NavigateAsync();
        }
        await LoginPage.LoginAsAdminAsync();
    }

    protected async Task LoginAsOperatorAsync()
    {
        if (!await LoginPage.IsOnLoginPageAsync())
        {
            await LoginPage.NavigateAsync();
        }
        await LoginPage.LoginAsOperatorAsync();
    }

    protected async Task AssertPageTitleContains(string expectedTitle)
    {
        var actualTitle = await Page.TitleAsync();
        Assert.That(actualTitle, Does.Contain(expectedTitle), 
            $"Expected page title to contain '{expectedTitle}', but was '{actualTitle}'");
    }

    protected Task AssertUrlContains(string expectedUrlPart)
    {
        var actualUrl = Page.Url;
        Assert.That(actualUrl, Does.Contain(expectedUrlPart),
            $"Expected URL to contain '{expectedUrlPart}', but was '{actualUrl}'");
        return Task.CompletedTask;
    }

    protected async Task AssertElementVisible(ILocator element, string elementDescription = "Element")
    {
        var isVisible = await element.IsVisibleAsync();
        Assert.That(isVisible, Is.True, $"{elementDescription} should be visible");
    }

    protected async Task AssertElementNotVisible(ILocator element, string elementDescription = "Element")
    {
        var isVisible = await element.IsVisibleAsync();
        Assert.That(isVisible, Is.False, $"{elementDescription} should not be visible");
    }

    protected async Task AssertElementHasText(ILocator element, string expectedText, string elementDescription = "Element")
    {
        var actualText = await element.TextContentAsync();
        Assert.That(actualText, Does.Contain(expectedText), 
            $"{elementDescription} should contain text '{expectedText}', but was '{actualText}'");
    }

    protected async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait for any loading spinners to disappear
        var loadingSpinner = Page.Locator(".spinner, .loading, [data-testid='loading']");
        try
        {
            await loadingSpinner.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5000 });
        }
        catch
        {
            // Ignore if no loading spinner is found
        }
    }

    /// <summary>
    /// Check if the application is running and accessible
    /// </summary>
    protected async Task<bool> IsApplicationRunningAsync()
    {
        try
        {
            await Page.GotoAsync(Config.BaseUrl, new() { Timeout = 5000 });
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 5000 });
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Assert that the application is running, skip test if not
    /// </summary>
    protected async Task RequireApplicationRunningAsync()
    {
        if (!await IsApplicationRunningAsync())
        {
            Assert.Ignore($"Application is not running at {Config.BaseUrl}. Skipping test.");
        }
    }
}
