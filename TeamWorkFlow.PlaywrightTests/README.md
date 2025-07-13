# TeamWorkFlow Playwright Tests

This directory contains end-to-end (E2E) tests for the TeamWorkFlow application using Microsoft Playwright.

## 🎯 Overview

The Playwright test suite provides comprehensive end-to-end testing for the TeamWorkFlow application, covering:

- **Authentication flows** (login, logout, role-based access)
- **Task management** (CRUD operations, search, filtering)
- **Project management** (creation, editing, deletion)
- **Machine (CMM) management** (calibration, capacity tracking)
- **Operator management** (assignment, admin workflows)
- **Navigation and UI** (responsive design, accessibility)

## 🏗 Project Structure

```
TeamWorkFlow.PlaywrightTests/
├── PageObjects/                 # Page Object Model classes
│   ├── BasePage.cs             # Base page with common functionality
│   ├── LoginPage.cs            # Login page interactions
│   ├── HomePage.cs             # Dashboard/home page
│   ├── TasksPage.cs            # Task management pages
│   ├── ProjectsPage.cs         # Project management pages
│   └── ...                     # Additional page objects
├── Tests/                      # Test classes
│   ├── BaseTest.cs             # Base test class with setup/teardown
│   ├── AuthenticationTests.cs  # Login/logout tests
│   ├── TaskManagementTests.cs  # Task CRUD tests
│   ├── ProjectManagementTests.cs # Project CRUD tests
│   └── ...                     # Additional test classes
├── screenshots/                # Test failure screenshots
├── playwright-report/          # HTML test reports
├── test-results/              # Test execution results
├── appsettings.json           # Test configuration
├── TestConfiguration.cs       # Configuration helper
├── GlobalUsings.cs            # Global using statements
└── playwright.config.ts       # Playwright configuration
```

## 🔒 Security

**Important**: This test suite uses secure credential management to prevent exposure of sensitive data.

- **Configuration**: Uses environment variables and secure fallbacks
- **Credentials**: Never commits real passwords to source control
- **Setup**: Run `.\setup-test-environment.ps1 -Help` for secure setup
- **Documentation**: See [SECURITY.md](SECURITY.md) for complete security guide

## 🚀 Getting Started

### Prerequisites

- .NET 6.0 SDK
- Node.js (for Playwright browsers)
- TeamWorkFlow application running locally

### Installation

1. **Restore NuGet packages:**
   ```bash
   dotnet restore TeamWorkFlow.PlaywrightTests/
   ```

2. **Install Playwright browsers:**
   ```bash
   cd TeamWorkFlow.PlaywrightTests
   dotnet build
   pwsh bin/Debug/net6.0/playwright.ps1 install
   ```

3. **Configure test settings:**
   Edit `appsettings.json` to match your environment:
   ```json
   {
     "TestSettings": {
       "BaseUrl": "https://localhost:7015",
       "Timeout": 30000
     },
     "TestUsers": {
       "AdminUser": {
         "Email": "admin@teamworkflow.local",
         "Password": "SecurePassword123!"
       }
     }
   }
   ```

   > **Security Note**: Use environment variables for production credentials. See [SECURITY.md](SECURITY.md) for secure setup.

## 🧪 Running Tests

### Run All Tests
```bash
dotnet test TeamWorkFlow.PlaywrightTests/
```

### Run Specific Test Class
```bash
dotnet test TeamWorkFlow.PlaywrightTests/ --filter "ClassName=AuthenticationTests"
```

### Run Tests with Specific Browser
```bash
dotnet test TeamWorkFlow.PlaywrightTests/ -- Playwright.BrowserName=chromium
```

### Run Tests in Headed Mode (Visible Browser)
```bash
dotnet test TeamWorkFlow.PlaywrightTests/ -- Playwright.LaunchOptions.Headless=false
```

### Generate HTML Report
```bash
dotnet test TeamWorkFlow.PlaywrightTests/ --logger html --results-directory TestResults
```

## 🔧 Configuration

### Test Settings

The `appsettings.json` file contains all test configuration:

- **BaseUrl**: Application URL for testing
- **Timeout**: Default timeout for operations
- **TestUsers**: Credentials for different user roles
- **TestData**: Sample data for test scenarios

### Browser Configuration

The `playwright.config.ts` file configures:

- **Browsers**: Chrome, Firefox, Safari, Edge
- **Viewports**: Desktop and mobile testing
- **Screenshots**: Capture on failure
- **Videos**: Record on failure
- **Traces**: Debug information

