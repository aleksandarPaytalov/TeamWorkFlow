# TeamWorkFlow Playwright Test Runner
# This script runs Playwright tests with various options and generates reports

param(
    [string]$TestFilter = "",
    [string]$Browser = "chromium",
    [switch]$Headed = $false,
    [switch]$Debug = $false,
    [switch]$UpdateSnapshots = $false,
    [switch]$OpenReport = $true,
    [string]$OutputDir = "TestResults",
    [switch]$Verbose = $false
)

Write-Host "🎭 TeamWorkFlow Playwright Test Runner" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Set working directory to script location
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

# Ensure the application is built
Write-Host "🔨 Building application..." -ForegroundColor Yellow
try {
    dotnet build ../TeamWorkFlow/TeamWorkFlow.csproj --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Application build failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "✅ Application built successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error building application: $_" -ForegroundColor Red
    exit 1
}

# Build test project
Write-Host "🔨 Building test project..." -ForegroundColor Yellow
try {
    dotnet build TeamWorkFlow.PlaywrightTests.csproj
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Test project build failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "✅ Test project built successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error building test project: $_" -ForegroundColor Red
    exit 1
}

# Install Playwright browsers if needed
Write-Host "🌐 Checking Playwright browsers..." -ForegroundColor Yellow
try {
    pwsh bin/Debug/net6.0/playwright.ps1 install --with-deps
    Write-Host "✅ Playwright browsers ready!" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Warning: Could not install Playwright browsers: $_" -ForegroundColor Yellow
}

# Clean previous results
if (Test-Path $OutputDir) {
    Write-Host "🧹 Cleaning previous test results..." -ForegroundColor Yellow
    Remove-Item $OutputDir -Recurse -Force
}

# Create output directory
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

# Prepare test command
$testCommand = "dotnet test TeamWorkFlow.PlaywrightTests.csproj"
$testCommand += " --results-directory $OutputDir"
$testCommand += " --logger trx"
$testCommand += " --logger html"

# Add test filter if specified
if ($TestFilter) {
    $testCommand += " --filter `"$TestFilter`""
    Write-Host "🔍 Running filtered tests: $TestFilter" -ForegroundColor Blue
}

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

# Add update snapshots if requested
if ($UpdateSnapshots) {
    $playwrightArgs += "Playwright.UpdateSnapshots=true"
    Write-Host "📸 Updating visual snapshots" -ForegroundColor Blue
}

# Add Playwright arguments to command
if ($playwrightArgs.Count -gt 0) {
    $testCommand += " -- " + ($playwrightArgs -join " ")
}

# Add verbosity
if ($Verbose) {
    $testCommand += " --verbosity detailed"
    Write-Host "Executing: $testCommand" -ForegroundColor Gray
} else {
    $testCommand += " --verbosity normal"
}

Write-Host "🧪 Running Playwright tests..." -ForegroundColor Green
Write-Host "Command: $testCommand" -ForegroundColor Gray

# Start application in background
Write-Host "🚀 Starting TeamWorkFlow application..." -ForegroundColor Yellow
$appProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project ../TeamWorkFlow/TeamWorkFlow.csproj" -PassThru -WindowStyle Hidden

# Wait a moment for application to start
Start-Sleep -Seconds 5

try {
    # Run the tests
    Invoke-Expression $testCommand
    $testExitCode = $LASTEXITCODE
    
    if ($testExitCode -eq 0) {
        Write-Host "✅ All tests passed!" -ForegroundColor Green
    } else {
        Write-Host "❌ Some tests failed (exit code: $testExitCode)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Error running tests: $_" -ForegroundColor Red
    $testExitCode = 1
} finally {
    # Stop the application
    if ($appProcess -and !$appProcess.HasExited) {
        Write-Host "🛑 Stopping TeamWorkFlow application..." -ForegroundColor Yellow
        Stop-Process -Id $appProcess.Id -Force -ErrorAction SilentlyContinue
    }
}

# Display results
Write-Host "`n📊 Test Results:" -ForegroundColor Cyan
Write-Host "===============" -ForegroundColor Cyan

# Find and display test results
$trxFiles = Get-ChildItem -Path $OutputDir -Recurse -Filter "*.trx"
if ($trxFiles.Count -gt 0) {
    $trxFile = $trxFiles[0].FullName
    Write-Host "📋 TRX Report: $trxFile" -ForegroundColor White
}

$htmlFiles = Get-ChildItem -Path $OutputDir -Recurse -Filter "*.html"
if ($htmlFiles.Count -gt 0) {
    $htmlFile = $htmlFiles[0].FullName
    Write-Host "🌐 HTML Report: $htmlFile" -ForegroundColor White
}

# Check for Playwright report
$playwrightReport = "playwright-report/index.html"
if (Test-Path $playwrightReport) {
    Write-Host "🎭 Playwright Report: $playwrightReport" -ForegroundColor White
    
    if ($OpenReport) {
        Write-Host "`n🌐 Opening Playwright report in browser..." -ForegroundColor Green
        Start-Process $playwrightReport
    }
}

# Check for screenshots
$screenshotsDir = "screenshots"
if (Test-Path $screenshotsDir) {
    $screenshots = Get-ChildItem -Path $screenshotsDir -Filter "*.png"
    if ($screenshots.Count -gt 0) {
        Write-Host "📸 Screenshots: $($screenshots.Count) files in $screenshotsDir/" -ForegroundColor Yellow
    }
}

# Display summary
Write-Host "`n📈 Summary:" -ForegroundColor Cyan
Write-Host "==========" -ForegroundColor Cyan
Write-Host "Browser: $Browser" -ForegroundColor White
Write-Host "Headed Mode: $Headed" -ForegroundColor White
Write-Host "Debug Mode: $Debug" -ForegroundColor White
if ($TestFilter) {
    Write-Host "Filter: $TestFilter" -ForegroundColor White
}

if ($testExitCode -eq 0) {
    Write-Host "`n🎉 Test execution completed successfully!" -ForegroundColor Green
} else {
    Write-Host "`n💥 Test execution completed with failures!" -ForegroundColor Red
}

exit $testExitCode
