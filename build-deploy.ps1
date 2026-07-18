<#
构建并打包安灯语音播报部署包
#>

$ErrorActionPreference = "Stop"

$projectDir = $PSScriptRoot
$deployDir = Join-Path $projectDir "deploy"
$zipPath = Join-Path $projectDir "AndonVoiceBroadcaster_Deploy.zip"

Write-Host "Build and package deployment..." -ForegroundColor Cyan

try {
    Write-Host "1. Build Release..." -ForegroundColor Yellow
    dotnet msbuild "$projectDir\AndonVoiceBroadcaster.csproj" -t:Rebuild -p:Configuration=Release -p:Platform=AnyCPU
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    Write-Host "Build successful" -ForegroundColor Green

    Write-Host "2. Create deploy directories..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Force -Path "$deployDir\audio", "$deployDir\config" | Out-Null
    Write-Host "Directories created" -ForegroundColor Green

    Write-Host "3. Validate and copy files..." -ForegroundColor Yellow
    $requiredSourceFiles = @(
        "$projectDir\bin\Release\AndonVoiceBroadcaster.exe",
        "$projectDir\config\appsettings.json",
        "$projectDir\audio\AlarmMusic.wav"
    )
    foreach ($requiredFile in $requiredSourceFiles) {
        if (-not (Test-Path -LiteralPath $requiredFile -PathType Leaf)) {
            throw "Required deployment file is missing: $requiredFile"
        }
    }

    $batchFiles = @(Get-ChildItem -LiteralPath $deployDir -Filter "*.bat" -File)
    $instructionFiles = @(Get-ChildItem -LiteralPath $deployDir -Filter "*.txt" -File)
    if ($batchFiles.Count -lt 2) {
        throw "At least two deployment batch files are required in: $deployDir"
    }
    if ($instructionFiles.Count -lt 1) {
        throw "A deployment instruction text file is required in: $deployDir"
    }

    try {
        Get-Content -Raw -Encoding UTF8 -LiteralPath "$projectDir\config\appsettings.json" | ConvertFrom-Json | Out-Null
    } catch {
        throw "config/appsettings.json is not valid JSON: $_"
    }

    Copy-Item -Path "$projectDir\bin\Release\AndonVoiceBroadcaster.exe" -Destination $deployDir -Force
    Copy-Item -Path "$projectDir\config\appsettings.json" -Destination "$deployDir\config" -Force
    Copy-Item -Path "$projectDir\audio\AlarmMusic.wav" -Destination "$deployDir\audio" -Force
    Write-Host "Deployment files validated and copied" -ForegroundColor Green

    Write-Host "4. Create zip package..." -ForegroundColor Yellow
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }
    Compress-Archive -Path "$deployDir\*" -DestinationPath $zipPath -Force
    $zipSize = (Get-Item $zipPath).Length / 1KB
    Write-Host "Zip created: $zipPath ($($zipSize.ToString("N2")) KB)" -ForegroundColor Green

    Write-Host "Deployment package built successfully!" -ForegroundColor Cyan

} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
