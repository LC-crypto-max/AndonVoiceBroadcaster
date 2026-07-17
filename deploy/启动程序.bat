@echo off
chcp 65001 >nul
title 安灯语音播报启动器

echo ================================================
echo      安灯语音播报 - 启动器
echo ================================================
echo.

set "DOTNET_VERSION=4.8"
set "REG_PATH=HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"
set "EXE_NAME=AndonVoiceBroadcaster.exe"

reg query "%REG_PATH%" /v Release >nul 2>&1
if %errorlevel% neq 0 (
    echo 错误：未检测到 .NET Framework %DOTNET_VERSION%
    echo.
    echo 请先安装 .NET Framework %DOTNET_VERSION% 后再运行本程序
    echo 下载地址：https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/net48
    echo.
    pause
    exit /b 1
)

set "DOTNET_RELEASE="
for /f "tokens=3" %%R in ('reg query "%REG_PATH%" /v Release 2^>nul ^| findstr /i "Release"') do set /a DOTNET_RELEASE=%%R

if not defined DOTNET_RELEASE (
    echo 错误：无法读取 .NET Framework 版本信息
    pause
    exit /b 1
)

if %DOTNET_RELEASE% LSS 528040 (
    echo 警告：检测到的 .NET Framework 版本低于 %DOTNET_VERSION%
    echo.
    echo 建议安装 .NET Framework %DOTNET_VERSION% 以确保程序正常运行
    echo 下载地址：https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/net48
    echo.
    echo 是否继续启动程序？(Y/N)
    set "choice="
    set /p "choice="
    if /i not "%choice%"=="Y" (
        echo 已取消启动
        pause
        exit /b 0
    )
)

echo 检测到 .NET Framework %DOTNET_VERSION%，正在启动程序...
echo.

if not exist "%EXE_NAME%" (
    echo 错误：未找到 %EXE_NAME%
    pause
    exit /b 1
)

start "" "%EXE_NAME%"

echo 程序已启动，窗口将最小化到系统托盘
echo.
echo 提示：
echo - 双击系统托盘图标可打开主窗口
echo - 右键点击系统托盘图标可退出程序
echo.
pause
