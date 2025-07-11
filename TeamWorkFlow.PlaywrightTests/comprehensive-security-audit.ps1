# Comprehensive Security Audit for TeamWorkFlow
# This script performs a thorough security vulnerability assessment

Write-Host "TeamWorkFlow Comprehensive Security Audit" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

$criticalIssues = @()
$warnings = @()
$passed = @()
$informational = @()

# Function to check file for sensitive patterns
function Test-FileForSensitiveData {
    param(
        [string]$FilePath,
        [string]$Description
    )
    
    if (-not (Test-Path $FilePath)) {
        return @{
            Status = "NotFound"
            Message = "File not found: $FilePath"
        }
    }
    
    $content = Get-Content $FilePath -Raw -ErrorAction SilentlyContinue
    if (-not $content) {
        return @{
            Status = "Empty"
            Message = "File is empty or unreadable: $FilePath"
        }
    }
    
    $sensitivePatterns = @{
        "RealEmails" = @("ap\.softuni@gmail\.com", "jon\.doe@softuni\.bg", "jane\.doe@softuni\.bg")
        "RealPasswords" = @("1234aA!", "1234bB!", "1234cC!")
        "ConnectionStrings" = @("Server=", "Database=", "Data Source=", "Initial Catalog=")
        "APIKeys" = @("api[_-]?key", "secret[_-]?key", "access[_-]?token")
        "PrivateKeys" = @("-----BEGIN PRIVATE KEY-----", "-----BEGIN RSA PRIVATE KEY-----")
        "AWSCredentials" = @("AKIA[0-9A-Z]{16}", "aws_access_key_id", "aws_secret_access_key")
        "AzureCredentials" = @("DefaultEndpointsProtocol=https", "AccountName=", "AccountKey=")
    }
    
    $findings = @()
    foreach ($category in $sensitivePatterns.Keys) {
        foreach ($pattern in $sensitivePatterns[$category]) {
            if ($content -match $pattern) {
                $findings += "$category pattern found: $pattern"
            }
        }
    }
    
    return @{
        Status = if ($findings.Count -gt 0) { "HasSensitiveData" } else { "Clean" }
        Findings = $findings
        Message = "$Description - $($findings.Count) sensitive patterns found"
    }
}

Write-Host "Phase 1: Configuration File Security Audit" -ForegroundColor Yellow
Write-Host "===========================================" -ForegroundColor Yellow

# Check main Playwright test configuration
$playwrightMainConfig = Test-FileForSensitiveData "appsettings.json" "Playwright Main Config"
if ($playwrightMainConfig.Status -eq "HasSensitiveData") {
    $mainFindingsList = $playwrightMainConfig.Findings -join ", "
    $criticalIssues += "CRITICAL: Sensitive data in appsettings.json - $mainFindingsList"
} elseif ($playwrightMainConfig.Status -eq "Clean") {
    $passed += "PASS: appsettings.json is clean (placeholders only)"
}

# Check development configuration
$playwrightDevConfig = Test-FileForSensitiveData "appsettings.Development.json" "Playwright Dev Config"
if ($playwrightDevConfig.Status -eq "HasSensitiveData") {
    # Check if these are acceptable test credentials
    $devContent = Get-Content "appsettings.Development.json" -Raw
    if ($devContent -match "@test\.local" -or $devContent -match "TestPass") {
        $passed += "PASS: Development config uses generic test credentials"
        $informational += "INFO: Development credentials are generic and safe for testing"
    } else {
        $devFindingsList = $playwrightDevConfig.Findings -join ", "
        $warnings += "WARN: Development config contains real credentials - $devFindingsList"
        $informational += "INFO: Development credentials should match database seeding for local testing"
    }
} elseif ($playwrightDevConfig.Status -eq "Clean") {
    $passed += "PASS: Development config is clean and secure"
}

# Check main application configurations
$appMainConfig = Test-FileForSensitiveData "..\TeamWorkFlow\appsettings.json" "Main App Config"
if ($appMainConfig.Status -eq "HasSensitiveData") {
    $appMainFindingsList = $appMainConfig.Findings -join ", "
    $criticalIssues += "CRITICAL: Sensitive data in main application config - $appMainFindingsList"
} elseif ($appMainConfig.Status -eq "Clean") {
    $passed += "PASS: Main application config is clean"
}

$appDevConfig = Test-FileForSensitiveData "..\TeamWorkFlow\appsettings.Development.json" "Main App Dev Config"
if ($appDevConfig.Status -eq "HasSensitiveData") {
    $appDevFindingsList = $appDevConfig.Findings -join ", "
    $warnings += "WARN: Sensitive data in main app development config - $appDevFindingsList"
} elseif ($appDevConfig.Status -eq "Clean") {
    $passed += "PASS: Main app development config is clean"
}

Write-Host "Phase 2: Documentation Security Review" -ForegroundColor Yellow
Write-Host "=======================================" -ForegroundColor Yellow

# Check documentation files for exposed credentials
$docFiles = @(
    @{Path = "README.md"; Description = "Playwright README"},
    @{Path = "..\README.md"; Description = "Main README"},
    @{Path = "SECURITY.md"; Description = "Security Documentation"},
    @{Path = "..\PLAYWRIGHT_IMPLEMENTATION_SUMMARY.md"; Description = "Playwright Implementation Summary"}
)

