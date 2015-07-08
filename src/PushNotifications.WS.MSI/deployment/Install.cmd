@echo off

SETLOCAL

SET TOOLS_PATH=.
SET NUGET=%TOOLS_PATH%\NuGet\NuGet.exe
SET FAKE=%TOOLS_PATH%\FAKE\tools\Fake.exe

echo Downloading latest version of NuGet.exe...
IF NOT EXIST %TOOLS_PATH%\NuGet md %TOOLS_PATH%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%NUGET%'"

echo Downloading latest version of Fake.exe...
%NUGET% "install" "FAKE" "-OutputDirectory" "%TOOLS_PATH%" "-ExcludeVersion" "-Prerelease"

for /f %%i in ("%0") do set curpath=%%~dpi 
cd /d %curpath%
cd .\..\Content

".\..\tools\%FAKE%" ".\..\tools\Install.fsx"