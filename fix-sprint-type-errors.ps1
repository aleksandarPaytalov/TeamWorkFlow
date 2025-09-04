# PowerShell script to fix type conversion errors in SprintModelsUnitTests
param(
    [string]$ProjectPath = "."
)

Write-Host "Fixing type conversion errors in SprintModelsUnitTests..." -ForegroundColor Green

$sprintTestFile = Join-Path $ProjectPath "UnitTests\SprintModelsUnitTests.cs"

if (Test-Path $sprintTestFile) {
    $content = Get-Content $sprintTestFile -Raw
    $originalContent = $content
    
    # Fix TotalEstimatedHours assignments from double to int
    $content = $content -replace "TotalEstimatedHours = (\d+)\.0", "TotalEstimatedHours = `$1"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $sprintTestFile -Value $content -NoNewline
        Write-Host "  Fixed: SprintModelsUnitTests.cs" -ForegroundColor Green
    } else {
        Write-Host "  No fixes needed: SprintModelsUnitTests.cs" -ForegroundColor Gray
    }
} else {
    Write-Host "  File not found: SprintModelsUnitTests.cs" -ForegroundColor Red
}

Write-Host "Type conversion fixes complete!" -ForegroundColor Green
