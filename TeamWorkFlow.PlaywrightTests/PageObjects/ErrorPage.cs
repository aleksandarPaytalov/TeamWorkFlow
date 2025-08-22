using Microsoft.Playwright;

namespace TeamWorkFlow.PlaywrightTests.PageObjects;

public class ErrorPage : BasePage
{
    public ErrorPage(IPage page) : base(page) { }

    // Error page specific elements
    private ILocator ErrorContainer => Page.Locator(".error-container");
    private ILocator ErrorIcon => Page.Locator(".error-icon");
    private ILocator ErrorCode => Page.Locator(".error-code");
    private ILocator ErrorTitle => Page.Locator(".error-title, h1");
    private ILocator ErrorPageMessage => Page.Locator(".error-message");
    private ILocator ErrorDescription => Page.Locator(".error-description");
    private ILocator ErrorActions => Page.Locator(".error-actions");
    private ILocator PrimaryButton => Page.Locator(".btn-primary-custom").First;
    private ILocator SecondaryButton => Page.Locator(".btn-secondary-custom, .error-actions a:last-child");
    private ILocator GoBackButton => Page.Locator("a:has-text('Go Back'), a:has-text('Back')");
    private ILocator GoHomeButton => Page.Locator("a:has-text('Go to Dashboard'), a:has-text('Go to Home'), a:has-text('Home')");
    private ILocator LoginButton => Page.Locator("a:has-text('Login')");
    private ILocator TryAgainButton => Page.Locator("a:has-text('Try Again'), a:has-text('Retry')");
    private ILocator RetryCountdown => Page.Locator("text=/Retry in \\d+/i, text=/\\d+ seconds?/i");

