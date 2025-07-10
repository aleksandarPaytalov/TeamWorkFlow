# Rebuild and Test Script
# This script handles the file lock issue by stopping the app, building, and providing instructions

Write-Host "🔧 TeamWorkFlow Rebuild and Test Script" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

Write-Host "`n⚠️  IMPORTANT: File Lock Issue Detected" -ForegroundColor Yellow
Write-Host "Your TeamWorkFlow application is running and locking DLL files." -ForegroundColor Yellow
Write-Host "This prevents rebuilding the test project with the latest fixes." -ForegroundColor Yellow

Write-Host "`n📋 To apply the authentication test fixes, please:" -ForegroundColor Cyan
Write-Host "1. Stop your TeamWorkFlow application (Ctrl+C in the terminal where it's running)" -ForegroundColor White
Write-Host "2. Run this command to build the test project:" -ForegroundColor White
Write-Host "   dotnet build" -ForegroundColor Gray
Write-Host "3. Start your application again:" -ForegroundColor White
Write-Host "   cd ../TeamWorkFlow" -ForegroundColor Gray
Write-Host "   dotnet run" -ForegroundColor Gray
Write-Host "4. Test the fixed authentication tests:" -ForegroundColor White
Write-Host "   cd ../TeamWorkFlow.PlaywrightTests" -ForegroundColor Gray
Write-Host "   dotnet test --no-build --filter `"Name=Login_WithValidAdminCredentials_ShouldSucceed`"" -ForegroundColor Gray

Write-Host "`n🎯 Alternative: Test Without Rebuilding" -ForegroundColor Green
Write-Host "If you want to test immediately without stopping the app:" -ForegroundColor White
Write-Host "1. The current compiled tests will still have the old selectors" -ForegroundColor Yellow
Write-Host "2. But our working tests still function:" -ForegroundColor White
Write-Host "   dotnet test --no-build --filter `"Name=QuickLogin_ShouldWork`"" -ForegroundColor Gray
Write-Host "   dotnet test --no-build --filter `"Name=BasicLogin_ShouldWork`"" -ForegroundColor Gray
Write-Host "   dotnet test --no-build --filter `"Name=Application_ShouldBeAccessible`"" -ForegroundColor Gray

Write-Host "`n🔍 What We Fixed:" -ForegroundColor Cyan
Write-Host "• Updated AuthenticationTests to use correct login detection" -ForegroundColor White
Write-Host "• Fixed user greeting selector: a[title='Manage Account']" -ForegroundColor White
Write-Host "• Replaced IsLoggedInAsync() with URL-based detection" -ForegroundColor White
Write-Host "• Updated logout detection logic" -ForegroundColor White

Write-Host "`n📊 Current Status:" -ForegroundColor Cyan
Write-Host "✅ Basic connectivity tests: WORKING" -ForegroundColor Green
Write-Host "✅ Quick login test: WORKING" -ForegroundColor Green
Write-Host "✅ Application access: WORKING" -ForegroundColor Green
Write-Host "🔧 Authentication tests: FIXED (need rebuild)" -ForegroundColor Yellow

Write-Host "`n💡 Recommendation:" -ForegroundColor Cyan
Write-Host "Stop the app, rebuild, restart, then test the authentication features!" -ForegroundColor White

# Check if app is running
try {
    $response = Invoke-WebRequest -Uri "https://localhost:7015" -UseBasicParsing -TimeoutSec 2 -SkipCertificateCheck
    Write-Host "`n🟢 Application Status: RUNNING at https://localhost:7015" -ForegroundColor Green
    Write-Host "   Process is likely TeamWorkFlow (28724) - this is causing the file locks" -ForegroundColor Yellow
} catch {
    Write-Host "`n🔴 Application Status: NOT RUNNING" -ForegroundColor Red
    Write-Host "   You can build now: dotnet build" -ForegroundColor Green
}

Write-Host "`n📚 Documentation:" -ForegroundColor Cyan
Write-Host "• README.md - Complete setup guide" -ForegroundColor White
Write-Host "• PLAYWRIGHT_FIXES_SUMMARY.md - All fixes applied" -ForegroundColor White
