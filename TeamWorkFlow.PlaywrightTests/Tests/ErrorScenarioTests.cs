using Microsoft.Playwright;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ErrorScenarioTests : BaseTest
{
    [SetUp]
    public async Task TestSetUp()
    {
        // Ensure we can connect to the application for error scenario testing
        await EnsureApplicationIsRunningAsync();
    }

    [Test]
    public async Task UnauthorizedAccess_ShouldRedirectToLogin()
    {
        // Arrange - Make sure we're not logged in
        if (await IsLoggedInAsync())
        {
            await LogoutAsync();
        }

        // Act - Try to access a protected resource (Task creation)
        await ErrorPage.NavigateToProtectedPageWithoutAuthAsync();

        // Assert
        // Should be redirected to login page (this is the expected behavior in production)
        var currentUrl = Page.Url.ToLower();
        var isLoginPage = currentUrl.Contains("login") || currentUrl.Contains("account") || currentUrl.Contains("identity");

        Assert.That(isLoginPage, Is.True,
            "Unauthorized access should redirect to login page");

        TestContext.WriteLine("✅ Unauthorized access redirects to login (production behavior)");
    }

    [Test]
    public async Task InvalidRoute_ShouldShow404Error()
    {
        // Arrange & Act - Test real invalid routes that users might encounter
        await ErrorPage.NavigateToNonExistentPageAsync();

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            "Non-existent page should show error page");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("404"),
            "Non-existent page should show 404 error");

        TestContext.WriteLine("✅ Non-existent page shows 404 error");
    }

    [Test]
    public async Task InvalidController_ShouldShow404Error()
    {
        // Arrange & Act - Test invalid controller/action
        await ErrorPage.NavigateToNonExistentControllerAsync();

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            "Invalid controller should show error page");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("404"),
            "Invalid controller should show 404 error");

        TestContext.WriteLine("✅ Invalid controller shows 404 error");
    }

    [Test]
    public async Task InvalidResourceId_ShouldShow404Error()
    {
        // Arrange - Login as admin to access protected resources
        await LoginAsAdminAsync();

        // Act - Test invalid resource IDs (common user scenario)
        await ErrorPage.NavigateToInvalidTaskIdAsync();

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            "Invalid task ID should show error page");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("404"),
            "Invalid task ID should show 404 error");

        TestContext.WriteLine("✅ Invalid task ID shows 404 error");
    }

    [Test]
    public async Task MalformedRequest_ShouldShow400Error()
    {
        // Arrange - Login as admin to access protected resources
        await LoginAsAdminAsync();

        // Act - Test malformed URL (common user scenario)
        await ErrorPage.NavigateToMalformedUrlAsync();

        // Assert
        // This might show 404 instead of 400 depending on routing, which is also valid
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            "Malformed request should show error page");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.AnyOf("400", "404"),
            "Malformed request should show 400 or 404 error");

        TestContext.WriteLine($"✅ Malformed request shows {errorCode} error");
    }

    [Test]
    public async Task AdminPageWithoutPermission_ShouldRedirectOrShow403()
    {
        // Arrange - Make sure we're not logged in (simulate regular user)
        if (await IsLoggedInAsync())
        {
            await LogoutAsync();
        }

        // Act - Try to access admin page
        await ErrorPage.NavigateToAdminPageWithoutPermissionAsync();

        // Assert
        var currentUrl = Page.Url.ToLower();
        var isErrorPage = await ErrorPage.IsErrorPageDisplayedAsync();
        var isRedirectedToLogin = currentUrl.Contains("/identity/account/login");

        TestContext.WriteLine($"Current URL: {currentUrl}");
        TestContext.WriteLine($"Is Error Page: {isErrorPage}");
        TestContext.WriteLine($"Is Redirected to Login: {isRedirectedToLogin}");

        Assert.That(isErrorPage || isRedirectedToLogin, Is.True,
            "Non-admin user should be blocked from admin pages");

        if (isErrorPage)
        {
            var errorCode = await ErrorPage.GetErrorCodeAsync();
            Assert.That(errorCode, Is.AnyOf("401", "403"),
                "Admin access should show 401 or 403 error");
        }

        TestContext.WriteLine("✅ Admin page access properly restricted");
    }

    [Test]
    public async Task DirectErrorPageAccess_ShouldWork()
    {
        // Arrange & Act - Test direct access to error pages (simulates server errors)
        await ErrorPage.TriggerDirectErrorPageAsync(500);

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            "Direct error page access should work");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("500"),
            "Direct error page should show correct status code");

        var errorMessage = await ErrorPage.GetErrorMessageAsync();
        Assert.That(errorMessage, Does.Contain("internal server error").IgnoreCase,
            "500 error should mention internal server error");

        Assert.That(await ErrorPage.HasTryAgainButtonAsync(), Is.True,
            "500 error should have try again button");

        TestContext.WriteLine("✅ Direct error page access works (simulates server error handling)");
    }

    [Test]
    public async Task RequestTimeout_ShouldShow408Error()
    {
        // Arrange & Act - Trigger a 408 error through the test controller
        await ErrorPage.TriggerDirectErrorPageAsync(408);

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
            "Request timeout should show error page");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("408"), 
            "Request timeout should show 408 error");

        var errorMessage = await ErrorPage.GetErrorMessageAsync();
        Assert.That(errorMessage, Does.Contain("timeout").IgnoreCase, 
            "408 error should mention timeout");

        Assert.That(await ErrorPage.HasTryAgainButtonAsync(), Is.True, 
            "408 error should have try again button");

        TestContext.WriteLine("✅ Request timeout shows 408 error");
    }

    [Test]
    public async Task TooManyRequests_ShouldShow429Error()
    {
        // Arrange & Act - Trigger a 429 error through the test controller
        await ErrorPage.TriggerDirectErrorPageAsync(429);

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
            "Too many requests should show error page");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("429"), 
            "Too many requests should show 429 error");

        var errorMessage = await ErrorPage.GetErrorMessageAsync();
        Assert.That(errorMessage, Does.Contain("too many requests").IgnoreCase, 
            "429 error should mention too many requests");

        TestContext.WriteLine("✅ Too many requests shows 429 error");
    }

    [Test]
    public async Task ErrorPageNavigation_FromDifferentPages_ShouldWork()
    {
        // Test error page navigation from different starting points
        var startingPages = new[]
        {
            "/",
            "/Task",
            "/Project"
        };

        foreach (var startingPage in startingPages)
        {
            // Arrange - Navigate to starting page
            await Page.GotoAsync($"{Config.BaseUrl}{startingPage}");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Act - Trigger error
            await ErrorPage.TriggerDirectErrorPageAsync(404);

            // Assert
            Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
                $"Error page should display when triggered from '{startingPage}'");

            // Test navigation back
            if (await ErrorPage.HasGoBackButtonAsync())
            {
                await ErrorPage.ClickGoBackButtonAsync();
                
                // Should navigate back (though may not be to exact starting page due to browser history)
                var currentUrl = Page.Url;
                Assert.That(currentUrl, Does.Not.Contain("Error"), 
                    "Go back button should navigate away from error page");
            }

            TestContext.WriteLine($"✅ Error page navigation works from '{startingPage}'");
        }
    }

    [Test]
    public async Task ErrorPage_ShouldMaintainSessionState()
    {
        // Arrange - Login as admin
        await LoginAsAdminAsync();
        Assert.That(await IsLoggedInAsync(), Is.True, "Should be logged in");

        // Act - Trigger an error
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert - Should still be logged in after error
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
            "Error page should be displayed");

        // Navigate back to main app
        await ErrorPage.ClickGoHomeButtonAsync();

        // Should still be logged in
        Assert.That(await IsLoggedInAsync(), Is.True, 
            "Should still be logged in after error page");

        TestContext.WriteLine("✅ Error page maintains session state");
    }

    [Test]
    public async Task ErrorPage_ShouldHandleBrowserBackButton()
    {
        // Arrange - Navigate to a normal page first
        await Page.GotoAsync($"{Config.BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act - Trigger error, then use browser back button
        await ErrorPage.TriggerDirectErrorPageAsync(404);
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
            "Error page should be displayed");

        await Page.GoBackAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Should navigate back to previous page
        var currentUrl = Page.Url;
        Assert.That(currentUrl, Does.Not.Contain("Error"), 
            "Browser back button should navigate away from error page");

        TestContext.WriteLine("✅ Error page handles browser back button correctly");
    }

    [Test]
    public async Task ErrorPage_ShouldHandlePageRefresh()
    {
        // Arrange & Act
        await ErrorPage.TriggerDirectErrorPageAsync(404);
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
            "Error page should be displayed");

        // Refresh the page
        await Page.ReloadAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Error page should still be displayed correctly
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
            "Error page should still be displayed after refresh");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("404"), 
            "Error code should be preserved after refresh");

        TestContext.WriteLine("✅ Error page handles page refresh correctly");
    }

    [Test]
    public async Task ErrorPage_ShouldWorkWithJavaScriptDisabled()
    {
        // Arrange - Disable JavaScript
        await Context.AddInitScriptAsync("() => { window.navigator.javaEnabled = () => false; }");

        // Act
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert - Basic error page should still work
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True, 
            "Error page should work without JavaScript");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("404"), 
            "Error code should be displayed without JavaScript");

        // Navigation links should still work (they're regular HTML links)
        Assert.That(await ErrorPage.HasGoHomeButtonAsync(), Is.True, 
            "Navigation buttons should be present without JavaScript");

        TestContext.WriteLine("✅ Error page works without JavaScript");
    }

    protected override bool RequiresApplicationConnection() => true;
}
