# PowerShell script to fix syntax errors introduced by the previous script
param(
    [string]$ProjectPath = "."
)

Write-Host "Fixing syntax errors in test files..." -ForegroundColor Green

# Get all test files
$testFiles = Get-ChildItem -Path $ProjectPath -Recurse -Include "*.cs" | Where-Object { 
    $_.Directory.Name -eq "UnitTests" -or $_.Directory.Name -eq "Controllers" 
}

foreach ($file in $testFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Fix double decimal points like "10.00.0" -> "10.0"
    $content = $content -replace "(\d+\.\d+)\.0", "`$1"
    
    # Fix cases where .0 was added to already decimal values
    $content = $content -replace "(\d+\.\d{2})\.0", "`$1"
    
    # Fix cases where .0.0 was created
    $content = $content -replace "\.0\.0", ".0"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Fixed: $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  No fixes needed: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host "Syntax error fixes complete!" -ForegroundColor Green
