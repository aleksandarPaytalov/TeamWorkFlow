#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Runs Playwright tests specifically for error pages in the TeamWorkFlow application.

.DESCRIPTION
    This script runs the error page tests with proper configuration and reporting.
    It includes both basic error page tests and error scenario tests.

.PARAMETER Browser
    The browser to run tests in. Options: chromium, firefox, webkit, all
    Default: chromium

.PARAMETER Headed
    Run tests in headed mode (visible browser window)
    Default: false (headless)

.PARAMETER Debug
    Run tests in debug mode with additional logging
    Default: false

.PARAMETER Workers
    Number of parallel workers to use
    Default: 1

.PARAMETER Timeout
    Test timeout in milliseconds
    Default: 30000 (30 seconds)

.PARAMETER BaseUrl
    Base URL of the application to test
    Default: http://localhost:7015

.EXAMPLE
    .\run-error-tests.ps1
    Runs error page tests in headless Chrome

.EXAMPLE
    .\run-error-tests.ps1 -Browser firefox -Headed
    Runs error page tests in Firefox with visible browser

.EXAMPLE
    .\run-error-tests.ps1 -Debug -BaseUrl "https://localhost:7015"
    Runs error page tests in debug mode against HTTPS endpoint
#>

param(
    [ValidateSet("chromium", "firefox", "webkit", "all")]
    [string]$Browser = "chromium",
    
    [switch]$Headed,
    
    [switch]$Debug,
    
    [int]$Workers = 1,
    
    [int]$Timeout = 30000,
    
    [string]$BaseUrl = "http://localhost:7015"
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Colors for output
$Green = "`e[32m"
$Red = "`e[31m"
$Yellow = "`e[33m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = $Reset)
    Write-Host "$Color$Message$Reset"
}

function Test-ApplicationRunning {
    param([string]$Url)
    
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Head -TimeoutSec 5 -UseBasicParsing
        return $response.StatusCode -eq 200
    }
    catch {
        return $false
    }
}

# Main script
Write-ColorOutput "🧪 TeamWorkFlow Error Page Tests" $Blue
Write-ColorOutput "=================================" $Blue
Write-Host ""

# Check if application is running
Write-ColorOutput "🔍 Checking if application is running at $BaseUrl..." $Yellow
if (-not (Test-ApplicationRunning $BaseUrl)) {
    Write-ColorOutput "❌ Application is not running at $BaseUrl" $Red
    Write-ColorOutput "Please start the application first:" $Yellow
    Write-ColorOutput "  dotnet run --project TeamWorkFlow" $Yellow
    exit 1
}
Write-ColorOutput "✅ Application is running" $Green
Write-Host ""

# Set environment variables
$env:APP_BASE_URL = $BaseUrl
$env:PLAYWRIGHT_TIMEOUT = $Timeout

# Build test arguments
$testArgs = @(
    "test"
    "--logger", "console;verbosity=normal"
    "--filter", "FullyQualifiedName~ErrorPageTests|FullyQualifiedName~ErrorScenarioTests"
)

if ($Debug) {
    $testArgs += "--logger", "trx;LogFileName=error-tests.trx"
    $testArgs += "--verbosity", "diagnostic"
}

# Playwright specific arguments
$playwrightArgs = @()

if ($Browser -ne "all") {
    $playwrightArgs += "--browser", $Browser
}

if ($Headed) {
    $playwrightArgs += "--headed"
}

if ($Workers -gt 1) {
    $playwrightArgs += "--workers", $Workers
}

if ($Debug) {
    $playwrightArgs += "--debug"
}

# Combine arguments
if ($playwrightArgs.Count -gt 0) {
    $testArgs += "--"
    $testArgs += $playwrightArgs
}

Write-ColorOutput "🚀 Running error page tests..." $Blue
Write-ColorOutput "Browser: $Browser" $Yellow
Write-ColorOutput "Headed: $Headed" $Yellow
Write-ColorOutput "Workers: $Workers" $Yellow
Write-ColorOutput "Timeout: $Timeout ms" $Yellow
Write-ColorOutput "Base URL: $BaseUrl" $Yellow
Write-Host ""

# Run the tests
try {
    $startTime = Get-Date
    
    & dotnet $testArgs
    $exitCode = $LASTEXITCODE
    
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    Write-Host ""
    Write-ColorOutput "⏱️  Test execution completed in $($duration.TotalSeconds.ToString('F2')) seconds" $Blue
    
    if ($exitCode -eq 0) {
        Write-ColorOutput "✅ All error page tests passed!" $Green
    } else {
        Write-ColorOutput "❌ Some error page tests failed!" $Red
    }
    
    # Show additional information if debug mode
    if ($Debug) {
        Write-Host ""
        Write-ColorOutput "📊 Additional Information:" $Blue
        Write-ColorOutput "- Test results: error-tests.trx" $Yellow
        Write-ColorOutput "- Screenshots: screenshots/" $Yellow
        Write-ColorOutput "- Playwright report: playwright-report/" $Yellow
        
        if (Test-Path "playwright-report/index.html") {
            Write-ColorOutput "- Open report: start playwright-report/index.html" $Yellow
        }
    }
    
    exit $exitCode
}
catch {
    Write-ColorOutput "❌ Error running tests: $($_.Exception.Message)" $Red
    exit 1
}
