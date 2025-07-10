# TeamWorkFlow Single Test Runner
# This script runs a single Playwright test for debugging

param(
    [string]$TestName = "LoginPage_ShouldLoadCorrectly",
    [string]$Browser = "chromium",
    [switch]$Headed = $false,
    [switch]$Debug = $false
)

Write-Host "🎭 TeamWorkFlow Single Test Runner" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

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

# Prepare test command
$testCommand = "dotnet test TeamWorkFlow.PlaywrightTests.csproj"
$testCommand += " --filter `"TestName=$TestName`""
$testCommand += " --logger console"
$testCommand += " --verbosity normal"

# Add browser configuration
$playwrightArgs = @()
if ($Browser) {
    $playwrightArgs += "Playwright.BrowserName=$Browser"
    Write-Host "🌐 Using browser: $Browser" -ForegroundColor Blue
}

# Add headed mode if requested
if ($Headed) {
    $playwrightArgs += "Playwright.LaunchOptions.Headless=false"
    Write-Host "👁️ Running in headed mode (visible browser)" -ForegroundColor Blue
}

# Add debug mode if requested
if ($Debug) {
    $playwrightArgs += "Playwright.LaunchOptions.SlowMo=1000"
    $playwrightArgs += "Playwright.LaunchOptions.Devtools=true"
    Write-Host "🐛 Running in debug mode (slow motion + devtools)" -ForegroundColor Blue
}

# Add Playwright arguments to command
if ($playwrightArgs.Count -gt 0) {
    $testCommand += " -- " + ($playwrightArgs -join " ")
}

Write-Host "🧪 Running test: $TestName" -ForegroundColor Green
Write-Host "Command: $testCommand" -ForegroundColor Gray

# Run the test
try {
    Invoke-Expression $testCommand
    $testExitCode = $LASTEXITCODE
    
    if ($testExitCode -eq 0) {
        Write-Host "✅ Test passed!" -ForegroundColor Green
    } else {
        Write-Host "❌ Test failed (exit code: $testExitCode)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Error running test: $_" -ForegroundColor Red
    $testExitCode = 1
}

# Check for screenshots
$screenshotsDir = "screenshots"
if (Test-Path $screenshotsDir) {
    $screenshots = Get-ChildItem -Path $screenshotsDir -Filter "*.png" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($screenshots) {
        Write-Host "📸 Latest screenshot: $($screenshots.FullName)" -ForegroundColor Yellow
    }
}

Write-Host "`n🎯 Test execution completed!" -ForegroundColor Cyan
exit $testExitCode
