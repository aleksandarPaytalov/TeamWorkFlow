using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class ConnectivityTest : PageTest
{
    private TestConfiguration Config { get; set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Config = TestConfiguration.Instance;
    }

    [Test]
    public async Task Application_ShouldBeAccessible()
    {
        // Act
        await Page.GotoAsync(Config.BaseUrl, new() { Timeout = 10000 });
        
        // Assert
        var title = await Page.TitleAsync();
        Assert.That(title, Is.Not.Empty, "Page should have a title");
        
        var url = Page.Url;
        Assert.That(url, Does.StartWith(Config.BaseUrl), "Should be on the correct domain");
        
        Console.WriteLine($"✅ Successfully connected to: {url}");
        Console.WriteLine($"📄 Page title: {title}");
    }

    [Test]
    public async Task LoginPage_ShouldBeAccessible()
    {
        // Act
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login", new() { Timeout = 10000 });
        
        // Assert
        var emailInput = Page.Locator("input[type='email'], input[name*='Email']");
        var passwordInput = Page.Locator("input[type='password'], input[name*='Password']");
        
        await emailInput.WaitForAsync(new() { Timeout = 5000 });
        await passwordInput.WaitForAsync(new() { Timeout = 5000 });
        
        Assert.That(await emailInput.IsVisibleAsync(), Is.True, "Email input should be visible");
        Assert.That(await passwordInput.IsVisibleAsync(), Is.True, "Password input should be visible");
        
        Console.WriteLine("✅ Login page is accessible and has required form fields");
    }

    [Test]
    public async Task BasicLogin_ShouldWork()
    {
        // Arrange
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login", new() { Timeout = 10000 });
        
        var emailInput = Page.Locator("input[type='email'], input[name*='Email']").First;
        var passwordInput = Page.Locator("input[type='password'], input[name*='Password']").First;
        var loginButton = Page.Locator("button[type='submit'], input[type='submit']").First;
        
        // Act
        await emailInput.FillAsync(Config.AdminUser.Email);
        await passwordInput.FillAsync(Config.AdminUser.Password);
        await loginButton.ClickAsync();
        
        // Wait for navigation
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });
        
        // Assert
        var currentUrl = Page.Url;
        var isLoggedIn = !currentUrl.Contains("Login") && !currentUrl.Contains("login");
        
        if (isLoggedIn)
        {
            Console.WriteLine($"✅ Successfully logged in! Redirected to: {currentUrl}");
        }
        else
        {
            Console.WriteLine($"❌ Login failed. Still on: {currentUrl}");
            
            // Check for error messages
            var errorMessages = await Page.Locator(".alert-danger, .validation-summary-errors, .field-validation-error").AllTextContentsAsync();
            if (errorMessages.Any())
            {
                Console.WriteLine("Error messages found:");
                foreach (var error in errorMessages)
                {
                    Console.WriteLine($"  - {error}");
                }
            }
        }
        
        Assert.That(isLoggedIn, Is.True, "Should be logged in and redirected away from login page");
    }
}
