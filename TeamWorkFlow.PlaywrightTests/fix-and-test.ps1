# Fix and Test Script
# This script helps resolve the current issues

Write-Host "🔧 TeamWorkFlow Fix and Test Script" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan

Write-Host "`n🔍 Current Issues Detected:" -ForegroundColor Yellow
Write-Host "1. ❌ Compilation error: duplicate 'currentUrl' variable (FIXED)" -ForegroundColor Red
Write-Host "2. ❌ Application still running on port 7015 (process 28724)" -ForegroundColor Red
Write-Host "3. ❌ File locks preventing rebuild" -ForegroundColor Red
Write-Host "4. ❌ Authentication test using old compiled version" -ForegroundColor Red

Write-Host "`n✅ What We Fixed:" -ForegroundColor Green
Write-Host "• Fixed duplicate variable name in AuthenticationTests.cs" -ForegroundColor White
Write-Host "• Updated login detection logic to match working tests" -ForegroundColor White
Write-Host "• Fixed user greeting selector" -ForegroundColor White

Write-Host "`n🎯 IMMEDIATE ACTION REQUIRED:" -ForegroundColor Cyan
Write-Host "You need to stop the running TeamWorkFlow application!" -ForegroundColor Yellow

Write-Host "`n📋 Step-by-Step Instructions:" -ForegroundColor Cyan
Write-Host "1. Find the terminal where TeamWorkFlow is running" -ForegroundColor White
Write-Host "2. Press Ctrl+C to stop the application" -ForegroundColor White
Write-Host "3. Run this command to build:" -ForegroundColor White
Write-Host "   dotnet build" -ForegroundColor Gray
Write-Host "4. Start the application again:" -ForegroundColor White
Write-Host "   cd ../TeamWorkFlow" -ForegroundColor Gray
Write-Host "   dotnet run" -ForegroundColor Gray
Write-Host "5. Test the fixed authentication:" -ForegroundColor White
Write-Host "   cd ../TeamWorkFlow.PlaywrightTests" -ForegroundColor Gray
Write-Host "   dotnet test --no-build --filter `"Name=Login_WithValidAdminCredentials_ShouldSucceed`"" -ForegroundColor Gray

Write-Host "`n🔍 How to Find the Running Application:" -ForegroundColor Cyan
Write-Host "• Look for a terminal with output like 'Now listening on: https://localhost:7015'" -ForegroundColor White
Write-Host "• Or check Task Manager for 'TeamWorkFlow' process (PID 28724)" -ForegroundColor White
Write-Host "• The terminal might show database queries or HTTP requests" -ForegroundColor White

Write-Host "`n⚡ Quick Test (Current Working Tests):" -ForegroundColor Green
Write-Host "While you're fixing the app, these tests work with current compiled version:" -ForegroundColor White
Write-Host "dotnet test --no-build --filter `"Name=QuickLogin_ShouldWork`"" -ForegroundColor Gray
Write-Host "dotnet test --no-build --filter `"Name=BasicLogin_ShouldWork`"" -ForegroundColor Gray
Write-Host "dotnet test --no-build --filter `"Name=Application_ShouldBeAccessible`"" -ForegroundColor Gray

# Check if app is running
Write-Host "`n🔍 Checking Application Status..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "https://localhost:7015" -UseBasicParsing -TimeoutSec 2 -SkipCertificateCheck
    Write-Host "🟢 Application is RUNNING at https://localhost:7015" -ForegroundColor Green
    Write-Host "   ⚠️  You MUST stop it before rebuilding!" -ForegroundColor Yellow
} catch {
    Write-Host "🔴 Application is NOT RUNNING" -ForegroundColor Red
    Write-Host "   ✅ You can build now: dotnet build" -ForegroundColor Green
}

Write-Host "`n💡 Pro Tip:" -ForegroundColor Cyan
Write-Host "After stopping the app, you can use 'dotnet build --no-restore' for faster builds!" -ForegroundColor White

Write-Host "`n📊 Expected Results After Fix:" -ForegroundColor Cyan
Write-Host "✅ Build should succeed without errors" -ForegroundColor Green
Write-Host "✅ Application should start on port 7015" -ForegroundColor Green
Write-Host "✅ Login_WithValidAdminCredentials_ShouldSucceed should PASS" -ForegroundColor Green
Write-Host "✅ All authentication tests should work" -ForegroundColor Green
