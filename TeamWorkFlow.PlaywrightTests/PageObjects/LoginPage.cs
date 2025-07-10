namespace TeamWorkFlow.PlaywrightTests.PageObjects;

public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page) { }

    // Page URL
    public string Url => $"{Config.BaseUrl}/Identity/Account/Login";

    // Page elements
    private ILocator EmailInput => Page.Locator("input[name='Input.Email'], input[type='email']");
    private ILocator PasswordInput => Page.Locator("input[name='Input.Password'], input[type='password']");
    private ILocator RememberMeCheckbox => Page.Locator("input[name='Input.RememberMe'], input[type='checkbox']");
    private ILocator LoginButton => Page.Locator("button[type='submit']:has-text('Log in'), input[value*='Log in']");
    private ILocator RegisterLink => Page.Locator("a:has-text('Register'), a[href*='Register']");
    private ILocator ForgotPasswordLink => Page.Locator("a:has-text('Forgot'), a[href*='ForgotPassword']");
    private ILocator LoginForm => Page.Locator("form");

    // Page methods
    public async Task NavigateAsync()
    {
        await NavigateToAsync(Url);
    }

    public async Task<bool> IsOnLoginPageAsync()
    {
        try
        {
            await LoginForm.WaitForAsync(new() { Timeout = 5000 });
            return await EmailInput.IsVisibleAsync() && await PasswordInput.IsVisibleAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task LoginAsync(string email, string password, bool rememberMe = false)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        
        if (rememberMe)
        {
            await RememberMeCheckbox.CheckAsync();
        }
        
        await LoginButton.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task LoginAsAdminAsync()
    {
        var adminUser = Config.AdminUser;
        await LoginAsync(adminUser.Email, adminUser.Password);
    }

    public async Task LoginAsOperatorAsync()
    {
        var operatorUser = Config.OperatorUser;
        await LoginAsync(operatorUser.Email, operatorUser.Password);
    }

    public async Task<bool> HasLoginErrorAsync()
    {
        return await HasErrorMessageAsync() || await HasValidationErrorsAsync();
    }

    public async Task<string[]> GetLoginErrorsAsync()
    {
        var errors = new List<string>();
        
        if (await HasErrorMessageAsync())
        {
            errors.Add(await GetErrorMessageAsync());
        }
        
        if (await HasValidationErrorsAsync())
        {
            errors.AddRange(await GetValidationErrorsAsync());
        }
        
        return errors.ToArray();
    }

    public async Task ClickRegisterLinkAsync()
    {
        await RegisterLink.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task ClickForgotPasswordLinkAsync()
    {
        await ForgotPasswordLink.ClickAsync();
        await WaitForPageLoadAsync();
    }

    public async Task<bool> IsEmailFieldValidAsync()
    {
        var emailField = EmailInput;
        var validationState = await emailField.GetAttributeAsync("aria-invalid");
        return validationState != "true";
    }

    public async Task<bool> IsPasswordFieldValidAsync()
    {
        var passwordField = PasswordInput;
        var validationState = await passwordField.GetAttributeAsync("aria-invalid");
        return validationState != "true";
    }

    public async Task ClearFormAsync()
    {
        await EmailInput.FillAsync("");
        await PasswordInput.FillAsync("");
        if (await RememberMeCheckbox.IsCheckedAsync())
        {
            await RememberMeCheckbox.UncheckAsync();
        }
    }
}
