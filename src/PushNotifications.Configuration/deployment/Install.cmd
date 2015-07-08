::@echo off

SETLOCAL

SET TOOLS_PATH=.
SET NUGET=%TOOLS_PATH%\NuGet\NuGet.exe

echo Downloading latest version of NuGet.exe...
IF NOT EXIST %TOOLS_PATH%\NuGet md %TOOLS_PATH%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%NUGET%'"

echo Downloading latest version of Pandora.Cli.exe...
%NUGET% "install" "Pandora.Cli" "-OutputDirectory" "%TOOLS_PATH%" "-ExcludeVersion" "-Prerelease"