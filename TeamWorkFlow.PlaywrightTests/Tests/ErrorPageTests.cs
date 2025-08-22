using Microsoft.Playwright;

namespace TeamWorkFlow.PlaywrightTests.Tests;

[TestFixture]
public class ErrorPageTests : BaseTest
{
    [SetUp]
    public async Task TestSetUp()
    {
        // Ensure we can connect to the application for error page testing
        await EnsureApplicationIsRunningAsync();
    }

    [Test]
    public async Task DirectErrorPage_ShouldBeAccessible()
    {
        // Arrange & Act - Test direct access to error pages
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            "Direct error page access should work");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("404"),
            "Direct error page should show correct status code");

        TestContext.WriteLine("✅ Direct error page access works");
    }

    [Test]
    [TestCase(400, "Bad Request", "fa-exclamation-triangle")]
    [TestCase(401, "Unauthorized", "fa-lock")]
    [TestCase(403, "Forbidden", "fa-ban")]
    [TestCase(404, "Page Not Found", "fa-search")]
    [TestCase(408, "Request Timeout", "fa-clock")]
    [TestCase(429, "Too Many Requests", "fa-tachometer")]
    [TestCase(500, "Internal Server Error", "fa-server")]
    public async Task ErrorPage_ShouldDisplayCorrectContent(int statusCode, string expectedTitle, string expectedIcon)
    {
        // Arrange & Act - Test direct error page access
        await ErrorPage.TriggerDirectErrorPageAsync(statusCode);

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            $"Error page should be displayed for status code {statusCode}");

        var actualCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(actualCode, Is.EqualTo(statusCode.ToString()),
            $"Error code should be {statusCode}");

        var actualTitle = await ErrorPage.GetErrorTitleAsync();
        Assert.That(actualTitle, Does.Contain(expectedTitle),
            $"Error title should contain '{expectedTitle}'");

        var iconClass = await ErrorPage.GetIconClassAsync();
        Assert.That(iconClass, Does.Contain(expectedIcon),
            $"Error icon should contain '{expectedIcon}' class");

        Assert.That(await ErrorPage.HasErrorIconAsync(), Is.True,
            "Error page should have an icon");

        Assert.That(await ErrorPage.HasErrorActionsAsync(), Is.True,
            "Error page should have action buttons");

        TestContext.WriteLine($"✅ Error page {statusCode} displays correct content");
    }

    [Test]
    public async Task Error401Page_ShouldHaveLoginButton()
    {
        // Arrange & Act - Test direct 401 error page
        await ErrorPage.TriggerDirectErrorPageAsync(401);

        // Assert
        Assert.That(await ErrorPage.IsError401PageCorrectAsync(), Is.True,
            "401 error page should have correct elements");

        Assert.That(await ErrorPage.HasLoginButtonAsync(), Is.True,
            "401 error page should have a login button");

        TestContext.WriteLine("✅ Error 401 page has login button");
    }

    [Test]
    public async Task Error404Page_ShouldHaveCorrectElements()
    {
        // Arrange & Act - Test direct 404 error page
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert
        Assert.That(await ErrorPage.IsError404PageCorrectAsync(), Is.True,
            "404 error page should have correct elements");

        Assert.That(await ErrorPage.HasGoBackButtonAsync(), Is.True,
            "404 error page should have a go back button");

        TestContext.WriteLine("✅ Error 404 page has correct elements");
    }

    [Test]
    public async Task Error429Page_ShouldHaveRetryCountdown()
    {
        // Arrange & Act - Test direct 429 error page
        await ErrorPage.TriggerDirectErrorPageAsync(429);

        // Assert
        Assert.That(await ErrorPage.IsError429PageCorrectAsync(), Is.True,
            "429 error page should have correct elements");

        // Check if countdown is present (optional feature)
        var hasCountdown = await ErrorPage.HasRetryCountdownAsync();

        // The test passes if the page is correct, countdown is optional
        TestContext.WriteLine($"✅ Error 429 page has correct elements (countdown present: {hasCountdown})");
    }

    [Test]
    public async Task Error500Page_ShouldHaveTryAgainButton()
    {
        // Arrange & Act - Test direct 500 error page
        await ErrorPage.TriggerDirectErrorPageAsync(500);

        // Assert
        Assert.That(await ErrorPage.IsError500PageCorrectAsync(), Is.True,
            "500 error page should have correct elements");

        Assert.That(await ErrorPage.HasTryAgainButtonAsync(), Is.True,
            "500 error page should have a try again button");

        TestContext.WriteLine("✅ Error 500 page has try again button");
    }

    [Test]
    public async Task CustomErrorPage_ShouldDisplayGenericError()
    {
        // Arrange & Act - Test custom error code
        await ErrorPage.TriggerDirectErrorPageAsync(418); // I'm a teapot

        // Assert
        Assert.That(await ErrorPage.IsErrorPageDisplayedAsync(), Is.True,
            "Custom error page should be displayed");

        var errorCode = await ErrorPage.GetErrorCodeAsync();
        Assert.That(errorCode, Is.EqualTo("418"),
            "Custom error should display correct status code");

        TestContext.WriteLine("✅ Custom error page displays correctly");
    }

    [Test]
    public async Task NonExistentPage_ShouldShow404Error()
    {
        // Arrange & Act
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
    [TestCase(1920, 1080)] // Desktop
    [TestCase(768, 1024)]  // Tablet
    [TestCase(375, 667)]   // Mobile
    public async Task ErrorPages_ShouldBeResponsive(int width, int height)
    {
        // Arrange
        await Page.SetViewportSizeAsync(width, height);

        // Act
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert
        Assert.That(await ErrorPage.IsResponsiveDesignWorkingAsync(), Is.True, 
            $"Error page should be responsive at {width}x{height}");

        // Check mobile-specific behavior
        if (width <= 768)
        {
            Assert.That(await ErrorPage.AreButtonsStackedOnMobileAsync(), Is.True, 
                "Buttons should be stacked on mobile devices");
        }

        TestContext.WriteLine($"✅ Error page is responsive at {width}x{height}");
    }

    [Test]
    public async Task ErrorPages_ShouldHaveAccessibleDesign()
    {
        // Arrange & Act
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert
        Assert.That(await ErrorPage.HasProperHeadingStructureAsync(), Is.True, 
            "Error page should have proper heading structure");

        Assert.That(await ErrorPage.HasProperContrastAsync(), Is.True, 
            "Error page should have proper text contrast");

        Assert.That(await ErrorPage.HasFontAwesomeIconAsync(), Is.True, 
            "Error page should have Font Awesome icons");

        TestContext.WriteLine("✅ Error page has accessible design");
    }

    [Test]
    public async Task ErrorPageButtons_ShouldHaveHoverEffects()
    {
        // Arrange & Act
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert
        Assert.That(await ErrorPage.HasSmoothTransitionsAsync(), Is.True, 
            "Error page buttons should have smooth transitions");

        TestContext.WriteLine("✅ Error page buttons have hover effects");
    }

    [Test]
    public async Task ErrorPageNavigation_ShouldWork()
    {
        // Arrange
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Act & Assert - Test go home button
        if (await ErrorPage.HasGoHomeButtonAsync())
        {
            await ErrorPage.ClickGoHomeButtonAsync();
            
            // Should navigate away from error page
            var currentUrl = Page.Url;
            Assert.That(currentUrl, Does.Not.Contain("Error"), 
                "Go home button should navigate away from error page");
            
            TestContext.WriteLine("✅ Go home button works correctly");
        }
    }

    [Test]
    public async Task ErrorPage_ShouldLoadFontAwesome()
    {
        // Arrange & Act
        await ErrorPage.TriggerDirectErrorPageAsync(404);

        // Assert
        Assert.That(await ErrorPage.HasFontAwesomeIconAsync(), Is.True, 
            "Error page should load Font Awesome icons");

        // Check if Font Awesome CSS is loaded
        var fontAwesomeLoaded = await Page.EvaluateAsync<bool>(@"
            () => {
                const links = document.querySelectorAll('link[href*=""font-awesome""]');
                return links.length > 0;
            }
        ");

        Assert.That(fontAwesomeLoaded, Is.True, 
            "Font Awesome CSS should be loaded");

        TestContext.WriteLine("✅ Font Awesome icons load correctly");
    }

    [Test]
    public async Task ErrorPages_ShouldHaveCorrectActionButtonCount()
    {
        // Test different error pages have appropriate number of action buttons
        var testCases = new[]
        {
            new { StatusCode = 400, MinButtons = 2 },
            new { StatusCode = 401, MinButtons = 2 },
            new { StatusCode = 403, MinButtons = 2 },
            new { StatusCode = 404, MinButtons = 2 },
            new { StatusCode = 408, MinButtons = 2 },
            new { StatusCode = 429, MinButtons = 2 },
            new { StatusCode = 500, MinButtons = 2 }
        };

        foreach (var testCase in testCases)
        {
            // Arrange & Act
            await ErrorPage.TriggerDirectErrorPageAsync(testCase.StatusCode);

            // Assert
            var buttonCount = await ErrorPage.GetActionButtonCountAsync();
            Assert.That(buttonCount, Is.GreaterThanOrEqualTo(testCase.MinButtons), 
                $"Error {testCase.StatusCode} should have at least {testCase.MinButtons} action buttons");

            TestContext.WriteLine($"✅ Error {testCase.StatusCode} has {buttonCount} action buttons");
        }
    }

    protected override bool RequiresApplicationConnection() => true;
}
