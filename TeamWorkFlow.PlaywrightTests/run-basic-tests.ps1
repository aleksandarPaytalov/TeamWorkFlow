# TeamWorkFlow Basic Playwright Tests
# This script runs the basic working tests

Write-Host "🎭 TeamWorkFlow Basic Playwright Tests" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

# Set working directory to script location
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

# Check if application is running
Write-Host "🔍 Checking if TeamWorkFlow application is running..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "https://localhost:7015" -UseBasicParsing -TimeoutSec 5 -SkipCertificateCheck
    Write-Host "✅ Application is running!" -ForegroundColor Green
} catch {
    Write-Host "❌ Application is not running at https://localhost:7015" -ForegroundColor Red
    Write-Host "Please start the application first:" -ForegroundColor Yellow
    Write-Host "  cd ../TeamWorkFlow" -ForegroundColor Gray
    Write-Host "  dotnet run" -ForegroundColor Gray
    exit 1
}

# Build the test project
Write-Host "🔨 Building test project..." -ForegroundColor Yellow
try {
    dotnet build TeamWorkFlow.PlaywrightTests.csproj
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to build test project!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "✅ Test project built successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error building test project: $_" -ForegroundColor Red
    exit 1
}

# Define tests to run
$tests = @(
    @{Name="Application_ShouldBeAccessible"; Description="Test application connectivity"},
    @{Name="BasicLogin_ShouldWork"; Description="Test basic login functionality"},
    @{Name="QuickLogin_ShouldWork"; Description="Test quick login with user detection"}
)

$passedTests = 0
$failedTests = 0
$totalTests = $tests.Count

Write-Host "`n🧪 Running $totalTests basic tests..." -ForegroundColor Green

foreach ($test in $tests) {
    Write-Host "`n📋 Running: $($test.Description)" -ForegroundColor Blue
    Write-Host "Test: $($test.Name)" -ForegroundColor Gray
    
    try {
        $result = dotnet test --filter "Name=$($test.Name)" --logger console --verbosity minimal
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ PASSED: $($test.Name)" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "❌ FAILED: $($test.Name)" -ForegroundColor Red
            $failedTests++
        }
    } catch {
        Write-Host "❌ ERROR: $($test.Name) - $_" -ForegroundColor Red
        $failedTests++
    }
}

# Summary
Write-Host "`n📊 Test Results Summary:" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan
Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor Red

if ($failedTests -eq 0) {
    Write-Host "`n🎉 All basic tests passed!" -ForegroundColor Green
    Write-Host "Your Playwright setup is working correctly!" -ForegroundColor Green
    
    Write-Host "`n📋 Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Run more comprehensive tests:" -ForegroundColor White
    Write-Host "   dotnet test --filter 'ClassName=AuthenticationTests'" -ForegroundColor Gray
    Write-Host "2. Run all tests:" -ForegroundColor White
    Write-Host "   dotnet test" -ForegroundColor Gray
    Write-Host "3. Run tests in headed mode to see browser:" -ForegroundColor White
    Write-Host "   dotnet test -- Playwright.LaunchOptions.Headless=false" -ForegroundColor Gray
} else {
    Write-Host "`n💥 Some tests failed!" -ForegroundColor Red
    Write-Host "Check the output above for details." -ForegroundColor Yellow
    
    Write-Host "`n🔧 Troubleshooting:" -ForegroundColor Cyan
    Write-Host "1. Make sure your application is running" -ForegroundColor White
    Write-Host "2. Check test user credentials in appsettings.json" -ForegroundColor White
    Write-Host "3. Run tests in headed mode to see what's happening:" -ForegroundColor White
    Write-Host "   dotnet test --filter 'Name=BasicLogin_ShouldWork' -- Playwright.LaunchOptions.Headless=false" -ForegroundColor Gray
}

Write-Host "`n📚 Documentation:" -ForegroundColor Cyan
Write-Host "• README.md - Detailed guide" -ForegroundColor White
Write-Host "• PLAYWRIGHT_FIXES_SUMMARY.md - Recent fixes" -ForegroundColor White

exit $failedTests
