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

        // Set up browser context options
        Page.SetDefaultTimeout(Config.Timeout);
        
        // Navigate to base URL to ensure the application is running
        await Page.GotoAsync(Config.BaseUrl);
        
        // Wait for the page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
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
            var userGreeting = Page.Locator("[data-testid='user-greeting'], .navbar-text");
            await userGreeting.WaitForAsync(new() { Timeout = 3000 });
            return true;
        }
        catch
        {
            return false;
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
}
