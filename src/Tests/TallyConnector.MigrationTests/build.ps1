$ErrorActionPreference = "Continue"
$output = dotnet build 2>&1 | Out-String
Write-Host $output
$output | Out-File -FilePath "build_result.txt"
