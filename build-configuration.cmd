@echo off

%FAKE% %NYX% "target=clean" -st
%FAKE% %NYX% "target=RestoreNugetPackages" -st

IF NOT [%1]==[] (set RELEASE_NUGETKEY="%1")
IF NOT [%2]==[] (set RELEASE_TARGETSOURCE="%2")

SET SUMMARY="PushNotifications"
SET DESCRIPTION="PushNotifications"

%FAKE% %NYX% appName=PushNotifications.WS           appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% target=Build appReleaseNotes=src/PushNotifications.WS.MSI/PushNotifications.WS.MSI.rn.md
REM %FAKE% %NYX% appName=PushNotifications.WS.MSI       appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% appType=msi
%FAKE% %NYX% appName=PushNotifications.Api          appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY%
%FAKE% %NYX% appName=PushNotifications.Api.Client   appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetserver=%NUGET_SOURCE_DEV_PUSH% nugetkey=%RELEASE_NUGETKEY% nugetPackageName=PushNotifications.Api.Client
