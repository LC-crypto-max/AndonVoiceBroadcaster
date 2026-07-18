@echo off
setlocal EnableExtensions
cd /d "%~dp0"
title Andon Voice Broadcaster

set "REG_PATH=HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"
set "EXE_NAME=AndonVoiceBroadcaster.exe"

echo ================================================
echo       Andon Voice Broadcaster - Launcher
echo ================================================
echo.

set "DOTNET_RELEASE="
for /f "tokens=3" %%R in ('reg query "%REG_PATH%" /v Release 2^>nul ^| findstr /i "Release"') do set /a DOTNET_RELEASE=%%R
if not defined DOTNET_RELEASE (
    echo [ERROR] .NET Framework 4.8 was not detected.
    echo Download: https://dotnet.microsoft.com/download/dotnet-framework/net48
    pause
    exit /b 1
)

if %DOTNET_RELEASE% LSS 528040 (
    echo [ERROR] .NET Framework Release %DOTNET_RELEASE% is lower than 528040.
    echo Download: https://dotnet.microsoft.com/download/dotnet-framework/net48
    pause
    exit /b 1
)

if not exist "%EXE_NAME%" (
    echo [ERROR] Missing %EXE_NAME%.
    pause
    exit /b 1
)

if not exist "config\appsettings.json" (
    echo [ERROR] Missing config\appsettings.json.
    pause
    exit /b 1
)

if not exist "audio\AlarmMusic.wav" (
    echo [ERROR] Missing audio\AlarmMusic.wav.
    pause
    exit /b 1
)

echo [PASS] Runtime and deployment files are ready.
echo Starting %EXE_NAME%...
start "" "%EXE_NAME%"

echo The application has started. Check the Windows system tray.
timeout /t 3 /nobreak >nul
exit /b 0
