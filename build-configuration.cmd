@echo off

%FAKE% %NYX% "target=clean" -st
%FAKE% %NYX% "target=RestoreNugetPackages" -st

IF NOT [%1]==[] (set RELEASE_NUGETKEY="%1")
IF NOT [%2]==[] (set RELEASE_TARGETSOURCE="%2")

SET SUMMARY="PushNotifications"
SET DESCRIPTION="PushNotifications"

%FAKE% %NYX% appName=PushNotifications.WS               appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% nugetPackageName=push-notifications.ws  target=Build appReleaseNotes=src/PushNotifications.WS.MSI/PushNotifications.WS.MSI.rn.md
IF errorlevel 1 (echo Faild with exit code %errorlevel% & exit /b %errorlevel%)
%FAKE% %NYX% appName=PushNotifications.WS.MSI           appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% nugetPackageName=push-notifications.ws.msi appType=msi
IF errorlevel 1 (echo Faild with exit code %errorlevel% & exit /b %errorlevel%)
%FAKE% %NYX% appName=PushNotifications.Api.Host         appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% nugetPackageName=push-notifications.api.host
IF errorlevel 1 (echo Faild with exit code %errorlevel% & exit /b %errorlevel%)
%FAKE% %NYX% appName=PushNotifications.Api.Reference    appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% nugetPackageName=push-notifications.api.reference
IF errorlevel 1 (echo Faild with exit code %errorlevel% & exit /b %errorlevel%)
%FAKE% %NYX% appName=PushNotifications.Api.Client       appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% nugetPackageName=push-notifications.api.client
IF errorlevel 1 (echo Faild with exit code %errorlevel% & exit /b %errorlevel%)