    // Navigation methods for real-world error scenarios
    public async Task NavigateToNonExistentPageAsync()
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/NonExistentPage{DateTime.Now.Ticks}");
        await WaitForPageLoadAsync();
    }

    public async Task NavigateToNonExistentControllerAsync()
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/InvalidController/InvalidAction");
        await WaitForPageLoadAsync();
    }

    public async Task NavigateToProtectedPageWithoutAuthAsync()
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/Task/Create");
        await WaitForPageLoadAsync();
    }

    public async Task NavigateToAdminPageWithoutPermissionAsync()
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/Admin/Users");
        await WaitForPageLoadAsync();
    }

    public async Task NavigateToInvalidTaskIdAsync()
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/Task/Details/999999");
        await WaitForPageLoadAsync();
    }

    public async Task NavigateToInvalidProjectIdAsync()
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/Project/Details/999999");
        await WaitForPageLoadAsync();
    }

    public async Task NavigateToMalformedUrlAsync()
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/Task/Details/invalid-id");
        await WaitForPageLoadAsync();
    }

    public async Task TriggerDirectErrorPageAsync(int statusCode)
    {
        await Page.GotoAsync($"{TestConfiguration.Instance.BaseUrl}/Home/Error/{statusCode}");
        await WaitForPageLoadAsync();
    }

    // Verification methods
    public async Task<bool> IsErrorPageDisplayedAsync()
    {
        try
        {
            await ErrorContainer.WaitForAsync(new() { Timeout = 5000 });
            return await ErrorContainer.IsVisibleAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetErrorCodeAsync()
    {
        return await ErrorCode.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetErrorTitleAsync()
    {
        return await ErrorTitle.TextContentAsync() ?? string.Empty;
    }

    public new async Task<string> GetErrorMessageAsync()
    {
        return await ErrorPageMessage.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetErrorDescriptionAsync()
    {
        return await ErrorDescription.TextContentAsync() ?? string.Empty;
    }

    public async Task<bool> HasErrorIconAsync()
    {
        return await ErrorIcon.IsVisibleAsync();
    }

    public async Task<bool> HasErrorActionsAsync()
    {
        return await ErrorActions.IsVisibleAsync();
    }

    public async Task<bool> HasGoBackButtonAsync()
    {
        return await GoBackButton.IsVisibleAsync();
    }

    public async Task<bool> HasGoHomeButtonAsync()
    {
        return await GoHomeButton.IsVisibleAsync();
    }

    public async Task<bool> HasLoginButtonAsync()
    {
        return await LoginButton.IsVisibleAsync();
    }

    public async Task<bool> HasTryAgainButtonAsync()
    {
        return await TryAgainButton.IsVisibleAsync();
    }

    public async Task<bool> HasRetryCountdownAsync()
    {
        return await RetryCountdown.IsVisibleAsync();
    }

    public async Task<int> GetActionButtonCountAsync()
    {
        var buttons = ErrorActions.Locator("a, button");
        return await buttons.CountAsync();
    }

    // Interaction methods
    public async Task ClickGoBackButtonAsync()
    {
        await GoBackButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickGoHomeButtonAsync()
    {
        await GoHomeButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickLoginButtonAsync()
    {
        await LoginButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickTryAgainButtonAsync()
    {
        await TryAgainButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickPrimaryButtonAsync()
    {
        await PrimaryButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickSecondaryButtonAsync()
    {
        await SecondaryButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    // Responsive design verification
    public async Task<bool> IsResponsiveDesignWorkingAsync()
    {
        // Check if error container is responsive
        var containerBox = await ErrorContainer.BoundingBoxAsync();
        if (containerBox == null) return false;

        // Verify container doesn't exceed viewport width
        var viewportSize = Page.ViewportSize;
        if (viewportSize == null) return false;

        return containerBox.Width <= viewportSize.Width;
    }

    public async Task<bool> AreButtonsStackedOnMobileAsync()
    {
        // Check if buttons are stacked vertically on mobile
        var buttons = ErrorActions.Locator("a, button");
        var buttonCount = await buttons.CountAsync();
        
        if (buttonCount < 2) return true; // Single button doesn't need stacking

        var firstButton = buttons.First;
        var secondButton = buttons.Nth(1);

        var firstBox = await firstButton.BoundingBoxAsync();
        var secondBox = await secondButton.BoundingBoxAsync();

        if (firstBox == null || secondBox == null) return false;

        // On mobile, buttons should be stacked (second button below first)
        return secondBox.Y > firstBox.Y;
    }

    // Font Awesome icon verification
    public async Task<bool> HasFontAwesomeIconAsync()
    {
        var iconElement = ErrorIcon.Locator("i[class*='fa']");
        return await iconElement.IsVisibleAsync();
    }

    public async Task<string> GetIconClassAsync()
    {
        var iconElement = ErrorIcon.Locator("i[class*='fa']");
        return await iconElement.GetAttributeAsync("class") ?? string.Empty;
    }

    // Accessibility verification
    public async Task<bool> HasProperHeadingStructureAsync()
    {
        // Check if error title is properly marked as h1
        var h1Element = Page.Locator("h1.error-title");
        return await h1Element.IsVisibleAsync();
    }

    public async Task<bool> HasProperContrastAsync()
    {
        // Basic check for text visibility (not comprehensive color contrast)
        var titleVisible = await ErrorTitle.IsVisibleAsync();
        var messageVisible = await ErrorMessage.IsVisibleAsync();
        var descriptionVisible = await ErrorDescription.IsVisibleAsync();

        return titleVisible && messageVisible && descriptionVisible;
    }

    // Animation and transition verification
    public async Task<bool> HasSmoothTransitionsAsync()
    {
        // Check if buttons have hover effects by verifying CSS transitions
        var button = PrimaryButton;
        var transitionProperty = await button.EvaluateAsync<string>("el => getComputedStyle(el).transition");
        return !string.IsNullOrEmpty(transitionProperty) && transitionProperty != "none";
    }

    // Error-specific validations
    public async Task<bool> IsError401PageCorrectAsync()
    {
        var code = await GetErrorCodeAsync();
        var hasLoginButton = await HasLoginButtonAsync();
        var iconClass = await GetIconClassAsync();
        
        return code == "401" && hasLoginButton && iconClass.Contains("fa-lock");
    }

    public async Task<bool> IsError404PageCorrectAsync()
    {
        var code = await GetErrorCodeAsync();
        var iconClass = await GetIconClassAsync();
        
        return code == "404" && iconClass.Contains("fa-search");
    }

    public async Task<bool> IsError429PageCorrectAsync()
    {
        var code = await GetErrorCodeAsync();
        var iconClass = await GetIconClassAsync();

        // 429 page is correct if it has the right code and icon
        // Countdown is optional and may not always be present
        return code == "429" && iconClass.Contains("fa-tachometer");
    }

    public async Task<bool> IsError500PageCorrectAsync()
    {
        var code = await GetErrorCodeAsync();
        var hasTryAgain = await HasTryAgainButtonAsync();
        var iconClass = await GetIconClassAsync();
        
        return code == "500" && hasTryAgain && iconClass.Contains("fa-server");
    }
}
