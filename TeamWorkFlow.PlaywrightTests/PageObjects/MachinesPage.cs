namespace TeamWorkFlow.PlaywrightTests.PageObjects;

public class MachinesPage : BasePage
{
    public MachinesPage(IPage page) : base(page) { }

    // Page URLs
    public string ListUrl => $"{Config.BaseUrl}/Machine";
    public string CreateUrl => $"{Config.BaseUrl}/Machine/Create";
    public string EditUrl(int id) => $"{Config.BaseUrl}/Machine/Edit/{id}";
    public string DetailsUrl(int id) => $"{Config.BaseUrl}/Machine/Details/{id}";
    public string DeleteUrl(int id) => $"{Config.BaseUrl}/Machine/Delete/{id}";

    // List page elements
    private ILocator MachineCards => Page.Locator(".machine-card, .card");
    private ILocator MachineTable => Page.Locator("table.machines-table, .table");
    private ILocator MachineRows => Page.Locator("tbody tr, .machine-row");
    private ILocator CreateNewMachineButton => Page.Locator("a:has-text('Create'), a:has-text('New Machine'), .btn-create");
    private ILocator NoMachinesMessage => Page.Locator(".no-machines, .empty-state");

    // Form elements
    private ILocator MachineNameInput => Page.Locator("input[name='Name'], #Name");
    private ILocator CalibrationScheduleInput => Page.Locator("input[name='CalibrationSchedule'], #CalibrationSchedule");
    private ILocator CapacityInput => Page.Locator("input[name='Capacity'], #Capacity");
    private ILocator IsCalibratedSelect => Page.Locator("select[name='IsCalibrated'], #IsCalibrated");
    private ILocator ImageUrlInput => Page.Locator("input[name='ImageUrl'], #ImageUrl");

    // Action buttons
    private ILocator ViewDetailsButtons => Page.Locator("a:has-text('Details'), .btn-details");
    private ILocator EditButtons => Page.Locator("a:has-text('Edit'), .btn-edit");
    private ILocator DeleteButtons => Page.Locator("a:has-text('Delete'), .btn-delete");

    // Details page elements
    private ILocator MachineDetailsContainer => Page.Locator(".machine-details, .details-container");
    private ILocator MachineNameDisplay => Page.Locator(".machine-name, h1, h2");
    private ILocator CalibrationStatusDisplay => Page.Locator(".calibration-status, .status");
    private ILocator CapacityDisplay => Page.Locator(".capacity, .machine-capacity");
    private ILocator MachineImageDisplay => Page.Locator(".machine-image, img");

    // Page methods
    public async Task NavigateToListAsync()
    {
        await NavigateToAsync(ListUrl);
    }

    public async Task NavigateToCreateAsync()
    {
        await NavigateToAsync(CreateUrl);
    }

    public async Task NavigateToEditAsync(int machineId)
    {
        await NavigateToAsync(EditUrl(machineId));
    }

    public async Task NavigateToDetailsAsync(int machineId)
    {
        await NavigateToAsync(DetailsUrl(machineId));
    }

    public async Task NavigateToDeleteAsync(int machineId)
    {
        await NavigateToAsync(DeleteUrl(machineId));
    }

    public async Task<bool> IsOnMachinesListPageAsync()
    {
        return await MachineCards.First.IsVisibleAsync() || await MachineTable.IsVisibleAsync() || await NoMachinesMessage.IsVisibleAsync();
    }

    public async Task<int> GetMachinesCountAsync()
    {
        if (await MachineCards.First.IsVisibleAsync())
        {
            return await MachineCards.CountAsync();
        }
        else if (await MachineRows.First.IsVisibleAsync())
        {
            return await MachineRows.CountAsync();
        }
        return 0;
    }

