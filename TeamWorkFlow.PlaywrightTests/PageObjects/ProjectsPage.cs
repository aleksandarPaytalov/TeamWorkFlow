namespace TeamWorkFlow.PlaywrightTests.PageObjects;

public class ProjectsPage : BasePage
{
    public ProjectsPage(IPage page) : base(page) { }

    // Page URLs
    public string ListUrl => $"{Config.BaseUrl}/Project/All";
    public string CreateUrl => $"{Config.BaseUrl}/Project/Add";
    public string EditUrl(int id) => $"{Config.BaseUrl}/Project/Edit/{id}";
    public string DetailsUrl(int id) => $"{Config.BaseUrl}/Project/Details/{id}";
    public string DeleteUrl(int id) => $"{Config.BaseUrl}/Project/Delete/{id}";

    // List page elements
    private ILocator ProjectCards => Page.Locator(".project-card");
    private ILocator ProjectTable => Page.Locator("table.projects-table, .table");
    private ILocator ProjectRows => Page.Locator("tbody tr, .project-row");
    private ILocator CreateNewProjectButton => Page.Locator(".btn-success");
    private ILocator NoProjectsMessage => Page.Locator(".no-projects, .empty-state");

    // Form elements
    private ILocator ProjectNameInput => Page.Locator("input[name='ProjectName']");
    private ILocator ProjectNumberInput => Page.Locator("input[name='ProjectNumber']");
    private ILocator ProjectStatusSelect => Page.Locator("select[name='ProjectStatusId']");
    private ILocator TotalHoursSpentInput => Page.Locator("input[name='TotalHoursSpent']");

    // Action buttons
    private ILocator ViewDetailsButtons => Page.Locator(".action-btn-details");
    private ILocator EditButtons => Page.Locator(".action-btn-edit");
    private ILocator DeleteButtons => Page.Locator(".action-btn-delete");

    // Details page elements
    private ILocator ProjectDetailsContainer => Page.Locator(".project-details, .details-container");
    private ILocator ProjectNameDisplay => Page.Locator(".project-title, h1, h2, h3");
    private ILocator ProjectNumberDisplay => Page.Locator(".project-number");
    private ILocator ProjectStatusDisplay => Page.Locator(".project-status");
    private ILocator TotalHoursDisplay => Page.Locator(".total-hours, .hours");

    // Page methods
    public async Task NavigateToListAsync()
    {
        await NavigateToAsync(ListUrl);
    }

    public async Task NavigateToCreateAsync()
    {
        await NavigateToAsync(CreateUrl);
    }

    public async Task NavigateToEditAsync(int projectId)
    {
        await NavigateToAsync(EditUrl(projectId));
    }

    public async Task NavigateToDetailsAsync(int projectId)
    {
        await NavigateToAsync(DetailsUrl(projectId));
    }

    public async Task NavigateToDeleteAsync(int projectId)
    {
        await NavigateToAsync(DeleteUrl(projectId));
    }

