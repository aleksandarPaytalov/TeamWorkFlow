# PowerShell script to fix incorrect changes to SprintSummaryModel
param(
    [string]$ProjectPath = "."
)

Write-Host "Fixing SprintSummaryModel property names..." -ForegroundColor Green

# Get Sprint test files
$sprintFiles = Get-ChildItem -Path $ProjectPath -Recurse -Include "*Sprint*.cs" | Where-Object { 
    $_.Directory.Name -eq "UnitTests"
}

foreach ($file in $sprintFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Revert TotalPlannedHours back to TotalEstimatedHours for SprintSummaryModel
    $content = $content -replace "TotalPlannedHours", "TotalEstimatedHours"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Fixed: $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  No fixes needed: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host "SprintSummaryModel fixes complete!" -ForegroundColor Green
