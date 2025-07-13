using TeamWorkFlow.PlaywrightTests.PageObjects;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class TaskManagementTests : BaseTest
{
    [SetUp]
    public Task TaskTestSetUp()
    {
        // No automatic login - each test will handle authentication as needed
        TestContext.WriteLine("� TaskManagementTests setup completed - tests will handle authentication individually");
        return Task.CompletedTask;
    }

    [Test]
    public async Task TasksList_ShouldRequireAuthentication()
    {
        TestContext.WriteLine("🔐 Testing that tasks list requires authentication...");

        // Act - Try to access tasks page without authentication
        await TasksPage.NavigateToListAsync();

        // Assert - Should be redirected to login page
        var currentUrl = Page.Url;
        Assert.That(currentUrl, Does.Contain("/Identity/Account/Login"),
            "Unauthenticated users should be redirected to login page");

        // Verify return URL is set correctly for tasks page
        var hasReturnUrl = currentUrl.Contains("ReturnUrl=%2FTask") ||
                          currentUrl.Contains("ReturnUrl=/Task") ||
                          currentUrl.Contains("returnUrl=%2FTask") ||
                          currentUrl.Contains("returnUrl=/Task");

        Assert.That(hasReturnUrl, Is.True,
            "Login redirect should include return URL for tasks page");

        TestContext.WriteLine("✅ Tasks list correctly requires authentication - security working properly");
    }

    [Test]
    public async Task LoginWithFakeCredentials_ShouldFail()
    {
        TestContext.WriteLine("🔐 Testing login with fake admin credentials...");

        // Arrange - Navigate to login page
        await LoginPage.NavigateAsync();
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");

        // Act - Attempt login with fake admin credentials
        var fakeAdminEmail = Config.AdminUser.Email; // This is "fake.admin@test.local"
        var fakeAdminPassword = Config.AdminUser.Password; // This is "FakeAdminPass123!"


        TestContext.WriteLine($"🔐 Attempting login with fake admin: {fakeAdminEmail}");
        await LoginPage.LoginAsync(fakeAdminEmail, fakeAdminPassword);

        // Assert - Login should fail and stay on login page
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True,
            "Login with fake credentials should fail and remain on login page");

        // Verify error message is shown
        var hasLoginError = await LoginPage.HasLoginErrorAsync();
        Assert.That(hasLoginError, Is.True,
            "Failed login should display error message");

        TestContext.WriteLine("✅ Fake credentials correctly rejected - authentication security working properly");
    }

    [Test]
    public async Task LoginWithFakeOperatorCredentials_ShouldFail()
    {
        TestContext.WriteLine("🔐 Testing login with fake operator credentials...");

        // Arrange - Navigate to login page
        await LoginPage.NavigateAsync();
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");

        // Act - Attempt login with fake operator credentials
        var fakeOperatorEmail = Config.OperatorUser.Email; // This is "fake.operator@test.local"
        var fakeOperatorPassword = Config.OperatorUser.Password; // This is "FakeOperatorPass456!"

        TestContext.WriteLine($"🔐 Attempting login with fake operator: {fakeOperatorEmail}");
        await LoginPage.LoginAsync(fakeOperatorEmail, fakeOperatorPassword);

        // Assert - Login should fail and stay on login page
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True,
            "Login with fake operator credentials should fail and remain on login page");

        // Verify error message is shown
        var hasLoginError = await LoginPage.HasLoginErrorAsync();
        Assert.That(hasLoginError, Is.True,
            "Failed login should display error message");

        TestContext.WriteLine("✅ Fake operator credentials correctly rejected - authentication security working properly");
    }

    [Test]
    public async Task LoginForm_WithEmptyCredentials_ShouldShowValidation()
    {
        TestContext.WriteLine("📝 Testing login form validation with empty credentials...");

        // Arrange - Navigate to login page
        await LoginPage.NavigateAsync();
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");

        // Act - Try to submit empty form
        await LoginPage.LoginAsync("", "");

        // Assert - Should remain on login page with validation errors
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True,
            "Should remain on login page when submitting empty credentials");

        // Check for validation errors or that form doesn't submit
        var hasErrors = await LoginPage.HasLoginErrorAsync();
        var emailValid = await LoginPage.IsEmailFieldValidAsync();
        var passwordValid = await LoginPage.IsPasswordFieldValidAsync();

        // At least one validation should fail
        var hasValidation = hasErrors || !emailValid || !passwordValid;
        Assert.That(hasValidation, Is.True,
            "Empty credentials should trigger validation errors");

        TestContext.WriteLine("✅ Login form validation working correctly");
    }

    [Test]
    public async Task TaskCreatePage_ShouldRequireAuthentication()
    {
        TestContext.WriteLine("🔐 Testing that task creation requires authentication...");

        // Act - Try to access task creation page without authentication
        await TasksPage.NavigateToCreateAsync();

        // Assert - Should be redirected to login page
        var currentUrl = Page.Url;
        Assert.That(currentUrl, Does.Contain("/Identity/Account/Login"),
            "Unauthenticated users should be redirected to login page when accessing task creation");

        // Verify return URL is set correctly for task creation
        var hasReturnUrl = currentUrl.Contains("ReturnUrl=") &&
                          (currentUrl.Contains("Task") || currentUrl.Contains("Create"));

        Assert.That(hasReturnUrl, Is.True,
            "Login redirect should include return URL for task creation page");

        TestContext.WriteLine("✅ Task creation correctly requires authentication - security working properly");
    }

    [Test]
    public async Task LoginPage_ShouldLoadCorrectly()
    {
        TestContext.WriteLine("📄 Testing login page loads correctly...");

        // Act - Navigate to login page
        await LoginPage.NavigateAsync();

        // Assert - Should be on login page with all required elements
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");

        // Verify page title contains expected text
        var pageTitle = await Page.TitleAsync();
        Assert.That(pageTitle, Does.Contain("Log in").Or.Contain("Login").Or.Contain("TeamWorkFlow"),
            "Page title should indicate this is a login page");

        // Verify essential form elements are present
        var emailField = Page.Locator("input[name='Input.Email'], input[type='email']");
        var passwordField = Page.Locator("input[name='Input.Password'], input[type='password']");
        var loginButton = Page.Locator("button[type='submit']:has-text('Log in'), input[value*='Log in']");

        Assert.That(await emailField.IsVisibleAsync(), Is.True, "Email field should be visible");
        Assert.That(await passwordField.IsVisibleAsync(), Is.True, "Password field should be visible");
        Assert.That(await loginButton.IsVisibleAsync(), Is.True, "Login button should be visible");

        TestContext.WriteLine("✅ Login page loads correctly with all required elements");
    }

    [Test]
    public async Task LoginPage_ShouldBeResponsive()
    {
        TestContext.WriteLine("📱 Testing login page responsiveness...");

        // Arrange - Navigate to login page
        await LoginPage.NavigateAsync();
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");

        // Test mobile viewport
        await Page.SetViewportSizeAsync(375, 667); // iPhone SE size
        await Page.WaitForTimeoutAsync(500); // Allow layout to adjust

        // Assert - Form should still be usable on mobile
        var emailField = Page.Locator("input[name='Input.Email'], input[type='email']");
        var passwordField = Page.Locator("input[name='Input.Password'], input[type='password']");
        var loginButton = Page.Locator("button[type='submit']:has-text('Log in'), input[value*='Log in']");

        Assert.That(await emailField.IsVisibleAsync(), Is.True, "Email field should be visible on mobile");
        Assert.That(await passwordField.IsVisibleAsync(), Is.True, "Password field should be visible on mobile");
        Assert.That(await loginButton.IsVisibleAsync(), Is.True, "Login button should be visible on mobile");

        // Test desktop viewport
        await Page.SetViewportSizeAsync(1920, 1080);
        await Page.WaitForTimeoutAsync(500); // Allow layout to adjust

        Assert.That(await emailField.IsVisibleAsync(), Is.True, "Email field should be visible on desktop");
        Assert.That(await passwordField.IsVisibleAsync(), Is.True, "Password field should be visible on desktop");
        Assert.That(await loginButton.IsVisibleAsync(), Is.True, "Login button should be visible on desktop");

        TestContext.WriteLine("✅ Login page is responsive across different screen sizes");
    }

    [Test]
    public async Task LoginForm_ShouldHaveProperInputTypes()
    {
        TestContext.WriteLine("🔍 Testing login form input types and attributes...");

        // Arrange - Navigate to login page
        await LoginPage.NavigateAsync();
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");

        // Act & Assert - Check email field
        var emailField = Page.Locator("input[name='Input.Email'], input[type='email']");
        Assert.That(await emailField.IsVisibleAsync(), Is.True, "Email field should be visible");

        var emailType = await emailField.GetAttributeAsync("type");
        Assert.That(emailType, Is.EqualTo("email").Or.EqualTo("text"),
            "Email field should have appropriate input type");

        // Check password field
        var passwordField = Page.Locator("input[name='Input.Password'], input[type='password']");
        Assert.That(await passwordField.IsVisibleAsync(), Is.True, "Password field should be visible");

        var passwordType = await passwordField.GetAttributeAsync("type");
        Assert.That(passwordType, Is.EqualTo("password"),
            "Password field should have password input type");

        // Check form method
        var form = Page.Locator("form");
        var formMethod = await form.GetAttributeAsync("method");
        Assert.That(formMethod?.ToLower(), Is.EqualTo("post"),
            "Login form should use POST method for security");

        TestContext.WriteLine("✅ Login form has proper input types and security attributes");
    }

    [Test]
    public async Task LoginAttempt_WithInvalidEmail_ShouldFail()
    {
        TestContext.WriteLine("📧 Testing login with invalid email format...");

        // Arrange - Navigate to login page
        await LoginPage.NavigateAsync();
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True, "Should be on login page");

        // Act - Try to login with invalid email format
        var invalidEmail = "not-an-email";
        var somePassword = "SomePassword123!";

        await LoginPage.LoginAsync(invalidEmail, somePassword);

        // Assert - Should remain on login page
        Assert.That(await LoginPage.IsOnLoginPageAsync(), Is.True,
            "Should remain on login page with invalid email format");

        // Check for validation or error
        var hasError = await LoginPage.HasLoginErrorAsync();
        var emailValid = await LoginPage.IsEmailFieldValidAsync();

        var hasValidation = hasError || !emailValid;
        Assert.That(hasValidation, Is.True,
            "Invalid email format should trigger validation or error");

        TestContext.WriteLine("✅ Invalid email format correctly handled");
    }

    [Test]
    public async Task LoginRedirect_ShouldPreserveReturnUrl()
    {
        TestContext.WriteLine("🔄 Testing login redirect preserves return URL...");

        // Act - Try to access a protected page directly
        var protectedUrl = $"{Config.BaseUrl}/Task";
        await Page.GotoAsync(protectedUrl);

        // Assert - Should be redirected to login with return URL
        var currentUrl = Page.Url;
        Assert.That(currentUrl, Does.Contain("/Identity/Account/Login"),
            "Should be redirected to login page");

        // Verify return URL is preserved
        var hasReturnUrl = currentUrl.Contains("ReturnUrl=") || currentUrl.Contains("returnUrl=");
        Assert.That(hasReturnUrl, Is.True,
            "Login redirect should preserve the original URL as return URL");

        // Verify the return URL contains the original path
        var containsTaskPath = currentUrl.Contains("%2FTask") ||
                              currentUrl.Contains("/Task") ||
                              currentUrl.Contains("Task");
        Assert.That(containsTaskPath, Is.True,
            "Return URL should contain the original task path");

        TestContext.WriteLine("✅ Login redirect correctly preserves return URL for post-login navigation");
    }

    [Test]
    public async Task ApplicationSecurity_ShouldBlockUnauthorizedAccess()
    {
        TestContext.WriteLine("🛡️ Testing application security blocks unauthorized access...");

        // Test multiple protected endpoints
        var protectedEndpoints = new[]
        {
            "/Task",
            "/Task/Create",
            "/Task/Edit/1",
            "/Task/Details/1",
            "/Project",
            "/Operator"
        };

        foreach (var endpoint in protectedEndpoints)
        {
            TestContext.WriteLine($"🔒 Testing protection for: {endpoint}");

            // Act - Try to access protected endpoint
            await Page.GotoAsync($"{Config.BaseUrl}{endpoint}");

            // Assert - Should be redirected to login
            var currentUrl = Page.Url;
            Assert.That(currentUrl, Does.Contain("/Identity/Account/Login"),
                $"Endpoint {endpoint} should redirect to login page");
        }

        TestContext.WriteLine("✅ All protected endpoints correctly require authentication - security working properly");
    }
}
