# TeamWorkFlow Playwright Test Environment Setup
# This script helps set up secure test credentials using environment variables

param(
    [switch]$SetEnvironmentVariables = $false,
    [switch]$ShowCurrentValues = $false,
    [switch]$Help = $false
)

function Show-Help {
    Write-Host "TeamWorkFlow Playwright Test Environment Setup" -ForegroundColor Cyan
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "This script helps manage secure test credentials for Playwright tests." -ForegroundColor White
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\setup-test-environment.ps1 -SetEnvironmentVariables  # Set environment variables interactively"
    Write-Host "  .\setup-test-environment.ps1 -ShowCurrentValues       # Show current configuration values"
    Write-Host "  .\setup-test-environment.ps1 -Help                    # Show this help"
    Write-Host ""
    Write-Host "Environment Variables Used:" -ForegroundColor Yellow
    Write-Host "  TEST_ADMIN_EMAIL      - Admin user email for tests"
    Write-Host "  TEST_ADMIN_PASSWORD   - Admin user password for tests"
    Write-Host "  TEST_OPERATOR_EMAIL   - Operator user email for tests"
    Write-Host "  TEST_OPERATOR_PASSWORD - Operator user password for tests"
    Write-Host ""
    Write-Host "Security Notes:" -ForegroundColor Red
    Write-Host "- Never commit real credentials to source control"
    Write-Host "- Use test-specific accounts, not production accounts"
    Write-Host "- Environment variables are preferred for CI/CD pipelines"
    Write-Host "- Development credentials are in appsettings.Development.json"
}

function Show-CurrentValues {
    Write-Host "Current Test Configuration Values:" -ForegroundColor Cyan
    Write-Host "=================================" -ForegroundColor Cyan
    Write-Host ""
    
    $adminEmail = $env:TEST_ADMIN_EMAIL
    $adminPassword = $env:TEST_ADMIN_PASSWORD
    $operatorEmail = $env:TEST_OPERATOR_EMAIL
    $operatorPassword = $env:TEST_OPERATOR_PASSWORD
    
    Write-Host "Environment Variables:" -ForegroundColor Yellow
    Write-Host "  TEST_ADMIN_EMAIL:      $(if ($adminEmail) { $adminEmail } else { '(not set)' })"
    Write-Host "  TEST_ADMIN_PASSWORD:   $(if ($adminPassword) { '***' } else { '(not set)' })"
    Write-Host "  TEST_OPERATOR_EMAIL:   $(if ($operatorEmail) { $operatorEmail } else { '(not set)' })"
    Write-Host "  TEST_OPERATOR_PASSWORD: $(if ($operatorPassword) { '***' } else { '(not set)' })"
    Write-Host ""
    
    if (-not ($adminEmail -and $adminPassword -and $operatorEmail -and $operatorPassword)) {
        Write-Host "⚠️  Some environment variables are not set." -ForegroundColor Yellow
        Write-Host "   Tests will use fallback values from appsettings.Development.json" -ForegroundColor Gray
    } else {
        Write-Host "✅ All environment variables are set." -ForegroundColor Green
    }
}

function Set-EnvironmentVariables {
    Write-Host "Setting up Test Environment Variables" -ForegroundColor Cyan
    Write-Host "====================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "⚠️  Important Security Notes:" -ForegroundColor Red
    Write-Host "- Use test-specific accounts only"
    Write-Host "- Never use production credentials"
    Write-Host "- These should be valid accounts in your test database"
    Write-Host ""
    
    # Get admin credentials
    Write-Host "Admin User Credentials:" -ForegroundColor Yellow
    $adminEmail = Read-Host "Enter admin email for tests (e.g., admin@teamworkflow.local)"
    $adminPassword = Read-Host "Enter admin password for tests" -AsSecureString
    $adminPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($adminPassword))
    
    Write-Host ""
    Write-Host "Operator User Credentials:" -ForegroundColor Yellow
    $operatorEmail = Read-Host "Enter operator email for tests (e.g., operator@teamworkflow.local)"
    $operatorPassword = Read-Host "Enter operator password for tests" -AsSecureString
    $operatorPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($operatorPassword))
    
    # Set environment variables for current session
    Write-Host ""
    Write-Host "Setting environment variables for current session..." -ForegroundColor Green
    $env:TEST_ADMIN_EMAIL = $adminEmail
    $env:TEST_ADMIN_PASSWORD = $adminPasswordPlain
    $env:TEST_OPERATOR_EMAIL = $operatorEmail
    $env:TEST_OPERATOR_PASSWORD = $operatorPasswordPlain
    
    Write-Host "✅ Environment variables set for current PowerShell session." -ForegroundColor Green
    Write-Host ""
    Write-Host "To make these permanent, you can:" -ForegroundColor Yellow
    Write-Host "1. Add them to your system environment variables"
    Write-Host "2. Add them to your CI/CD pipeline secrets"
    Write-Host "3. Create a .env file (not recommended for production)"
    Write-Host ""
    Write-Host "For CI/CD (GitHub Actions example):" -ForegroundColor Cyan
    Write-Host "  TEST_ADMIN_EMAIL: $adminEmail"
    Write-Host "  TEST_ADMIN_PASSWORD: ***"
    Write-Host "  TEST_OPERATOR_EMAIL: $operatorEmail"
    Write-Host "  TEST_OPERATOR_PASSWORD: ***"
}

# Main script logic
if ($Help) {
    Show-Help
    exit 0
}

if ($ShowCurrentValues) {
    Show-CurrentValues
    exit 0
}

if ($SetEnvironmentVariables) {
    Set-EnvironmentVariables
    exit 0
}

# Default behavior - show help
Show-Help
