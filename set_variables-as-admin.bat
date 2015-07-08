echo off
@setlocal enableextensions
cd /d "%~dp0"
CALL build.cmd ReleaseLocal
@cd /d "%~dp0"\bin\nuget\PushNotifications.Configuration\tools

set env=%CLUSTER_NAME%
set host=%COMPUTERNAME%
echo MachineName %host%
setlocal enabledelayedexpansion

CALL Install.cmd
@cd /d "%~dp0"\bin\nuget\PushNotifications.Configuration\content
CALL :setVaraiabless PushNotifications.json PushNotifications

echo

goto :eof
:setVaraiabless
SETLOCAL
set file=%1
set app=%2
echo Calling... Elders.Pandora.exe open -a %app% -c %env% -m %host% -j %file%
.\..\tools\Pandora.Cli\tools\Elders.Pandora.Cli.exe open -a %app% -c %env% -m %host% -j %file%
ENDLOCAL
PAUSE