    public async Task<bool> IsOnProjectsListPageAsync()
    {
        try
        {
            // Check if we're on the projects page by looking for the projects container or title
            var projectsContainer = Page.Locator(".projects-container");
            var projectsTitle = Page.Locator("h1:has-text('All Projects'), .projects-title");

            return await projectsContainer.IsVisibleAsync() ||
                   await projectsTitle.IsVisibleAsync() ||
                   await ProjectCards.First.IsVisibleAsync() ||
                   await ProjectTable.IsVisibleAsync() ||
                   await NoProjectsMessage.IsVisibleAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> GetProjectsCountAsync()
    {
        try
        {
            if (await ProjectCards.First.IsVisibleAsync())
            {
                return await ProjectCards.CountAsync();
            }
            else if (await ProjectRows.First.IsVisibleAsync())
            {
                return await ProjectRows.CountAsync();
            }
            return 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<bool> IsCreateButtonAvailableAsync()
    {
        try
        {
            return await CreateNewProjectButton.IsVisibleAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task ClickCreateNewProjectAsync()
    {
        try
        {
            // Check if the button is available (user must be admin)
            if (await CreateNewProjectButton.IsVisibleAsync())
            {
                await CreateNewProjectButton.ClickAsync();
                await WaitForPageLoadAsync();

                // Wait for the create form to load
                await ProjectNameInput.WaitForAsync(new() { Timeout = 10000 });
            }
            else
            {
                throw new Exception("Create New Project button not found. User may not have admin privileges or not on projects list page.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to click create new project button. Make sure you're logged in as admin and on the projects list page. Error: {ex.Message}", ex);
        }
    }

    public async Task CreateProjectAsync(string name, string projectNumber, string status = "Active", int totalHours = 0)
    {
        try
        {
            // Wait for the form to be available
            await ProjectNameInput.WaitForAsync(new() { Timeout = 10000 });

            await ProjectNameInput.FillAsync(name);
            await ProjectNumberInput.FillAsync(projectNumber);

            if (await ProjectStatusSelect.IsVisibleAsync())
            {
                await ProjectStatusSelect.SelectOptionAsync(new[] { status });
            }

            if (await TotalHoursSpentInput.IsVisibleAsync())
            {
                await TotalHoursSpentInput.FillAsync(totalHours.ToString());
            }

            await SubmitButton.ClickAsync();
            await WaitForPageLoadAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create project. Make sure you're on the create project form. Error: {ex.Message}", ex);
        }
    }

    public async Task CreateSampleProjectAsync()
    {
        var sampleProject = Config.SampleProject;
        await CreateProjectAsync(
            sampleProject.Name,
            sampleProject.ProjectNumber,
            "Active",
            int.Parse(sampleProject.TotalHoursSpent)
        );
    }

    public async Task<bool> IsProjectFormValidAsync()
    {
        return !await HasValidationErrorsAsync();
    }

    public async Task ClickFirstProjectDetailsAsync()
    {
        await ViewDetailsButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickFirstProjectEditAsync()
    {
        await EditButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickFirstProjectDeleteAsync()
    {
        await DeleteButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task<bool> IsOnProjectDetailsPageAsync()
    {
        return await ProjectDetailsContainer.IsVisibleAsync();
    }

    public async Task<string> GetProjectNameFromDetailsAsync()
    {
        return await ProjectNameDisplay.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetProjectNumberFromDetailsAsync()
    {
        return await ProjectNumberDisplay.TextContentAsync() ?? string.Empty;
    }

    public async Task EditProjectAsync(string newName, string newProjectNumber)
    {
        await ProjectNameInput.FillAsync(newName);
        await ProjectNumberInput.FillAsync(newProjectNumber);
        await SaveButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ConfirmDeleteAsync()
    {
        var confirmButton = Page.Locator("button:has-text('Delete'), input[value='Delete']");
        await confirmButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task SearchProjectsAsync(string searchTerm)
    {
        await SearchAsync(searchTerm);
    }

    public async Task SortProjectsAsync(string sortOption)
    {
        await SortByAsync(sortOption);
    }

    public async Task<string[]> GetProjectNamesAsync()
    {
        var names = new List<string>();

        try
        {
            if (await ProjectCards.First.IsVisibleAsync())
            {
                var cards = await ProjectCards.AllAsync();
                foreach (var card in cards)
                {
                    var nameElement = card.Locator(".project-title, .project-name, .card-title, h3, h4");
                    var name = await nameElement.TextContentAsync();
                    if (!string.IsNullOrWhiteSpace(name))
                        names.Add(name.Trim());
                }
            }
            else if (await ProjectRows.First.IsVisibleAsync())
            {
                var rows = await ProjectRows.AllAsync();
                foreach (var row in rows)
                {
                    var nameCell = row.Locator("td:first-child, .project-name");
                    var name = await nameCell.TextContentAsync();
                    if (!string.IsNullOrWhiteSpace(name))
                        names.Add(name.Trim());
                }
            }
        }
        catch
        {
            // Return empty array if there's an error
        }

        return names.ToArray();
    }

    public async Task<bool> ProjectExistsAsync(string projectName)
    {
        var projectNames = await GetProjectNamesAsync();
        return projectNames.Any(name => name.Contains(projectName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> IsResponsiveDesignWorkingAsync()
    {
        try
        {
            // Test mobile viewport
            await Page.SetViewportSizeAsync(375, 667);
            await Page.WaitForTimeoutAsync(1000);

            var projectsContainer = Page.Locator(".projects-container");
            var isMobileResponsive = await projectsContainer.IsVisibleAsync() ||
                                    await ProjectCards.First.IsVisibleAsync() ||
                                    await ProjectTable.IsVisibleAsync();

            // Test desktop viewport
            await Page.SetViewportSizeAsync(1920, 1080);
            await Page.WaitForTimeoutAsync(1000);

            var isDesktopResponsive = await projectsContainer.IsVisibleAsync() ||
                                     await ProjectCards.First.IsVisibleAsync() ||
                                     await ProjectTable.IsVisibleAsync();

            return isMobileResponsive && isDesktopResponsive;
        }
        catch
        {
            return false;
        }
    }
}
