# Error Page Tests Documentation

## 🎯 Overview

This document describes the comprehensive Playwright test suite for the TeamWorkFlow application's error pages. The tests ensure that all seven common HTTP error pages are properly implemented, responsive, and provide a good user experience.

## 📋 Error Pages Tested

### Core Error Pages
1. **400 - Bad Request** - Malformed request syntax
2. **401 - Unauthorized** - Authentication required
3. **403 - Forbidden** - Access denied
4. **404 - Not Found** - Resource not found
5. **408 - Request Timeout** - Server timeout
6. **429 - Too Many Requests** - Rate limiting
7. **500 - Internal Server Error** - Server error

### Additional Coverage
- **Custom Error Codes** - Generic error page for other status codes
- **Non-existent Routes** - Real 404 scenarios

## 🧪 Test Structure

### Test Files

#### `ErrorPageTests.cs`
Core functionality tests for error pages:
- ✅ Error page accessibility
- ✅ Correct content display for each error type
- ✅ Proper icons and messaging
- ✅ Action button functionality
- ✅ Responsive design across devices
- ✅ Accessibility compliance
- ✅ Font Awesome icon loading
- ✅ Hover effects and transitions

#### `ErrorScenarioTests.cs`
Real-world error scenario tests:
- ✅ Unauthorized access handling
- ✅ Invalid route navigation
- ✅ Malformed request handling
- ✅ Server error responses
- ✅ Session state maintenance
- ✅ Browser navigation (back/forward)
- ✅ Page refresh handling
- ✅ JavaScript-disabled scenarios

#### `ErrorPage.cs` (Page Object)
Page object model for error pages with methods for:
- Navigation and triggering errors
- Element verification and interaction
- Responsive design validation
- Accessibility checking
- Icon and styling verification

## 🚀 Running the Tests

### Quick Start
```bash
# Run all error page tests
.\run-error-tests.ps1

# Run in headed mode (visible browser)
.\run-error-tests.ps1 -Headed

# Run with debug output
.\run-error-tests.ps1 -Debug

# Run in Firefox
.\run-error-tests.ps1 -Browser firefox -Headed
```

### Advanced Options
```bash
# Custom base URL
.\run-error-tests.ps1 -BaseUrl "https://localhost:7015"

# Multiple workers for parallel execution
.\run-error-tests.ps1 -Workers 4

# Custom timeout
.\run-error-tests.ps1 -Timeout 60000
```

### Manual Test Execution
```bash
# Build the tests
dotnet build TeamWorkFlow.PlaywrightTests

# Run specific test class
dotnet test TeamWorkFlow.PlaywrightTests --filter "FullyQualifiedName~ErrorPageTests"

# Run specific test method
dotnet test TeamWorkFlow.PlaywrightTests --filter "TestMethodName=ErrorPage_ShouldDisplayCorrectContent"
```

## 📱 Responsive Design Testing

The tests verify error pages work correctly across different screen sizes:

### Desktop (1920x1080)
- Full layout with side-by-side buttons
- Large error codes and icons
- Complete error descriptions

### Tablet (768x1024)
- Responsive layout adjustments
- Proper button sizing
- Readable text scaling

### Mobile (375x667)
- Stacked button layout
- Optimized font sizes
- Touch-friendly interactions

## ♿ Accessibility Testing

Error pages are tested for accessibility compliance:

### Semantic Structure
- ✅ Proper heading hierarchy (h1 for error titles)
- ✅ Meaningful link text
- ✅ Logical tab order

### Visual Design
- ✅ High contrast text
- ✅ Readable font sizes
- ✅ Clear visual hierarchy

### Interactive Elements
- ✅ Keyboard navigation
- ✅ Focus indicators
- ✅ Screen reader compatibility

## 🎨 Visual Design Verification

### Modern Design Elements
- ✅ Gradient backgrounds
- ✅ Glass-morphism effects
- ✅ Smooth transitions and hover effects
- ✅ Consistent color scheme

### Icons and Branding
- ✅ Font Awesome icons load correctly
- ✅ Appropriate icons for each error type
- ✅ Consistent styling across all pages

## 🔧 Test Configuration

### Prerequisites
1. **Application Running**: The TeamWorkFlow application must be running
2. **Database**: Test database should be accessible
3. **Playwright**: Browsers should be installed (`playwright install`)

### Environment Variables
- `APP_BASE_URL`: Application base URL (default: http://localhost:7015)
- `PLAYWRIGHT_TIMEOUT`: Test timeout in milliseconds (default: 30000)

### Test Data
Tests use the ErrorTestController to trigger specific error conditions:
- `/ErrorTest/Test400` - Triggers 400 error
- `/ErrorTest/Test401` - Triggers 401 error
- `/ErrorTest/Test404` - Triggers 404 error
- etc.

## 📊 Test Coverage

### Functional Coverage
- ✅ All 7 common error pages
- ✅ Custom error page fallback
- ✅ Navigation functionality
- ✅ Session state preservation
- ✅ Browser compatibility

### UI/UX Coverage
- ✅ Responsive design (3 breakpoints)
- ✅ Accessibility compliance
- ✅ Visual design consistency
- ✅ Interactive elements
- ✅ Loading states

### Error Scenario Coverage
- ✅ Real unauthorized access
- ✅ Invalid route handling
- ✅ Server error responses
- ✅ Browser navigation
- ✅ JavaScript-disabled scenarios

## 🐛 Troubleshooting

### Common Issues

#### Application Not Running
```
❌ Application is not running at http://localhost:7015
```
**Solution**: Start the application with `dotnet run --project TeamWorkFlow`

#### Browser Not Installed
```
❌ Browser 'chromium' is not installed
```
**Solution**: Run `playwright install` in the test project directory

#### Test Timeouts
```
❌ Test timed out after 30000ms
```
**Solution**: Increase timeout with `-Timeout 60000` or check application performance

### Debug Mode
Run tests with `-Debug` flag for additional information:
- Detailed test execution logs
- Screenshot capture on failures
- Test result files (TRX format)
- Playwright HTML reports

## 📈 Continuous Integration

### CI/CD Integration
The error page tests are designed to run in CI/CD pipelines:

```yaml
- name: Run Error Page Tests
  run: |
    dotnet test TeamWorkFlow.PlaywrightTests \
      --filter "FullyQualifiedName~ErrorPageTests|FullyQualifiedName~ErrorScenarioTests" \
      --logger "trx;LogFileName=error-tests.trx"
```

### Parallel Execution
Tests can run in parallel for faster execution:
```bash
.\run-error-tests.ps1 -Workers 4
```

## 🔮 Future Enhancements

### Planned Improvements
- [ ] Performance testing for error page load times
- [ ] Cross-browser compatibility matrix
- [ ] Automated visual regression testing
- [ ] Error page analytics integration testing
- [ ] Internationalization (i18n) testing

### Test Expansion
- [ ] Error page SEO testing
- [ ] Error page caching behavior
- [ ] Error page security headers
- [ ] Error page monitoring integration

## 📚 Resources

- [Playwright Documentation](https://playwright.dev/dotnet/)
- [TeamWorkFlow Error Pages Implementation](../TeamWorkFlow/Views/Home/)
- [Error Page Design Guidelines](../TeamWorkFlow/Views/Shared/_ErrorLayout.cshtml)
- [Test Configuration](./appsettings.json)
