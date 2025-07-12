namespace TeamWorkFlow.PlaywrightTests.PageObjects;

public class TasksPage : BasePage
{
    public TasksPage(IPage page) : base(page) { }

    // Page URLs
    public string ListUrl => $"{Config.BaseUrl}/Task";
    public string CreateUrl => $"{Config.BaseUrl}/Task/Create";
    public string EditUrl(int id) => $"{Config.BaseUrl}/Task/Edit/{id}";
    public string DetailsUrl(int id) => $"{Config.BaseUrl}/Task/Details/{id}";
    public string DeleteUrl(int id) => $"{Config.BaseUrl}/Task/Delete/{id}";

    // List page elements
    private ILocator TaskCards => Page.Locator(".task-card, .card");
    private ILocator TaskTable => Page.Locator("table.tasks-table, .table");
    private ILocator TaskRows => Page.Locator("tbody tr, .task-row");
    private ILocator CreateNewTaskButton => Page.Locator("a:has-text('Create'), a:has-text('New Task'), .btn-create");
    private ILocator NoTasksMessage => Page.Locator(".no-tasks, .empty-state");

    // Form elements
    private ILocator TaskNameInput => Page.Locator("input[name='Name'], #Name");
    private ILocator TaskDescriptionInput => Page.Locator("textarea[name='Description'], #Description");
    private ILocator ProjectNumberInput => Page.Locator("input[name='ProjectNumber'], #ProjectNumber");
    private ILocator StartDateInput => Page.Locator("input[name='StartDate'], #StartDate");
    private ILocator DeadlineInput => Page.Locator("input[name='Deadline'], #Deadline");
    private ILocator EndDateInput => Page.Locator("input[name='EndDate'], #EndDate");
    private ILocator PrioritySelect => Page.Locator("select[name='PriorityId'], #PriorityId");
    private ILocator StatusSelect => Page.Locator("select[name='StatusId'], #StatusId");
    private ILocator ProjectSelect => Page.Locator("select[name='ProjectId'], #ProjectId");

    // Action buttons
    private ILocator ViewDetailsButtons => Page.Locator("a:has-text('Details'), .btn-details");
    private ILocator EditButtons => Page.Locator("a:has-text('Edit'), .btn-edit");
    private ILocator DeleteButtons => Page.Locator("a:has-text('Delete'), .btn-delete");

    // Details page elements
    private ILocator TaskDetailsContainer => Page.Locator(".task-details, .details-container");
    private ILocator TaskNameDisplay => Page.Locator(".task-name, h1, h2");
    private ILocator TaskDescriptionDisplay => Page.Locator(".task-description, .description");
    private ILocator TaskStatusDisplay => Page.Locator(".task-status, .status");
    private ILocator TaskPriorityDisplay => Page.Locator(".task-priority, .priority");

    // Page methods
    public async Task NavigateToListAsync()
    {
        await NavigateToAsync(ListUrl);
    }

    public async Task NavigateToCreateAsync()
    {
        await NavigateToAsync(CreateUrl);
    }

    public async Task NavigateToEditAsync(int taskId)
    {
        await NavigateToAsync(EditUrl(taskId));
    }

    public async Task NavigateToDetailsAsync(int taskId)
    {
        await NavigateToAsync(DetailsUrl(taskId));
    }

    public async Task NavigateToDeleteAsync(int taskId)
    {
        await NavigateToAsync(DeleteUrl(taskId));
    }

    public async Task<bool> IsOnTasksListPageAsync()
    {
        return await TaskCards.First.IsVisibleAsync() || await TaskTable.IsVisibleAsync() || await NoTasksMessage.IsVisibleAsync();
    }

    public async Task<int> GetTasksCountAsync()
    {
        if (await TaskCards.First.IsVisibleAsync())
        {
            return await TaskCards.CountAsync();
        }
        else if (await TaskRows.First.IsVisibleAsync())
        {
            return await TaskRows.CountAsync();
        }
        return 0;
    }

