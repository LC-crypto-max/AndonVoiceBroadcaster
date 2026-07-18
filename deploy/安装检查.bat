@echo off
setlocal EnableExtensions
cd /d "%~dp0"
title Andon Voice Broadcaster - Environment Check

set /a FAILURES=0
set /a WARNINGS=0
set "REG_PATH=HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"

echo ==================================================
echo   Andon Voice Broadcaster - Environment Check
echo ==================================================
echo Folder: %CD%
echo.

echo [1/6] Operating system
ver
echo [INFO] Windows 10 or Windows 11 is recommended.
echo.

echo [2/6] .NET Framework 4.8
set "DOTNET_RELEASE="
for /f "tokens=3" %%R in ('reg query "%REG_PATH%" /v Release 2^>nul ^| findstr /i "Release"') do set /a DOTNET_RELEASE=%%R
if not defined DOTNET_RELEASE (
    echo [FAIL] .NET Framework 4.8 was not detected.
    set /a FAILURES+=1
) else if %DOTNET_RELEASE% LSS 528040 (
    echo [FAIL] .NET Framework Release %DOTNET_RELEASE% is lower than 528040.
    set /a FAILURES+=1
) else (
    echo [PASS] .NET Framework Release=%DOTNET_RELEASE%.
)
echo Download: https://dotnet.microsoft.com/download/dotnet-framework/net48
echo.

echo [3/6] Windows Chinese voice
powershell -NoProfile -ExecutionPolicy Bypass -Command "try { Add-Type -AssemblyName System.Speech; $voices = (New-Object System.Speech.Synthesis.SpeechSynthesizer).GetInstalledVoices() | Where-Object { $_.Enabled -and $_.VoiceInfo.Culture.Name -like 'zh-*' }; if ($voices.Count -gt 0) { $voices | ForEach-Object { Write-Host ('[PASS] Chinese voice: ' + $_.VoiceInfo.Name) }; exit 0 } else { exit 1 } } catch { exit 1 }"
if errorlevel 1 (
    echo [FAIL] No enabled Windows Chinese voice was detected.
    echo        Install Chinese speech in Windows language settings, then check again.
    set /a FAILURES+=1
)
echo.

echo [4/6] Windows Audio service
sc query AudioSrv | findstr /i "RUNNING" >nul
if errorlevel 1 (
    echo [WARN] Windows Audio is not running. Check the service and audio driver.
    set /a WARNINGS+=1
) else (
    echo [PASS] Windows Audio is running.
)
echo.

echo [5/6] Deployment files
for %%F in ("AndonVoiceBroadcaster.exe" "audio\AlarmMusic.wav" "config\appsettings.json") do (
    if exist "%%~F" (
        echo [PASS] %%~F
    ) else (
        echo [FAIL] Missing %%~F
        set /a FAILURES+=1
    )
)
echo.

echo [6/6] Default JSON configuration
if exist "config\appsettings.json" (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "try { Get-Content -Raw -Encoding UTF8 -LiteralPath 'config\appsettings.json' | ConvertFrom-Json | Out-Null; exit 0 } catch { exit 1 }"
    if errorlevel 1 (
        echo [FAIL] config\appsettings.json is not valid JSON.
        set /a FAILURES+=1
    ) else (
        echo [PASS] config\appsettings.json is valid JSON.
    )
) else (
    echo [SKIP] config\appsettings.json does not exist.
)
echo.

echo ==================================================
if %FAILURES% EQU 0 (
    echo RESULT: PASS. Warnings=%WARNINGS%.
    echo Connect the speaker and set it as the default output, then run the start script.
) else (
    echo RESULT: FAIL. Failures=%FAILURES%, Warnings=%WARNINGS%.
    echo Resolve all FAIL items before starting the application.
)
echo ==================================================
echo.
pause
exit /b %FAILURES%
