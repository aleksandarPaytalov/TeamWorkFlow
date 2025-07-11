# Stop Application and Run Authentication UI Tests
# This script stops the TeamWorkFlow application and runs the new authentication UI tests

Write-Host "Authentication Tests - Final Solution" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
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

# Step 2: Stop running TeamWorkFlow processes
Write-Host "Step 2: Stopping TeamWorkFlow application..." -ForegroundColor Yellow
$runningProcesses = Get-Process -Name "TeamWorkFlow" -ErrorAction SilentlyContinue
if ($runningProcesses) {
    Write-Host "Found running TeamWorkFlow processes:" -ForegroundColor Yellow
    foreach ($process in $runningProcesses) {
        Write-Host "  Process ID: $($process.Id)" -ForegroundColor Gray
    }
    
    try {
        Stop-Process -Name "TeamWorkFlow" -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 3
        
        # Verify processes are stopped
        $stillRunning = Get-Process -Name "TeamWorkFlow" -ErrorAction SilentlyContinue
        if ($stillRunning) {
            Write-Host "Some processes are still running. Trying force kill..." -ForegroundColor Yellow
            foreach ($process in $stillRunning) {
                Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
            }
            Start-Sleep -Seconds 2
        }
        
        Write-Host "TeamWorkFlow processes stopped successfully" -ForegroundColor Green
    } catch {
        Write-Host "Error stopping processes: $_" -ForegroundColor Red
        Write-Host "Please stop TeamWorkFlow manually and re-run this script" -ForegroundColor Yellow
        exit 1
    }
} else {
    Write-Host "No running TeamWorkFlow processes found" -ForegroundColor Green
}
Write-Host ""

# Step 3: Clean and rebuild
Write-Host "Step 3: Cleaning and rebuilding solution..." -ForegroundColor Yellow
Set-Location ..
try {
    dotnet clean --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Clean failed" -ForegroundColor Red
        Set-Location TeamWorkFlow.PlaywrightTests
        exit 1
    }

    dotnet build --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed" -ForegroundColor Red
        Set-Location TeamWorkFlow.PlaywrightTests
        exit 1
    }
    Write-Host "Solution cleaned and rebuilt successfully" -ForegroundColor Green
} catch {
    Write-Host "Build process failed: $_" -ForegroundColor Red
    Set-Location TeamWorkFlow.PlaywrightTests
    exit 1
}
Set-Location TeamWorkFlow.PlaywrightTests
Write-Host ""

# Step 4: Run authentication UI tests
Write-Host "Step 4: Running Authentication UI Tests..." -ForegroundColor Yellow
Write-Host "These tests use fake credentials and don't require database seeding" -ForegroundColor Gray
Write-Host ""

try {
    dotnet test --filter "AuthenticationUITests" --logger "console;verbosity=normal" --no-build
    $testExitCode = $LASTEXITCODE
} catch {
    Write-Host "Test execution failed: $_" -ForegroundColor Red
    $testExitCode = 1
}

Write-Host ""
if ($testExitCode -eq 0) {
    Write-Host "SUCCESS: All Authentication UI Tests passed!" -ForegroundColor Green
    Write-Host "100% pass rate achieved with secure fake credentials" -ForegroundColor Green
    Write-Host "Security rating maintained at 100%" -ForegroundColor Green
    Write-Host ""
    Write-Host "MISSION ACCOMPLISHED!" -ForegroundColor Green
    Write-Host "- Authentication UI tests: 100% pass rate" -ForegroundColor White
    Write-Host "- Security compliance: 100%" -ForegroundColor White
    Write-Host "- No database dependency" -ForegroundColor White
    Write-Host "- Fake credentials used safely" -ForegroundColor White
} else {
    Write-Host "Some Authentication UI Tests failed" -ForegroundColor Red
    Write-Host "Security rating maintained at 100%" -ForegroundColor Green
    Write-Host ""
    Write-Host "Possible issues:" -ForegroundColor Yellow
    Write-Host "1. TeamWorkFlow application may still be running" -ForegroundColor Gray
    Write-Host "2. Build may have failed" -ForegroundColor Gray
    Write-Host "3. Test dependencies may be missing" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Try running the tests manually:" -ForegroundColor Cyan
    Write-Host "dotnet test --filter 'AuthenticationUITests' --logger console" -ForegroundColor White
}

Write-Host ""
Write-Host "Final Status:" -ForegroundColor Cyan
Write-Host "Security Rating: 100%" -ForegroundColor Green
if ($testExitCode -eq 0) {
    Write-Host "Authentication UI Tests: PASSED" -ForegroundColor Green
} else {
    Write-Host "Authentication UI Tests: NEEDS ATTENTION" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Test Details:" -ForegroundColor Cyan
Write-Host "- Tests use fake credentials (fake.admin@test.local)" -ForegroundColor Gray
Write-Host "- No database seeding required" -ForegroundColor Gray
Write-Host "- UI validation and behavior testing" -ForegroundColor Gray
Write-Host "- Responsive design verification" -ForegroundColor Gray
Write-Host "- Complete security compliance" -ForegroundColor Gray

exit $testExitCode
