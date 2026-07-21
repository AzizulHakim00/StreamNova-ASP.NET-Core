@echo off
setlocal
cd /d "%~dp0"

echo Starting StreamNova...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo.
    echo .NET SDK was not found.
    echo Install .NET 10 SDK, then run this file again.
    pause
    exit /b 1
)

dotnet restore
if errorlevel 1 goto :failed

dotnet run
exit /b 0

:failed
echo.
echo StreamNova could not start. Review the error above.
pause
exit /b 1
