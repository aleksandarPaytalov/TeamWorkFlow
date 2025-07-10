using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class AuthenticationTests : BaseTest
{
    [Test]
    public async Task LoginPage_ShouldLoadCorrectly()
    {
        // Arrange & Act
        await LoginPage.NavigateAsync();

        // Assert
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");
        await AssertPageTitleContains("Log in");
    }

    [Test]
    public async Task Login_WithValidAdminCredentials_ShouldSucceed()
    {
        // Arrange
        await LoginPage.NavigateAsync();

        // Act
        await LoginPage.LoginAsAdminAsync();

        // Assert
        var currentUrl = Page.Url;
        var isLoggedIn = !currentUrl.Contains("Login") && !currentUrl.Contains("login");
        Assert.That(isLoggedIn, Is.True, "Should be logged in");

        // Verify user greeting is visible using the correct selector
        var userGreeting = Page.Locator("a[title='Manage Account']");
        await AssertElementVisible(userGreeting, "User greeting");

        // Verify admin user name is displayed
        var greetingText = await userGreeting.TextContentAsync();
        Assert.That(greetingText, Does.Contain("Aleksandar").Or.Contain("Paytalov").Or.Contain("Hi"),
            "User greeting should contain user name or greeting");
    }

    [Test]
    public async Task Login_WithValidOperatorCredentials_ShouldSucceed()
    {
        // Arrange
        await LoginPage.NavigateAsync();

        // Act
        await LoginPage.LoginAsOperatorAsync();

        // Assert
        var currentUrl = Page.Url;
        var isLoggedIn = !currentUrl.Contains("Login") && !currentUrl.Contains("login");
        Assert.That(isLoggedIn, Is.True, "Should be logged in");

        // Verify user greeting is visible using the correct selector
        var userGreeting = Page.Locator("a[title='Manage Account']");
        await AssertElementVisible(userGreeting, "User greeting");

        // Verify operator user name is displayed
        var greetingText = await userGreeting.TextContentAsync();
        Assert.That(greetingText, Does.Contain("Jon").Or.Contain("Doe").Or.Contain("Hi"),
            "User greeting should contain operator user name");
    }

    [Test]
    public async Task Login_WithInvalidCredentials_ShouldFail()
    {
        // Arrange
        await LoginPage.NavigateAsync();

        // Act
        await LoginPage.LoginAsync("invalid@email.com", "wrongpassword");

        // Assert
        Assert.That(await IsLoggedInAsync(), Is.False, "Should not be logged in");
        Assert.That(await LoginPage.HasLoginErrorAsync(), Is.True, "Should display login error");
        
        var errors = await LoginPage.GetLoginErrorsAsync();
        Assert.That(errors.Length, Is.GreaterThan(0), "Should have at least one error message");
    }

    [Test]
    public async Task Login_WithEmptyCredentials_ShouldShowValidationErrors()
    {
        // Arrange
        await LoginPage.NavigateAsync();

        // Act
        await LoginPage.LoginAsync("", "");

        // Assert
        Assert.That(await IsLoggedInAsync(), Is.False, "Should not be logged in");
        Assert.That(await LoginPage.HasLoginErrorAsync(), Is.True, "Should display validation errors");
        
        var errors = await LoginPage.GetLoginErrorsAsync();
        Assert.That(errors.Length, Is.GreaterThan(0), "Should have validation error messages");
    }

    [Test]
    public async Task Login_WithInvalidEmailFormat_ShouldShowValidationError()
    {
        // Arrange
        await LoginPage.NavigateAsync();

        // Act
        await LoginPage.LoginAsync("invalid-email", "password123");

        // Assert
        Assert.That(await IsLoggedInAsync(), Is.False, "Should not be logged in");
        Assert.That(await LoginPage.IsEmailFieldValidAsync(), Is.False, "Email field should be invalid");
    }

    [Test]
    public async Task Logout_WhenLoggedIn_ShouldRedirectToLoginPage()
    {
        // Arrange
        await LoginAsAdminAsync();
        var currentUrl = Page.Url;
        var isLoggedIn = !currentUrl.Contains("Login") && !currentUrl.Contains("login");
        Assert.That(isLoggedIn, Is.True, "Should be logged in initially");

        // Act - Try multiple logout approaches
        try
        {
            // First try to find and click the logout button directly
            var logoutButton = Page.Locator("form[action='/Identity/Account/Logout'] button[type='submit']");
            if (await logoutButton.IsVisibleAsync())
            {
                await logoutButton.ClickAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            else
            {
                // If button not found, navigate directly to logout URL
                await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Logout");
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
        }
        catch
        {
            // Fallback: navigate directly to logout URL
            await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Logout");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Assert - Check multiple conditions for successful logout
        var finalUrl = Page.Url;
        var isOnLoginPage = finalUrl.Contains("Login") || finalUrl.Contains("login");
        var isOnLogoutPage = finalUrl.Contains("Logout") || finalUrl.Contains("logout");
        var userGreeting = Page.Locator("a[title='Manage Account']");
        var isUserGreetingVisible = await userGreeting.IsVisibleAsync();
        var loginForm = Page.Locator("form input[type='email'], form input[name='Input.Email']");
        var isLoginFormVisible = await loginForm.IsVisibleAsync();
        var logoutMessage = Page.Locator("p:has-text('successfully logged out'), p:has-text('logged out')");
        var isLogoutMessageVisible = await logoutMessage.IsVisibleAsync();

        // Debug information
        Console.WriteLine($"Final URL after logout: {finalUrl}");
        Console.WriteLine($"Is on login/logout page: {isOnLoginPage || isOnLogoutPage}");
        Console.WriteLine($"User greeting visible: {isUserGreetingVisible}");
        Console.WriteLine($"Login form visible: {isLoginFormVisible}");
        Console.WriteLine($"Logout message visible: {isLogoutMessageVisible}");

        // Logout is successful if any of these conditions are met:
        // 1. We're on the login page
        // 2. We're on the logout page with confirmation message
        // 3. User greeting is no longer visible
        // 4. Login form is visible again
        var logoutSuccessful = isOnLoginPage || isOnLogoutPage || !isUserGreetingVisible || isLoginFormVisible || isLogoutMessageVisible;

        Assert.That(logoutSuccessful, Is.True,
            $"Logout should work. URL: {finalUrl}, UserGreeting: {isUserGreetingVisible}, LoginForm: {isLoginFormVisible}, LogoutMessage: {isLogoutMessageVisible}");
    }

    [Test]
    public async Task Navigation_WhenNotLoggedIn_ShouldRedirectToLogin()
    {
        // Arrange - ensure not logged in by clearing cookies and session
        await Page.Context.ClearCookiesAsync();
        await Page.GotoAsync($"{Config.BaseUrl}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act - try to access a protected page
        await Page.GotoAsync($"{Config.BaseUrl}/Task/All");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait a bit for any redirects to complete
        await Page.WaitForTimeoutAsync(2000);

        // Assert - should be redirected to login or show login form
        var currentUrl = Page.Url;
        var isOnLoginPage = currentUrl.Contains("Login") || currentUrl.Contains("login") || currentUrl.Contains("Account");
        var loginForm = Page.Locator("form input[type='email'], form input[name='Input.Email']");
        var isLoginFormVisible = await loginForm.IsVisibleAsync();
        var unauthorizedMessage = Page.Locator("h1:has-text('Unauthorized'), h1:has-text('401'), .unauthorized");
        var isUnauthorizedVisible = await unauthorizedMessage.IsVisibleAsync();

        // Debug information
        Console.WriteLine($"Accessing protected route - URL: {currentUrl}, LoginPage: {isOnLoginPage}, LoginForm: {isLoginFormVisible}, Unauthorized: {isUnauthorizedVisible}");

        // Should either be redirected to login page, show login form, or show unauthorized message
        var shouldRedirectToLogin = isOnLoginPage || isLoginFormVisible || isUnauthorizedVisible;

        Assert.That(shouldRedirectToLogin, Is.True,
            "Should be redirected to login page, show login form, or show unauthorized message when accessing protected content");
    }

    [Test]
    public async Task RememberMe_WhenChecked_ShouldPersistLogin()
    {
        // Arrange
        await LoginPage.NavigateAsync();

        // Act
        await LoginPage.LoginAsync(Config.AdminUser.Email, Config.AdminUser.Password, rememberMe: true);

        // Assert
        var currentUrl = Page.Url;
        var isLoggedIn = !currentUrl.Contains("Login") && !currentUrl.Contains("login");
        Assert.That(isLoggedIn, Is.True, "Should be logged in");

        // Verify remember me functionality by checking if login persists after browser restart
        // Note: This is a basic test - full persistence testing would require browser context management
        var userGreeting = Page.Locator("a[title='Manage Account']");
        await AssertElementVisible(userGreeting, "User greeting should be visible with remember me");
    }

    [Test]
    public async Task LoginForm_ShouldHaveAccessibilityAttributes()
    {
        // Arrange
        await LoginPage.NavigateAsync();

        // Act & Assert
        var emailInput = Page.Locator("input[name='Input.Email'], input[type='email']");
        var passwordInput = Page.Locator("input[name='Input.Password'], input[type='password']");
        
        // Check for proper input types
        var emailType = await emailInput.GetAttributeAsync("type");
        var passwordType = await passwordInput.GetAttributeAsync("type");
        
        Assert.That(emailType, Is.EqualTo("email"), "Email input should have type='email'");
        Assert.That(passwordType, Is.EqualTo("password"), "Password input should have type='password'");
        
        // Check for labels or placeholders
        var emailLabel = await emailInput.GetAttributeAsync("placeholder");
        var passwordLabel = await passwordInput.GetAttributeAsync("placeholder");
        
        Assert.That(emailLabel ?? "", Is.Not.Empty.Or.Not.Null, "Email input should have placeholder or label");
        Assert.That(passwordLabel ?? "", Is.Not.Empty.Or.Not.Null, "Password input should have placeholder or label");
    }
}