    public async Task ClickCreateNewMachineAsync()
    {
        await CreateNewMachineButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task CreateMachineAsync(string name, string calibrationDate, int capacity, bool isCalibrated = true, string imageUrl = "")
    {
        await MachineNameInput.FillAsync(name);
        await CalibrationScheduleInput.FillAsync(calibrationDate);
        await CapacityInput.FillAsync(capacity.ToString());
        
        if (await IsCalibratedSelect.IsVisibleAsync())
        {
            await IsCalibratedSelect.SelectOptionAsync(isCalibrated.ToString());
        }
        
        if (!string.IsNullOrEmpty(imageUrl) && await ImageUrlInput.IsVisibleAsync())
        {
            await ImageUrlInput.FillAsync(imageUrl);
        }
        
        await SubmitButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task CreateSampleMachineAsync()
    {
        var sampleMachine = Config.SampleMachine;
        var calibrationDate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
        
        await CreateMachineAsync(
            sampleMachine.Name,
            calibrationDate,
            int.Parse(sampleMachine.Capacity),
            true,
            sampleMachine.ImageUrl
        );
    }

    public async Task<bool> IsMachineFormValidAsync()
    {
        return !await HasValidationErrorsAsync();
    }

    public async Task ClickFirstMachineDetailsAsync()
    {
        await ViewDetailsButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickFirstMachineEditAsync()
    {
        await EditButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickFirstMachineDeleteAsync()
    {
        await DeleteButtons.First.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task<bool> IsOnMachineDetailsPageAsync()
    {
        return await MachineDetailsContainer.IsVisibleAsync();
    }

    public async Task<string> GetMachineNameFromDetailsAsync()
    {
        return await MachineNameDisplay.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetCalibrationStatusFromDetailsAsync()
    {
        return await CalibrationStatusDisplay.TextContentAsync() ?? string.Empty;
    }

    public async Task EditMachineAsync(string newName, int newCapacity)
    {
        await MachineNameInput.FillAsync(newName);
        await CapacityInput.FillAsync(newCapacity.ToString());
        await SaveButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ConfirmDeleteAsync()
    {
        var confirmButton = Page.Locator("button:has-text('Delete'), input[value='Delete']");
        await confirmButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task SearchMachinesAsync(string searchTerm)
    {
        await SearchAsync(searchTerm);
    }

    public async Task SortMachinesAsync(string sortOption)
    {
        await SortByAsync(sortOption);
    }

    public async Task<string[]> GetMachineNamesAsync()
    {
        var names = new List<string>();
        
        if (await MachineCards.First.IsVisibleAsync())
        {
            var cards = await MachineCards.AllAsync();
            foreach (var card in cards)
            {
                var nameElement = card.Locator(".machine-name, .card-title, h3, h4");
                var name = await nameElement.TextContentAsync();
                if (!string.IsNullOrWhiteSpace(name))
                    names.Add(name);
            }
        }
        else if (await MachineRows.First.IsVisibleAsync())
        {
            var rows = await MachineRows.AllAsync();
            foreach (var row in rows)
            {
                var nameCell = row.Locator("td:first-child, .machine-name");
                var name = await nameCell.TextContentAsync();
                if (!string.IsNullOrWhiteSpace(name))
                    names.Add(name);
            }
        }
        
        return names.ToArray();
    }

    public async Task<bool> MachineExistsAsync(string machineName)
    {
        var machineNames = await GetMachineNamesAsync();
        return machineNames.Any(name => name.Contains(machineName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> IsResponsiveDesignWorkingAsync()
    {
        // Test mobile viewport
        await Page.SetViewportSizeAsync(375, 667);
        await Page.WaitForTimeoutAsync(1000);

        var isMobileResponsive = await MachineCards.First.IsVisibleAsync() || await MachineTable.IsVisibleAsync();

        // Test desktop viewport
        await Page.SetViewportSizeAsync(1920, 1080);
        await Page.WaitForTimeoutAsync(1000);
        
        var isDesktopResponsive = await MachineCards.First.IsVisibleAsync() || await MachineTable.IsVisibleAsync();
        
        return isMobileResponsive && isDesktopResponsive;
    }

    public async Task<bool> HasCalibrationAlertsAsync()
    {
        var calibrationAlerts = Page.Locator(".calibration-alert, .alert-warning");
        return await calibrationAlerts.IsVisibleAsync();
    }

    public async Task<bool> HasCapacityIndicatorsAsync()
    {
        var capacityIndicators = Page.Locator(".capacity-indicator, .progress-bar");
        return await capacityIndicators.IsVisibleAsync();
    }
}
