# TeamWorkFlow Playwright Security Verification Script
# This script verifies that security measures are properly implemented

Write-Host "Security Verification for TeamWorkFlow Playwright Tests" -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host ""

$issues = @()
$warnings = @()
$passed = @()

# Check 1: Verify appsettings.json contains only placeholders
Write-Host "Checking appsettings.json for hardcoded credentials..." -ForegroundColor Yellow
$appsettingsContent = Get-Content "appsettings.json" -Raw
if ($appsettingsContent -match 'PLACEHOLDER') {
    $passed += "PASS: appsettings.json uses placeholder values"
} else {
    $warnings += "WARN: Could not verify appsettings.json placeholder usage"
}

# Simple check for obvious real credentials
if ($appsettingsContent -match 'gmail\.com' -or $appsettingsContent -match 'softuni\.bg' -or $appsettingsContent -match '1234aA!' -or $appsettingsContent -match '1234bB!') {
    $issues += "FAIL: Potential real credentials found in appsettings.json"
} else {
    $passed += "PASS: No obvious real credentials in appsettings.json"
}

# Check 2: Verify .gitignore excludes sensitive files
Write-Host "Checking .gitignore for security exclusions..." -ForegroundColor Yellow
$gitignoreContent = Get-Content "..\.gitignore" -Raw -ErrorAction SilentlyContinue
if ($gitignoreContent -match "TeamWorkFlow\.PlaywrightTests/\.env") {
    $passed += "PASS: .gitignore excludes .env files"
} else {
    $issues += "FAIL: .gitignore does not exclude .env files"
}

# Check 3: Verify security files exist
Write-Host "Checking security documentation and setup files..." -ForegroundColor Yellow
$securityFiles = @(
    @{File = "SECURITY.md"; Description = "Security documentation"},
    @{File = ".env.example"; Description = "Environment variable example"},
    @{File = "setup-test-environment.ps1"; Description = "Secure setup script"},
    @{File = "appsettings.Development.json"; Description = "Development configuration"}
)

foreach ($fileCheck in $securityFiles) {
    if (Test-Path $fileCheck.File) {
        $passed += "PASS: $($fileCheck.Description) exists"
    } else {
        $issues += "FAIL: Missing $($fileCheck.Description): $($fileCheck.File)"
    }
}

# Check 4: Verify no .env file is committed
Write-Host "Checking for committed .env files..." -ForegroundColor Yellow
if (Test-Path ".env") {
    $warnings += "WARN: .env file exists locally (ensure it's not committed)"
} else {
    $passed += "PASS: No .env file found (good for security)"
}

# Display Results
Write-Host ""
Write-Host "Security Verification Results:" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan
Write-Host ""

if ($passed.Count -gt 0) {
    Write-Host "PASSED ($($passed.Count)):" -ForegroundColor Green
    foreach ($pass in $passed) {
        Write-Host "   $pass" -ForegroundColor Green
    }
    Write-Host ""
}

if ($warnings.Count -gt 0) {
    Write-Host "WARNINGS ($($warnings.Count)):" -ForegroundColor Yellow
    foreach ($warning in $warnings) {
        Write-Host "   $warning" -ForegroundColor Yellow
    }
    Write-Host ""
}

if ($issues.Count -gt 0) {
    Write-Host "ISSUES ($($issues.Count)):" -ForegroundColor Red
    foreach ($issue in $issues) {
        Write-Host "   $issue" -ForegroundColor Red
    }
    Write-Host ""
    Write-Host "Security issues found! Please address them before committing." -ForegroundColor Red
    exit 1
} else {
    Write-Host "All security checks passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Set up test credentials: .\setup-test-environment.ps1 -SetEnvironmentVariables" -ForegroundColor White
    Write-Host "2. Run tests: dotnet test" -ForegroundColor White
    Write-Host "3. Review security guide: SECURITY.md" -ForegroundColor White
    exit 0
}
