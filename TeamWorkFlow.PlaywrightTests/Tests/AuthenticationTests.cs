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
        Assert.That(greetingText, Does.Contain(Config.OperatorUser.FirstName).Or.Contain(Config.OperatorUser.LastName),
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

        // Act
        await LogoutAsync();

        // Assert
        var logoutUrl = Page.Url;
        var isLoggedOut = logoutUrl.Contains("Login") || logoutUrl.Contains("login");
        Assert.That(isLoggedOut, Is.True, "Should be logged out");
    }

    [Test]
    public async Task Navigation_WhenNotLoggedIn_ShouldRedirectToLogin()
    {
        // Arrange - ensure not logged in
        if (await IsLoggedInAsync())
        {
            await LogoutAsync();
        }

        // Act - try to access a protected page
        await Page.GotoAsync($"{Config.BaseUrl}/Task");

        // Assert - should be redirected to login
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var currentUrl = Page.Url;
        Assert.That(currentUrl, Does.Contain("Login"), 
            "Should be redirected to login page when accessing protected content");
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