foreach ($docFile in $docFiles) {
    $docCheck = Test-FileForSensitiveData $docFile.Path $docFile.Description
    if ($docCheck.Status -eq "HasSensitiveData") {
        # Check if these are acceptable documentation examples
        $docContent = Get-Content $docFile.Path -Raw -ErrorAction SilentlyContinue
        if ($docContent -match "\[YOUR_CONNECTION_STRING_HERE\]" -or $docContent -match "YOUR_SERVER_NAME" -or $docContent -match "PLACEHOLDER") {
            $passed += "PASS: $($docFile.Description) uses placeholder examples"
        } else {
            $findingsList = $docCheck.Findings -join ", "
            $warnings += "WARN: Sensitive data in documentation - $($docFile.Description): $findingsList"
        }
    } elseif ($docCheck.Status -eq "Clean") {
        $passed += "PASS: $($docFile.Description) is clean"
    }
}

Write-Host "Phase 3: Git and Version Control Security" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# Check .gitignore effectiveness
$gitignoreContent = Get-Content "..\.gitignore" -Raw -ErrorAction SilentlyContinue
if ($gitignoreContent) {
    $requiredExclusions = @(
        "TeamWorkFlow.PlaywrightTests/.env",
        "TeamWorkFlow.PlaywrightTests/appsettings.Production.json",
        "TeamWorkFlow.PlaywrightTests/appsettings.Staging.json",
        "*.pfx",
        "*.publishsettings"
    )
    
    $missingExclusions = @()
    foreach ($exclusion in $requiredExclusions) {
        if ($gitignoreContent -notmatch [regex]::Escape($exclusion)) {
            $missingExclusions += $exclusion
        }
    }
    
    if ($missingExclusions.Count -gt 0) {
        $exclusionList = $missingExclusions -join ", "
        $warnings += "WARN: Missing .gitignore exclusions: $exclusionList"
    } else {
        $passed += "PASS: .gitignore has required security exclusions"
    }
} else {
    $criticalIssues += "CRITICAL: .gitignore file not found or unreadable"
}

# Check for accidentally committed sensitive files
$sensitiveFiles = @(
    ".env",
    "appsettings.Production.json",
    "appsettings.Staging.json",
    "test-credentials.json",
    "..\TeamWorkFlow\.env",
    "..\TeamWorkFlow\appsettings.Production.json"
)

foreach ($sensitiveFile in $sensitiveFiles) {
    if (Test-Path $sensitiveFile) {
        $criticalIssues += "CRITICAL: Sensitive file found: $sensitiveFile (should be in .gitignore)"
    }
}

$foundSensitiveFiles = $sensitiveFiles | Where-Object { Test-Path $_ }
if ($foundSensitiveFiles.Count -eq 0) {
    $passed += "PASS: No accidentally committed sensitive files found"
}

Write-Host "Phase 4: Environment Variable Security" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Yellow

# Check for environment variables that might contain sensitive data
$envVars = Get-ChildItem Env: | Where-Object { 
    $_.Name -match "(PASSWORD|SECRET|KEY|TOKEN|CONNECTION)" -and 
    $_.Name -notmatch "^(PATH|TEMP|TMP|PROCESSOR|NUMBER_OF_PROCESSORS)$"
}

if ($envVars) {
    $informational += "INFO: Found $($envVars.Count) environment variables with potentially sensitive names"
    foreach ($envVar in $envVars) {
        $informational += "INFO: Environment variable: $($envVar.Name)"
    }
} else {
    $passed += "PASS: No suspicious environment variables detected"
}

Write-Host ""
Write-Host "Comprehensive Security Audit Results" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

if ($criticalIssues.Count -gt 0) {
    Write-Host "CRITICAL ISSUES ($($criticalIssues.Count)):" -ForegroundColor Red
    foreach ($issue in $criticalIssues) {
        Write-Host "   $issue" -ForegroundColor Red
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

if ($passed.Count -gt 0) {
    Write-Host "PASSED ($($passed.Count)):" -ForegroundColor Green
    foreach ($pass in $passed) {
        Write-Host "   $pass" -ForegroundColor Green
    }
    Write-Host ""
}

if ($informational.Count -gt 0) {
    Write-Host "INFORMATIONAL ($($informational.Count)):" -ForegroundColor Cyan
    foreach ($info in $informational) {
        Write-Host "   $info" -ForegroundColor Cyan
    }
    Write-Host ""
}

# Final assessment
if ($criticalIssues.Count -gt 0) {
    Write-Host "SECURITY AUDIT FAILED - Critical issues must be resolved!" -ForegroundColor Red
    exit 1
} elseif ($warnings.Count -gt 0) {
    Write-Host "SECURITY AUDIT PASSED WITH WARNINGS - Review recommended" -ForegroundColor Yellow
    exit 0
} else {
    Write-Host "SECURITY AUDIT PASSED - No critical issues found!" -ForegroundColor Green
    exit 0
}
