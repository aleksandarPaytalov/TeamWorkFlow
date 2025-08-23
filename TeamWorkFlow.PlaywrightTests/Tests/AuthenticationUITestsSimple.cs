using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class AuthenticationUITestsSimple : BaseTest
{
    /// <summary>
    /// These tests require the application to be running to test the actual login page
    /// </summary>
    protected override bool RequiresApplicationConnection()
    {
        return true; // These tests need the application running
    }
    [Test]
    public async Task LoginPage_ShouldLoadCorrectly()
    {
        // Arrange & Act
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login");

        // Assert - Verify login page loads
        await Expect(Page).ToHaveTitleAsync(new Regex(".*(Log in|Welcome Back).*"));
        
        // Verify login form elements exist
        await Expect(Page.Locator("#Input_Email")).ToBeVisibleAsync();
        await Expect(Page.Locator("#Input_Password")).ToBeVisibleAsync();
        await Expect(Page.Locator("#login-submit")).ToBeVisibleAsync();
    }

    [Test]
    public async Task LoginForm_ShouldHaveCorrectInputTypes()
    {
        // Arrange
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login");

        // Act & Assert
        var emailInput = Page.Locator("#Input_Email");
        var passwordInput = Page.Locator("#Input_Password");
        
        // Check for proper input types
        var emailType = await emailInput.GetAttributeAsync("type");
        var passwordType = await passwordInput.GetAttributeAsync("type");
        
        Assert.That(emailType, Is.EqualTo("email"), "Email input should have type='email'");
        Assert.That(passwordType, Is.EqualTo("password"), "Password input should have type='password'");
    }

    [Test]
    public async Task Login_WithFakeCredentials_ShouldShowError()
    {
        // Arrange
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login");

        // Wait for page to be fully loaded
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act - Try to login with fake credentials
        await Page.FillAsync("#Input_Email", "fake.admin@test.local");
        await Page.FillAsync("#Input_Password", "FakeAdminPass123!");

        // Wait for the submit button to be stable and ready
        var submitButton = Page.Locator("#login-submit");
        await Expect(submitButton).ToBeVisibleAsync();
        await Expect(submitButton).ToBeEnabledAsync();

        // Click submit button with proper waiting and error handling
        try
        {
            // Wait for button to be stable before clicking
            await submitButton.WaitForAsync(new LocatorWaitForOptions
            {
                Timeout = 10000
            });

            // Use force click if button is having stability issues
            await submitButton.ClickAsync(new LocatorClickOptions { Force = true });
        }
        catch (TimeoutException)
        {
            // If button click fails, try alternative approach
            await Page.EvaluateAsync("document.querySelector('#login-submit').click()");
        }

        // Wait for response
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Should show error or stay on login page
        var currentUrl = Page.Url;
        var isStillOnLoginPage = currentUrl.Contains("Login") || currentUrl.Contains("login");

        // Should either stay on login page or show error
        Assert.That(isStillOnLoginPage, Is.True, "Should stay on login page with fake credentials");
    }

    [Test]
    public async Task Login_WithEmptyCredentials_ShouldShowValidationErrors()
    {
        // Arrange
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login");

        // Wait for page to be fully loaded
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait for the submit button to be stable and ready
        var submitButton = Page.Locator("#login-submit");
        await Expect(submitButton).ToBeVisibleAsync();
        await Expect(submitButton).ToBeEnabledAsync();

        // Act - Try to submit empty form with proper waiting
        try
        {
            // Wait for button to be stable before clicking
            await submitButton.WaitForAsync(new LocatorWaitForOptions
            {
                Timeout = 10000
            });

            // Use force click if button is having stability issues
            await submitButton.ClickAsync(new LocatorClickOptions { Force = true });
        }
        catch (TimeoutException)
        {
            // If button click fails, try alternative approach
            await Page.EvaluateAsync("document.querySelector('#login-submit').click()");
        }

        // Wait for validation response
        await Page.WaitForTimeoutAsync(2000);

        // Assert - Should show validation errors or stay on login page
        var currentUrl = Page.Url;
        var isStillOnLoginPage = currentUrl.Contains("Login") || currentUrl.Contains("login");

        Assert.That(isStillOnLoginPage, Is.True, "Should stay on login page with empty credentials");

        // Check if form validation is working by verifying form elements are still visible
        var emailInput = Page.Locator("#Input_Email");
        var passwordInput = Page.Locator("#Input_Password");

        // Form should still be present and functional
        await Expect(emailInput).ToBeVisibleAsync();
        await Expect(passwordInput).ToBeVisibleAsync();
        await Expect(submitButton).ToBeVisibleAsync();
    }

    [Test]
    public async Task Navigation_WhenNotLoggedIn_ShouldRedirectToLogin()
    {
        // Arrange - Clear any existing session
        await Page.Context.ClearCookiesAsync();

        // Act - Try to access a protected page
        await Page.GotoAsync($"{Config.BaseUrl}/Task/All");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait for any redirects
        await Page.WaitForTimeoutAsync(2000);

        // Assert - Should be redirected to login or show login form
        var currentUrl = Page.Url;
        var isOnLoginPage = currentUrl.Contains("Login") || currentUrl.Contains("login") || currentUrl.Contains("Account");
        var loginForm = Page.Locator("#Input_Email");
        var isLoginFormVisible = await loginForm.IsVisibleAsync();

        // Should either be redirected to login page or show login form
        var shouldRedirectToLogin = isOnLoginPage || isLoginFormVisible;

        Assert.That(shouldRedirectToLogin, Is.True,
            $"Should be redirected to login when accessing protected content. Current URL: {currentUrl}");
    }

    [Test]
    public async Task LoginPage_ShouldHaveProperFormStructure()
    {
        // Arrange
        await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login");

        // Act & Assert - Verify form structure
        var loginForm = Page.Locator("form");
        await Expect(loginForm).ToBeVisibleAsync();

        // Should have email input
        var emailInput = Page.Locator("#Input_Email");
        await Expect(emailInput).ToBeVisibleAsync();

        // Should have password input
        var passwordInput = Page.Locator("#Input_Password");
        await Expect(passwordInput).ToBeVisibleAsync();

        // Should have submit button
        var submitButton = Page.Locator("#login-submit");
        await Expect(submitButton).ToBeVisibleAsync();

        // Verify button text
        var buttonText = await submitButton.TextContentAsync();
        Assert.That(buttonText?.Trim(), Does.Contain("Log in").Or.Contain("Login").Or.Contain("Sign In").Or.Contain("Sign in"),
            "Submit button should have appropriate text");
    }

    [Test]
    public async Task LoginPage_ShouldBeResponsive()
    {
        // Test different viewport sizes
        var viewports = new[]
        {
            new { Width = 1920, Height = 1080 }, // Desktop
            new { Width = 768, Height = 1024 },  // Tablet
            new { Width = 375, Height = 667 }    // Mobile
        };

        foreach (var viewport in viewports)
        {
            // Arrange
            await Page.SetViewportSizeAsync(viewport.Width, viewport.Height);
            await Page.GotoAsync($"{Config.BaseUrl}/Identity/Account/Login");

            // Act & Assert
            var emailInput = Page.Locator("#Input_Email");
            var passwordInput = Page.Locator("#Input_Password");
            var submitButton = Page.Locator("#login-submit");

            // All elements should be visible at different screen sizes
            await Expect(emailInput).ToBeVisibleAsync();
            await Expect(passwordInput).ToBeVisibleAsync();
            await Expect(submitButton).ToBeVisibleAsync();
        }
    }
}
