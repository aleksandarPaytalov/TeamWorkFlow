# TeamWorkFlow Playwright Installation Script
# This script installs Playwright browsers and dependencies

Write-Host "🎭 TeamWorkFlow Playwright Installation" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET SDK not found! Please install .NET 6.0 SDK first." -ForegroundColor Red
    exit 1
}

# Build the Playwright test project
Write-Host "🔨 Building Playwright test project..." -ForegroundColor Yellow
try {
    dotnet build TeamWorkFlow.PlaywrightTests/TeamWorkFlow.PlaywrightTests.csproj
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to build Playwright test project!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "✅ Playwright test project built successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error building Playwright test project: $_" -ForegroundColor Red
    exit 1
}

# Install Playwright CLI tool globally
Write-Host "🛠️ Installing Playwright CLI tool..." -ForegroundColor Yellow
try {
    dotnet tool install --global Microsoft.Playwright.CLI
    Write-Host "✅ Playwright CLI tool installed!" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Playwright CLI tool may already be installed or failed to install" -ForegroundColor Yellow
}

# Install Playwright browsers
Write-Host "🌐 Installing Playwright browsers..." -ForegroundColor Yellow
try {
    Set-Location TeamWorkFlow.PlaywrightTests
    playwright install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to install Playwright browsers!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "✅ Playwright browsers installed successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error installing Playwright browsers: $_" -ForegroundColor Red
    exit 1
} finally {
    Set-Location ..
}

# Install system dependencies (Linux/macOS)
if ($IsLinux -or $IsMacOS) {
    Write-Host "🔧 Installing system dependencies..." -ForegroundColor Yellow
    try {
        Set-Location TeamWorkFlow.PlaywrightTests
        playwright install-deps
        Write-Host "✅ System dependencies installed!" -ForegroundColor Green
    } catch {
        Write-Host "⚠️ Warning: Could not install system dependencies: $_" -ForegroundColor Yellow
    } finally {
        Set-Location ..
    }
}

# Verify installation
Write-Host "🔍 Verifying Playwright installation..." -ForegroundColor Yellow
try {
    Set-Location TeamWorkFlow.PlaywrightTests
    $playwrightVersion = playwright --version
    Write-Host "✅ Playwright version: $playwrightVersion" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Warning: Could not verify Playwright version" -ForegroundColor Yellow
} finally {
    Set-Location ..
}

Write-Host "`n🎉 Playwright installation completed!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

Write-Host "`n📋 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Configure test settings in TeamWorkFlow.PlaywrightTests/appsettings.json" -ForegroundColor White
Write-Host "2. Start your TeamWorkFlow application (dotnet run --project TeamWorkFlow/)" -ForegroundColor White
Write-Host "3. Run Playwright tests:" -ForegroundColor White
Write-Host "   • All tests: dotnet test TeamWorkFlow.PlaywrightTests/" -ForegroundColor Gray
Write-Host "   • Specific test: dotnet test TeamWorkFlow.PlaywrightTests/ --filter 'TestName'" -ForegroundColor Gray
Write-Host "   • With script: ./TeamWorkFlow.PlaywrightTests/run-playwright-tests.ps1" -ForegroundColor Gray

Write-Host "`n📚 Documentation:" -ForegroundColor Cyan
Write-Host "• Playwright Tests: TeamWorkFlow.PlaywrightTests/README.md" -ForegroundColor White
Write-Host "• Main Project: README.md" -ForegroundColor White

Write-Host "`n✨ Happy Testing! 🧪" -ForegroundColor Green
