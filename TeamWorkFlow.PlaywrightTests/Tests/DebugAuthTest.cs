using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class DebugAuthTest : PageTest
{
    private TestConfiguration Config { get; set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Config = TestConfiguration.Instance;
    }

    [Test]
    public async Task DebugLogin_StepByStep()
    {
        Console.WriteLine("🔍 Starting debug login test...");
        
        // Step 1: Navigate to login page
        var loginUrl = $"{Config.BaseUrl}/Identity/Account/Login";
        Console.WriteLine($"📍 Navigating to: {loginUrl}");
        await Page.GotoAsync(loginUrl);
        
        var initialUrl = Page.Url;
        Console.WriteLine($"📍 Current URL after navigation: {initialUrl}");
        
        // Step 2: Fill login form
        Console.WriteLine("📝 Filling login form...");
        var emailInput = Page.Locator("input[type='email'], input[name*='Email']").First;
        var passwordInput = Page.Locator("input[type='password'], input[name*='Password']").First;
        var loginButton = Page.Locator("button[type='submit'], input[type='submit']").First;
        
        await emailInput.FillAsync(Config.AdminUser.Email);
        await passwordInput.FillAsync(Config.AdminUser.Password);
        
        Console.WriteLine($"📧 Email filled: {Config.AdminUser.Email}");
        Console.WriteLine("🔒 Password filled: [HIDDEN]");
        
        // Step 3: Click login button
        Console.WriteLine("🖱️ Clicking login button...");
        await loginButton.ClickAsync();
        
        // Step 4: Wait a bit and check URL
        Console.WriteLine("⏳ Waiting 2 seconds...");
        await Page.WaitForTimeoutAsync(2000);
        
        var urlAfterClick = Page.Url;
        Console.WriteLine($"📍 URL after login click: {urlAfterClick}");
        
        // Step 5: Wait for network idle
        Console.WriteLine("🌐 Waiting for network idle...");
        try
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });
            var urlAfterNetworkIdle = Page.Url;
            Console.WriteLine($"📍 URL after network idle: {urlAfterNetworkIdle}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Network idle timeout: {ex.Message}");
        }
        
        // Step 6: Check for validation errors
        var validationErrors = Page.Locator(".validation-summary-errors, .field-validation-error, .alert-danger");
        var errorCount = await validationErrors.CountAsync();
        Console.WriteLine($"❌ Validation errors found: {errorCount}");
        
        if (errorCount > 0)
        {
            for (int i = 0; i < errorCount; i++)
            {
                var errorText = await validationErrors.Nth(i).TextContentAsync();
                Console.WriteLine($"   Error {i + 1}: {errorText}");
            }
        }
        
        // Step 7: Check current page content
        var pageTitle = await Page.TitleAsync();
        Console.WriteLine($"📄 Page title: {pageTitle}");
        
        // Step 8: Look for user indicators
        var userGreeting = Page.Locator("a[title='Manage Account']");
        var userGreetingVisible = await userGreeting.IsVisibleAsync();
        Console.WriteLine($"👤 User greeting visible: {userGreetingVisible}");
        
        if (userGreetingVisible)
        {
            var greetingText = await userGreeting.TextContentAsync();
            Console.WriteLine($"👤 User greeting text: '{greetingText}'");
        }
        
        // Step 9: Check for login form still present
        var emailInputStillVisible = await emailInput.IsVisibleAsync();
        var passwordInputStillVisible = await passwordInput.IsVisibleAsync();
        Console.WriteLine($"📧 Email input still visible: {emailInputStillVisible}");
        Console.WriteLine($"🔒 Password input still visible: {passwordInputStillVisible}");
        
        // Step 10: Final URL check
        var finalUrl = Page.Url;
        var isLoggedIn = !finalUrl.Contains("Login") && !finalUrl.Contains("login");
        Console.WriteLine($"📍 Final URL: {finalUrl}");
        Console.WriteLine($"✅ Is logged in (URL check): {isLoggedIn}");
        
        // Step 11: Take screenshot for analysis
        await Page.ScreenshotAsync(new() { Path = "screenshots/debug_login_final.png", FullPage = true });
        Console.WriteLine("📸 Screenshot saved: screenshots/debug_login_final.png");
        
        // Step 12: Get page HTML snippet
        var bodyContent = await Page.Locator("body").InnerHTMLAsync();
        var snippet = bodyContent.Length > 500 ? bodyContent.Substring(0, 500) + "..." : bodyContent;
        Console.WriteLine($"📄 Page content snippet: {snippet}");
        
        // Don't assert anything - just gather information
        Console.WriteLine("🔍 Debug test completed - check console output for details");
    }
}