    public async Task ClickCreateNewTaskAsync()
    {
        await CreateNewTaskButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task CreateTaskAsync(string name, string description, string projectNumber, 
        string startDate, string deadline, string priority = "Medium", string status = "Not Started")
    {
        await TaskNameInput.FillAsync(name);
        await TaskDescriptionInput.FillAsync(description);
        await ProjectNumberInput.FillAsync(projectNumber);
        await StartDateInput.FillAsync(startDate);
        await DeadlineInput.FillAsync(deadline);
        
        if (await PrioritySelect.IsVisibleAsync())
        {
            await PrioritySelect.SelectOptionAsync(new[] { priority });
        }
        
        if (await StatusSelect.IsVisibleAsync())
        {
            await StatusSelect.SelectOptionAsync(new[] { status });
        }
        
        await SubmitButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task CreateSampleTaskAsync()
    {
        var sampleTask = Config.SampleTask;
        var today = DateTime.Now.ToString("dd/MM/yyyy");
        var deadline = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
        
        await CreateTaskAsync(
            sampleTask.Name,
            sampleTask.Description,
            sampleTask.ProjectNumber,
            today,
            deadline
        );
    }

    public async Task<bool> IsTaskFormValidAsync()
    {
        return !await HasValidationErrorsAsync();
    }

    public async Task ClickFirstTaskDetailsAsync()
    {
        await ViewDetailsButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickFirstTaskEditAsync()
    {
        await EditButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickFirstTaskDeleteAsync()
    {
        await DeleteButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task<bool> IsOnTaskDetailsPageAsync()
    {
        return await TaskDetailsContainer.IsVisibleAsync();
    }

    public async Task<string> GetTaskNameFromDetailsAsync()
    {
        return await TaskNameDisplay.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetTaskDescriptionFromDetailsAsync()
    {
        return await TaskDescriptionDisplay.TextContentAsync() ?? string.Empty;
    }

    public async Task EditTaskAsync(string newName, string newDescription)
    {
        await TaskNameInput.FillAsync(newName);
        await TaskDescriptionInput.FillAsync(newDescription);
        await SaveButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ConfirmDeleteAsync()
    {
        var confirmButton = Page.Locator("button:has-text('Delete'), input[value='Delete']");
        await confirmButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task SearchTasksAsync(string searchTerm)
    {
        await SearchAsync(searchTerm);
    }

    public async Task SortTasksAsync(string sortOption)
    {
        await SortByAsync(sortOption);
    }

    public async Task<string[]> GetTaskNamesAsync()
    {
        var names = new List<string>();
        
        if (await TaskCards.First.IsVisibleAsync())
        {
            var cards = await TaskCards.AllAsync();
            foreach (var card in cards)
            {
                var nameElement = card.Locator(".task-name, .card-title, h3, h4");
                var name = await nameElement.TextContentAsync();
                if (!string.IsNullOrWhiteSpace(name))
                    names.Add(name);
            }
        }
        else if (await TaskRows.First.IsVisibleAsync())
        {
            var rows = await TaskRows.AllAsync();
            foreach (var row in rows)
            {
                var nameCell = row.Locator("td:first-child, .task-name");
                var name = await nameCell.TextContentAsync();
                if (!string.IsNullOrWhiteSpace(name))
                    names.Add(name);
            }
        }
        
        return names.ToArray();
    }

    public async Task<bool> TaskExistsAsync(string taskName)
    {
        var taskNames = await GetTaskNamesAsync();
        return taskNames.Any(name => name.Contains(taskName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> IsResponsiveDesignWorkingAsync()
    {
        // Test mobile viewport
        await Page.SetViewportSizeAsync(375, 667);
        await Page.WaitForTimeoutAsync(1000);

        var isMobileResponsive = await TaskCards.First.IsVisibleAsync() || await TaskTable.IsVisibleAsync();

        // Test desktop viewport
        await Page.SetViewportSizeAsync(1920, 1080);
        await Page.WaitForTimeoutAsync(1000);
        
        var isDesktopResponsive = await TaskCards.First.IsVisibleAsync() || await TaskTable.IsVisibleAsync();
        
        return isMobileResponsive && isDesktopResponsive;
    }
}
