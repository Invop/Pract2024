@echo off
setlocal

:check_docker
:: Check if Docker is installed
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Docker is not installed.
    echo Opening Docker installation page...
    start https://docs.docker.com/desktop/install/windows-install/
    goto end
) else (
    echo Docker is installed.
    set DOCKER_INSTALLED=true
)

:: Check if Git is installed
git --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Git is not installed.
    echo Opening Git installation page...
    start https://git-scm.com/downloads
) else (
    echo Git is installed.
)

:: Check if .NET 8 is installed
dotnet --list-sdks | findstr "8." >nul 2>&1
if %errorlevel% neq 0 (
    echo .NET 8 is not installed.
    echo Opening .NET 8 installation page...
    start https://dotnet.microsoft.com/en-us/download
) else (
    echo .NET 8 is installed.
)

:: Execute dotnet dev-certs https --trust
echo Running 'dotnet dev-certs https --trust'...
dotnet dev-certs https --trust
if %errorlevel% neq 0 (
    echo User denied the certificate trust. Closing the application.
    goto end
)


:check_docker_running
:: Check if Docker Desktop is running
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo Docker Desktop is not running. Please start Docker Desktop and press Enter to continue...
    pause
    goto check_docker_running
)

:: Execute docker-compose up only if Docker is installed
if defined DOCKER_INSTALLED (
    :: Get the directory of the script
    set SCRIPT_DIR=%~dp0
    :: Convert UNC path to a local path if necessary
    pushd "%SCRIPT_DIR%" >nul 2>&1
    if %errorlevel% neq 0 (
        echo Unable to change to the script directory. It may be a UNC path.
        goto end
    )

    :: Run docker-compose up in a new instance of the console
    start cmd /k "cd /d %CD% && docker-compose up"
    popd
)

:: Run servers in separate console windows
start "Server" cmd /c "cd /d %CD%\ManekiApp\Server && dotnet run"
start "TelegramBot" cmd /c "cd /d %CD%\ManekiApp\TelegramBot && dotnet run"
start "TelegramPayBot" cmd /c "cd /d %CD%\ManekiApp\TelegramPayBot && dotnet run"

start https://localhost:5001/
:end
endlocal

