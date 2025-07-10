namespace TeamWorkFlow.PlaywrightTests.PageObjects;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly TestConfiguration Config;

    protected BasePage(IPage page)
    {
        Page = page;
        Config = TestConfiguration.Instance;
    }

    // Common navigation elements
    protected ILocator NavigationBar => Page.Locator("nav.navbar");
    protected ILocator UserGreeting => Page.Locator("a[title='Manage Account']");
    protected ILocator LogoutButton => Page.Locator("a[href*='logout'], button:has-text('Logout')");
    protected ILocator HomeLink => Page.Locator("a[href='/'], a:has-text('Home')");
    protected ILocator TasksLink => Page.Locator("a[href*='Task'], a:has-text('Tasks')");
    protected ILocator ProjectsLink => Page.Locator("a[href*='Project'], a:has-text('Projects')");
    protected ILocator MachinesLink => Page.Locator("a[href*='Machine'], a:has-text('Machines')");
    protected ILocator OperatorsLink => Page.Locator("a[href*='Operator'], a:has-text('Operators')");
    protected ILocator PartsLink => Page.Locator("a[href*='Part'], a:has-text('Parts')");
    protected ILocator AdminLink => Page.Locator("a[href*='Admin'], a:has-text('Admin')");

    // Common form elements
    protected ILocator SubmitButton => Page.Locator("button[type='submit'], input[type='submit']");
    protected ILocator CancelButton => Page.Locator("button:has-text('Cancel'), a:has-text('Cancel')");
    protected ILocator SaveButton => Page.Locator("button:has-text('Save'), input[value='Save']");
    protected ILocator DeleteButton => Page.Locator("button:has-text('Delete'), a:has-text('Delete')");
    protected ILocator EditButton => Page.Locator("button:has-text('Edit'), a:has-text('Edit')");
    protected ILocator CreateButton => Page.Locator("button:has-text('Create'), a:has-text('Create')");

    // Common page elements
    protected ILocator PageTitle => Page.Locator("h1, .page-title");
    protected ILocator LoadingSpinner => Page.Locator(".spinner, .loading, [data-testid='loading']");
    protected ILocator ErrorMessage => Page.Locator(".alert-danger, .error-message, [data-testid='error']");
    protected ILocator SuccessMessage => Page.Locator(".alert-success, .success-message, [data-testid='success']");
    protected ILocator ValidationErrors => Page.Locator(".field-validation-error, .validation-summary-errors");

    // Search and filter elements
    protected ILocator SearchInput => Page.Locator("input[name='searchTerm'], input[placeholder*='Search']");
    protected ILocator SearchButton => Page.Locator("button:has-text('Search'), input[value='Search']");
    protected ILocator SortDropdown => Page.Locator("select[name*='sort'], select[name*='Sort']");
    protected ILocator FilterDropdown => Page.Locator("select[name*='filter'], select[name*='Filter']");

    // Pagination elements
    protected ILocator PaginationContainer => Page.Locator(".pagination, [data-testid='pagination']");
    protected ILocator NextPageButton => Page.Locator(".pagination .next, a:has-text('Next')");
    protected ILocator PreviousPageButton => Page.Locator(".pagination .previous, a:has-text('Previous')");
    protected ILocator PageNumbers => Page.Locator(".pagination .page-link");

    // Common methods
    public virtual async Task NavigateToAsync(string url)
    {
        await Page.GotoAsync(url);
        await WaitForPageLoadAsync();
    }

    public virtual async Task WaitForPageLoadAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await LoadingSpinner.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5000 });
    }

    public virtual async Task<bool> IsLoggedInAsync()
    {
        try
        {
            await UserGreeting.WaitForAsync(new() { Timeout = 3000 });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public virtual async Task<string> GetPageTitleAsync()
    {
        return await PageTitle.TextContentAsync() ?? string.Empty;
    }

    public virtual async Task<bool> HasErrorMessageAsync()
    {
        return await ErrorMessage.IsVisibleAsync();
    }

    public virtual async Task<string> GetErrorMessageAsync()
    {
        return await ErrorMessage.TextContentAsync() ?? string.Empty;
    }

    public virtual async Task<bool> HasSuccessMessageAsync()
    {
        return await SuccessMessage.IsVisibleAsync();
    }

    public virtual async Task<string> GetSuccessMessageAsync()
    {
        return await SuccessMessage.TextContentAsync() ?? string.Empty;
    }

    public virtual async Task<bool> HasValidationErrorsAsync()
    {
        return await ValidationErrors.IsVisibleAsync();
    }

    public virtual async Task<string[]> GetValidationErrorsAsync()
    {
        var errors = await ValidationErrors.AllAsync();
        var errorTexts = new List<string>();
        
        foreach (var error in errors)
        {
            var text = await error.TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
                errorTexts.Add(text);
        }
        
        return errorTexts.ToArray();
    }

    public virtual async Task ClickNavigationLinkAsync(string linkText)
    {
        var link = Page.Locator($"nav a:has-text('{linkText}')");
        await link.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public virtual async Task LogoutAsync()
    {
        await LogoutButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public virtual async Task SearchAsync(string searchTerm)
    {
        await SearchInput.FillAsync(searchTerm);
        await SearchButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public virtual async Task SortByAsync(string sortOption)
    {
        await SortDropdown.SelectOptionAsync(sortOption);
        await WaitForPageLoadAsync();
    }

    public virtual async Task<int> GetItemCountAsync()
    {
        var items = Page.Locator(".card, .list-item, tr:not(:first-child)");
        return await items.CountAsync();
    }

    public virtual async Task TakeScreenshotAsync(string name)
    {
        await Page.ScreenshotAsync(new()
        {
            Path = $"screenshots/{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png",
            FullPage = true
        });
    }
}
