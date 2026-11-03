$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

dotnet test tests/Shop.Tests/Shop.Tests.csproj `
    --configuration Release `
    --collect:"XPlat Code Coverage" `
    --settings coverlet.runsettings `
    --results-directory "$root/coverage"

Write-Host "Cobertura: coverage/**/coverage.cobertura.xml"
Write-Host "For an HTML report install ReportGenerator: dotnet tool install -g dotnet-reportgenerator-globaltool"
