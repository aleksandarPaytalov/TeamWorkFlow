using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class QuickTest : PageTest
{
    private TestConfiguration Config { get; set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Config = TestConfiguration.Instance;
    }

    [Test]
    public async Task QuickLogin_ShouldWork()
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
        
        Console.WriteLine($"Current URL after login: {currentUrl}");
        Console.WriteLine($"Is logged in: {isLoggedIn}");
        
        // Check for user indicators
        var logoutButton = Page.Locator("a[href*='logout'], button:has-text('Logout')");
        var userDropdown = Page.Locator(".navbar .dropdown-toggle, .navbar-text");
        var userGreeting = Page.Locator("a[title='Manage Account']");
        
        var hasLogoutButton = await logoutButton.IsVisibleAsync();
        var hasUserDropdown = await userDropdown.IsVisibleAsync();
        var hasUserGreeting = await userGreeting.IsVisibleAsync();
        
        Console.WriteLine($"Has logout button: {hasLogoutButton}");
        Console.WriteLine($"Has user dropdown: {hasUserDropdown}");
        Console.WriteLine($"Has user greeting: {hasUserGreeting}");
        
        // Check what elements are actually present in the navbar
        var navbarElements = await Page.Locator("nav *").AllTextContentsAsync();
        Console.WriteLine("Navbar elements:");
        foreach (var element in navbarElements.Take(10)) // Show first 10 elements
        {
            if (!string.IsNullOrWhiteSpace(element))
                Console.WriteLine($"  - {element}");
        }
        
        Assert.That(isLoggedIn, Is.True, "Should be logged in and redirected away from login page");
        Assert.That(hasLogoutButton || hasUserDropdown || hasUserGreeting, Is.True, 
            "Should have some user indicator visible");
    }

    [Test]
    public async Task CheckNavbarStructure_AfterLogin()
    {
        // Login first
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login");
        
        var emailInput = Page.Locator("input[type='email']").First;
        var passwordInput = Page.Locator("input[type='password']").First;
        var loginButton = Page.Locator("button[type='submit']").First;
        
        await emailInput.FillAsync(Config.AdminUser.Email);
        await passwordInput.FillAsync(Config.AdminUser.Password);
        await loginButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Take a screenshot to see what the page looks like
        await Page.ScreenshotAsync(new() { Path = "screenshots/after_login_debug.png", FullPage = true });
        
        // Get all navbar content
        var navbar = Page.Locator("nav, .navbar");
        if (await navbar.IsVisibleAsync())
        {
            var navbarHtml = await navbar.InnerHTMLAsync();
            Console.WriteLine("Navbar HTML structure:");
            Console.WriteLine(navbarHtml.Substring(0, Math.Min(500, navbarHtml.Length)) + "...");
        }
        
        // Check for common user indicators
        var possibleUserElements = new[]
        {
            ".navbar-text",
            ".user-greeting", 
            ".dropdown-toggle",
            "a[href*='logout']",
            "button:has-text('Logout')",
            ".navbar .btn",
            ".navbar a:has-text('Admin')",
            ".navbar a:has-text('Logout')"
        };
        
        foreach (var selector in possibleUserElements)
        {
            var element = Page.Locator(selector);
            var isVisible = await element.IsVisibleAsync();
            var count = await element.CountAsync();
            Console.WriteLine($"{selector}: visible={isVisible}, count={count}");
            
            if (isVisible && count > 0)
            {
                var text = await element.First.TextContentAsync();
                Console.WriteLine($"  Text: '{text}'");
            }
        }
        
        Assert.Pass("Debug test completed - check console output");
    }
}
