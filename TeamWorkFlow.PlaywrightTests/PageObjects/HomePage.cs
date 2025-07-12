namespace TeamWorkFlow.PlaywrightTests.PageObjects;

public class HomePage : BasePage
{
    public HomePage(IPage page) : base(page) { }

    // Page URL
    public string Url => $"{Config.BaseUrl}/";

    // Dashboard elements
    private ILocator DashboardContainer => Page.Locator(".dashboard, [data-testid='dashboard']");
    private ILocator SummaryCards => Page.Locator(".summary-card, .dashboard-card");
    private ILocator TasksSummaryCard => Page.Locator(".summary-card:has-text('Tasks'), .dashboard-card:has-text('Tasks')");
    private ILocator ProjectsSummaryCard => Page.Locator(".summary-card:has-text('Projects'), .dashboard-card:has-text('Projects')");
    private ILocator MachinesSummaryCard => Page.Locator(".summary-card:has-text('Machines'), .dashboard-card:has-text('Machines')");
    private ILocator OperatorsSummaryCard => Page.Locator(".summary-card:has-text('Operators'), .dashboard-card:has-text('Operators')");
    private ILocator PartsSummaryCard => Page.Locator(".summary-card:has-text('Parts'), .dashboard-card:has-text('Parts')");

    // Quick action buttons
    private ILocator QuickActionsContainer => Page.Locator(".quick-actions, [data-testid='quick-actions']");
    private ILocator CreateTaskButton => Page.Locator("a:has-text('Create Task'), button:has-text('Create Task')");
    private ILocator CreateProjectButton => Page.Locator("a:has-text('Create Project'), button:has-text('Create Project')");
    private ILocator ViewAllTasksButton => Page.Locator("a:has-text('View All Tasks')").First;
    private ILocator ViewAllProjectsButton => Page.Locator("a:has-text('View All Projects'), a[href*='Project']");

    // Recent activity section
    private ILocator RecentActivityContainer => Page.Locator(".recent-activity, [data-testid='recent-activity']");
    private ILocator RecentActivityItems => Page.Locator(".activity-item, .recent-item");

    // Page methods
    public async Task NavigateAsync()
    {
        await NavigateToAsync(Url);
    }

    public async Task<bool> IsOnHomePageAsync()
    {
        try
        {
            await DashboardContainer.WaitForAsync(new() { Timeout = 5000 });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsDashboardLoadedAsync()
    {
        return await SummaryCards.First.IsVisibleAsync();
    }

    public async Task<int> GetSummaryCardsCountAsync()
    {
        return await SummaryCards.CountAsync();
    }

    public async Task<string> GetTasksSummaryAsync()
    {
        return await TasksSummaryCard.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetProjectsSummaryAsync()
    {
        return await ProjectsSummaryCard.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetMachinesSummaryAsync()
    {
        return await MachinesSummaryCard.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetOperatorsSummaryAsync()
    {
        return await OperatorsSummaryCard.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetPartsSummaryAsync()
    {
        return await PartsSummaryCard.TextContentAsync() ?? string.Empty;
    }

    public async Task ClickCreateTaskAsync()
    {
        await CreateTaskButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickCreateProjectAsync()
    {
        await CreateProjectButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickViewAllTasksAsync()
    {
        await ViewAllTasksButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickViewAllProjectsAsync()
    {
        await ViewAllProjectsButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task<bool> HasQuickActionsAsync()
    {
        return await QuickActionsContainer.IsVisibleAsync();
    }

    public async Task<bool> HasRecentActivityAsync()
    {
        return await RecentActivityContainer.IsVisibleAsync();
    }

    public async Task<int> GetRecentActivityItemsCountAsync()
    {
        return await RecentActivityItems.CountAsync();
    }

    public async Task<string[]> GetRecentActivityItemsAsync()
    {
        var items = await RecentActivityItems.AllAsync();
        var activities = new List<string>();
        
        foreach (var item in items)
        {
            var text = await item.TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
                activities.Add(text);
        }
        
        return activities.ToArray();
    }

    public async Task ClickSummaryCardAsync(string cardType)
    {
        var card = cardType.ToLower() switch
        {
            "tasks" => TasksSummaryCard,
            "projects" => ProjectsSummaryCard,
            "machines" => MachinesSummaryCard,
            "operators" => OperatorsSummaryCard,
            "parts" => PartsSummaryCard,
            _ => throw new ArgumentException($"Unknown card type: {cardType}")
        };

        await card.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task<bool> IsUserGreetingVisibleAsync()
    {
        return await UserGreeting.IsVisibleAsync();
    }

    public async Task<string> GetUserGreetingTextAsync()
    {
        return await UserGreeting.TextContentAsync() ?? string.Empty;
    }
}