## 📝 Writing Tests

### Page Object Pattern

All tests use the Page Object Model pattern:

```csharp
public class TasksPage : BasePage
{
    public TasksPage(IPage page) : base(page) { }
    
    private ILocator TaskNameInput => Page.Locator("#TaskName");
    
    public async Task CreateTaskAsync(string name)
    {
        await TaskNameInput.FillAsync(name);
        await SubmitButton.ClickAsync();
    }
}
```

### Test Structure

Tests inherit from `BaseTest` for common functionality:

```csharp
[TestFixture]
public class TaskManagementTests : BaseTest
{
    [SetUp]
    public async Task TestSetUp()
    {
        await LoginAsAdminAsync();
    }
    
    [Test]
    public async Task CreateTask_WithValidData_ShouldSucceed()
    {
        // Arrange
        await TasksPage.NavigateToCreateAsync();
        
        // Act
        await TasksPage.CreateSampleTaskAsync();
        
        // Assert
        Assert.That(await TasksPage.HasSuccessMessageAsync(), Is.True);
    }
}
```

## 🐛 Debugging

### Screenshots on Failure

Failed tests automatically capture screenshots in the `screenshots/` directory.

### Video Recording

Test execution videos are saved in `test-results/` for failed tests.

### Trace Files

Playwright traces can be viewed using:
```bash
npx playwright show-trace test-results/trace.zip
```

### Running Single Test in Debug Mode

```bash
dotnet test TeamWorkFlow.PlaywrightTests/ --filter "TestName=CreateTask_WithValidData_ShouldSucceed" -- Playwright.LaunchOptions.Headless=false Playwright.LaunchOptions.SlowMo=1000
```

## 🔄 CI/CD Integration

Tests are automatically run in GitHub Actions:

- **Unit tests** run first
- **Playwright tests** run after successful build
- **Reports and screenshots** are uploaded as artifacts
- **Multiple browsers** are tested in parallel

## 📊 Test Reports

### HTML Reports

After test execution, view the HTML report:
```bash
# Open the generated report
start playwright-report/index.html
```

### Coverage Integration

Playwright tests complement unit tests for full coverage:
- **Unit tests**: Business logic (90%+ coverage)
- **Playwright tests**: UI workflows and integration

## 🛠 Maintenance

### Adding New Tests

1. Create page object in `PageObjects/`
2. Add test class in `Tests/`
3. Update `BaseTest.cs` if needed
4. Run tests locally before committing

### Updating Selectors

When UI changes, update selectors in page objects:
- Use data-testid attributes when possible
- Prefer semantic selectors over CSS classes
- Test selector stability across browsers

### Test Data Management

- Use `TestConfiguration` for environment-specific data
- Create sample data methods in page objects
- Clean up test data in teardown methods

## 🚨 Troubleshooting

### Common Issues

1. **Browser not found**: Run `playwright install`
2. **Timeout errors**: Increase timeout in configuration
3. **Element not found**: Check selector stability
4. **Authentication failures**: Verify test user credentials

### Performance Tips

- Use `Page.WaitForLoadStateAsync()` for navigation
- Implement proper waits instead of `Thread.Sleep()`
- Reuse browser contexts when possible
- Run tests in parallel for faster execution

## 🧹 Project Maintenance

### Recent Cleanup (2025-01-12)
- ✅ Removed unnecessary documentation files
- ✅ Removed outdated PowerShell scripts
- ✅ Cleaned up build artifacts and temporary folders
- ✅ Enhanced .gitignore for better artifact exclusion
- ✅ Updated installation and usage instructions

### File Structure (Clean)
```
TeamWorkFlow.PlaywrightTests/
├── PageObjects/                 # Page Object Model classes
├── Tests/                       # Test classes
├── README.md                    # This documentation
├── SECURITY.md                  # Security guidelines
├── run-playwright-tests.ps1     # Enhanced test runner
├── appsettings.json            # Test configuration
└── TeamWorkFlow.PlaywrightTests.csproj
```

## 📚 Resources

- [Playwright Documentation](https://playwright.dev/dotnet/)
- [NUnit Documentation](https://docs.nunit.org/)
- [Page Object Model Pattern](https://playwright.dev/dotnet/pom)
- [TeamWorkFlow Application Documentation](../README.md)
- [Security Guidelines](SECURITY.md)
