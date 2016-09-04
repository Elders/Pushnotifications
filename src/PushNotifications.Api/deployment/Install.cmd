@echo off

SETLOCAL

SET NUGET=%LocalAppData%\NuGet\NuGet.exe
SET FAKE=bin\FAKE\tools\Fake.exe

echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile '%NUGET%'"

echo Downloading latest version of FAKE...
IF NOT EXIST bin\FAKE %NUGET% "install" "FAKE" "-OutputDirectory" "bin" "-ExcludeVersion" "-Version" "4.32.0"

for /f %%i in ("%0") do set curpath=%%~dpi 
cd /d %curpath%

IF NOT [%1]==[] (%FAKE% "Install.fsx" website="%1") ELSE %FAKE% "Install.fsx"