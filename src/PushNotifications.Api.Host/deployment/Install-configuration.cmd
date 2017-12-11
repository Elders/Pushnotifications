@echo off

SET WEBSITE_SOURCE=%~dp0..\content

set certificateName=
set thumbprint=
set password=

call :getVaraiabless certificateName push-notifications.api cert_name
call :getVaraiabless thumbprint push-notifications.api cert_thumbprint
call :getVaraiabless password push-notifications.api cert_password

set location=%ALLUSERSPROFILE%\Elders\push-notifications\shared\cert\%certificateName%
echo %location%

powershell -executionPolicy bypass -Command "& '.\.nyx\setup-website.ps1' -Tenant %1 -Website %2 -WebsiteSource '%WEBSITE_SOURCE%' -Company 'Elders' -App 'push-notifications' -AppHost 'api' -CertLocation '%location%' -CertThumbprint '%thumbprint%' -CertPassword '%password%'"

goto :eof
:getVaraiabless
SETLOCAL
set app=%2
set key=%3

echo Calling... Elders.Pandora.Cli.exe get -a %app% -c %cluster_name% -m %computername% -o consul -h http://consul.local.com:8500/ -k %key%
set result=
set command=%ROOT%\Pandora.Cli\tools\Elders.Pandora.Cli.exe get -a %app% -c %cluster_name% -m %computername% -o consul -h http://consul.local.com:8500/ -k %key%
for /f "delims=" %%i in ('%command%') do SET "result=%%i"
(ENDLOCAL & REM -- RETURN VALUES
    SET %~1=%result%
)