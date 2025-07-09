# TeamWorkFlow Test Coverage Analysis Script
# This script runs unit tests with code coverage and generates detailed HTML reports

param(
    [string]$OutputDir = "TestResults",
    [switch]$OpenReport = $true,
    [switch]$Verbose = $false
)

Write-Host "🧪 TeamWorkFlow Test Coverage Analysis" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

# Set working directory to script location
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

# Clean previous results
if (Test-Path $OutputDir) {
    Write-Host "🧹 Cleaning previous test results..." -ForegroundColor Yellow
    Remove-Item $OutputDir -Recurse -Force
}

# Create output directory
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

Write-Host "🔍 Running tests with coverage collection..." -ForegroundColor Green

# Run tests with coverage collection
$testCommand = "dotnet test UnitTests/UnitTests.csproj --collect:`"XPlat Code Coverage`" --results-directory $OutputDir --logger trx --verbosity normal"

if ($Verbose) {
    Write-Host "Executing: $testCommand" -ForegroundColor Gray
}

try {
    Invoke-Expression $testCommand
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Tests failed with exit code $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    
    Write-Host "✅ Tests completed successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error running tests: $_" -ForegroundColor Red
    exit 1
}

# Find coverage file
$coverageFiles = Get-ChildItem -Path $OutputDir -Recurse -Filter "coverage.cobertura.xml"

if ($coverageFiles.Count -eq 0) {
    Write-Host "❌ No coverage files found!" -ForegroundColor Red
    exit 1
}

$coverageFile = $coverageFiles[0].FullName
Write-Host "📊 Found coverage file: $($coverageFile)" -ForegroundColor Blue

# Generate HTML report
$reportDir = Join-Path $OutputDir "CoverageReport"
Write-Host "📈 Generating HTML coverage report..." -ForegroundColor Green

$reportCommand = "reportgenerator -reports:`"$coverageFile`" -targetdir:`"$reportDir`" -reporttypes:`"Html;HtmlSummary;Badges;TextSummary`" -verbosity:Info"

if ($Verbose) {
    Write-Host "Executing: $reportCommand" -ForegroundColor Gray
}

try {
    Invoke-Expression $reportCommand
    Write-Host "✅ Coverage report generated successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error generating report: $_" -ForegroundColor Red
    exit 1
}

# Display summary
Write-Host "`n📋 Coverage Summary:" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan

$summaryFile = Join-Path $reportDir "Summary.txt"
if (Test-Path $summaryFile) {
    Get-Content $summaryFile | Write-Host
} else {
    Write-Host "Summary file not found, but report was generated." -ForegroundColor Yellow
}

# Display file locations
Write-Host "`n📁 Generated Files:" -ForegroundColor Cyan
Write-Host "==================" -ForegroundColor Cyan
Write-Host "📊 HTML Report: $reportDir\index.html" -ForegroundColor White
Write-Host "📋 Summary: $reportDir\Summary.txt" -ForegroundColor White
Write-Host "🏆 Badges: $reportDir\badge_*.svg" -ForegroundColor White

# Open report if requested
if ($OpenReport) {
    $indexFile = Join-Path $reportDir "index.html"
    if (Test-Path $indexFile) {
        Write-Host "`n🌐 Opening coverage report in browser..." -ForegroundColor Green
        Start-Process $indexFile
    }
}

Write-Host "`n✨ Coverage analysis complete!" -ForegroundColor Green
