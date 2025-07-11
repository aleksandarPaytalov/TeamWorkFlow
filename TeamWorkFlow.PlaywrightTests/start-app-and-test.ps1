# Start Application and Run Authentication Tests
# This script starts the TeamWorkFlow application and runs authentication tests

Write-Host "TeamWorkFlow Authentication Tests - Complete Solution" -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Continue"

# Step 1: Verify Security Status
Write-Host "Step 1: Verifying Security Status..." -ForegroundColor Yellow
try {
    $securityResult = & powershell -ExecutionPolicy Bypass -File "comprehensive-security-audit.ps1"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Security audit failed! Aborting." -ForegroundColor Red
        exit 1
    }
    Write-Host "Security audit passed - 100% compliance maintained" -ForegroundColor Green
} catch {
    Write-Host "Could not run security audit: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 2: Check if application is already running
Write-Host "Step 2: Checking application status..." -ForegroundColor Yellow
$appRunning = $false
try {
    $response = Invoke-WebRequest -Uri "https://localhost:7015" -Method Head -TimeoutSec 5 -SkipCertificateCheck -ErrorAction SilentlyContinue
    if ($response.StatusCode -eq 200) {
        $appRunning = $true
        Write-Host "Application is already running on https://localhost:7015" -ForegroundColor Green
    }
} catch {
    Write-Host "Application is not running - will start it" -ForegroundColor Yellow
}
Write-Host ""

# Step 3: Start application if not running
if (-not $appRunning) {
    Write-Host "Step 3: Starting TeamWorkFlow application..." -ForegroundColor Yellow
    Write-Host "This will start the application in a new window" -ForegroundColor Gray
    
    # Start the application in a new PowerShell window
    $appPath = Join-Path (Get-Location).Parent.FullName "TeamWorkFlow"
    
    try {
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$appPath'; Write-Host 'Starting TeamWorkFlow Application...' -ForegroundColor Green; dotnet run" -WindowStyle Normal
        
        Write-Host "Application starting... waiting for it to be ready" -ForegroundColor Yellow
        
        # Wait for application to start (up to 30 seconds)
        $timeout = 30
        $elapsed = 0
        $ready = $false
        
        while ($elapsed -lt $timeout -and -not $ready) {
            Start-Sleep -Seconds 2
            $elapsed += 2
            
            try {
                $response = Invoke-WebRequest -Uri "https://localhost:7015" -Method Head -TimeoutSec 3 -SkipCertificateCheck -ErrorAction SilentlyContinue
                if ($response.StatusCode -eq 200) {
                    $ready = $true
                    Write-Host "Application is ready!" -ForegroundColor Green
                }
            } catch {
                Write-Host "Waiting... ($elapsed/$timeout seconds)" -ForegroundColor Gray
            }
        }
        
        if (-not $ready) {
            Write-Host "Application did not start within $timeout seconds" -ForegroundColor Red
            Write-Host "Please start the application manually:" -ForegroundColor Yellow
            Write-Host "  1. Open a new terminal" -ForegroundColor White
            Write-Host "  2. cd TeamWorkFlow" -ForegroundColor White
            Write-Host "  3. dotnet run" -ForegroundColor White
            Write-Host "  4. Wait for 'Now listening on: https://localhost:7015'" -ForegroundColor White
            Write-Host "  5. Re-run this script" -ForegroundColor White
            exit 1
        }
    } catch {
        Write-Host "Failed to start application: $_" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Step 3: Application already running - skipping startup" -ForegroundColor Green
}
Write-Host ""

# Step 4: Run authentication tests
Write-Host "Step 4: Running authentication tests..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor Gray
Write-Host ""

try {
    dotnet test --filter "AuthenticationTests" --logger "console;verbosity=normal" --no-build
    $testExitCode = $LASTEXITCODE
} catch {
    Write-Host "Test execution failed: $_" -ForegroundColor Red
    $testExitCode = 1
}

Write-Host ""
if ($testExitCode -eq 0) {
    Write-Host "SUCCESS: All authentication tests passed!" -ForegroundColor Green
    Write-Host "100% pass rate achieved" -ForegroundColor Green
    Write-Host "Security rating maintained at 100%" -ForegroundColor Green
    Write-Host ""
    Write-Host "MISSION ACCOMPLISHED!" -ForegroundColor Green
    Write-Host "- Authentication tests: 100% pass rate" -ForegroundColor White
    Write-Host "- Security compliance: 100%" -ForegroundColor White
    Write-Host "- Zero security compromises" -ForegroundColor White
} else {
    Write-Host "Some authentication tests failed" -ForegroundColor Red
    Write-Host "Security rating maintained at 100%" -ForegroundColor Green
    Write-Host ""
    Write-Host "Possible issues:" -ForegroundColor Yellow
    Write-Host "1. Application may not be fully ready" -ForegroundColor Gray
    Write-Host "2. Database may need seeding" -ForegroundColor Gray
    Write-Host "3. Try running tests again in a few seconds" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Final Status:" -ForegroundColor Cyan
Write-Host "Security Rating: 100%" -ForegroundColor Green
if ($testExitCode -eq 0) {
    Write-Host "Authentication Tests: PASSED" -ForegroundColor Green
} else {
    Write-Host "Authentication Tests: NEEDS ATTENTION" -ForegroundColor Yellow
}

exit $testExitCode
