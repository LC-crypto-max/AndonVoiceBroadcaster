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

    Write-Host "3. Copy files..." -ForegroundColor Yellow
    Copy-Item -Path "$projectDir\bin\Release\AndonVoiceBroadcaster.exe" -Destination $deployDir -Force
    Copy-Item -Path "$projectDir\config\appsettings.json" -Destination "$deployDir\config" -Force
    Copy-Item -Path "$projectDir\audio\AlarmMusic.wav" -Destination "$deployDir\audio" -Force
    Write-Host "Files copied" -ForegroundColor Green

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
