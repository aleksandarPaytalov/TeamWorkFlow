# PowerShell script to update test files with new property names
param(
    [string]$ProjectPath = "."
)

Write-Host "Updating test files with new property names..." -ForegroundColor Green

# Define the replacements
$replacements = @{
    "CalculatedTotalHours" = "CalculatedActualHours"
    "TotalEstimatedHours" = "TotalPlannedHours"
    "FormattedCalculatedTotalHours" = "FormattedCalculatedActualHours"
    "TimeVariance" = "TimeOverview"
}

# Get all test files
$testFiles = Get-ChildItem -Path $ProjectPath -Recurse -Include "*.cs" | Where-Object { 
    $_.Directory.Name -eq "UnitTests" -or $_.Directory.Name -eq "Controllers" 
}

foreach ($file in $testFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow
    
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    foreach ($oldProperty in $replacements.Keys) {
        $newProperty = $replacements[$oldProperty]
        
        # Replace property assignments
        $content = $content -replace "(\s+)$oldProperty(\s*=\s*)(\d+)", "`$1$newProperty`$2`$3.0"
        
        # Replace property references
        $content = $content -replace "(\.)$oldProperty(\s*[,\);])", "`$1$newProperty`$2"
        $content = $content -replace "(\.)$oldProperty(\s*,)", "`$1$newProperty`$2"
        
        # Replace in assertions
        $content = $content -replace "(Assert\.That\([^,]+\.)$oldProperty", "`$1$newProperty"
        
        # Replace in model initializations
        $content = $content -replace "(\s+)$oldProperty(\s*=)", "`$1$newProperty`$2"
    }
    
    # Special case for double values
    $content = $content -replace "CalculatedActualHours(\s*=\s*)(\d+)([^.])", "CalculatedActualHours`$1`$2.0`$3"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  No changes: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host "Update complete!" -ForegroundColor Green
