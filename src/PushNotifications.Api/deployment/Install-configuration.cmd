@echo off

SET WEBSITE_SOURCE=%~dp0..\content

powershell  -executionPolicy bypass -Command "& '.\.nyx\setup-website.ps1' -Tenant %1 -Website %2 -WebsiteSource '%WEBSITE_SOURCE%' -Company 'Elders' -App 'PushNotifications' -AppHost 'api' -CertLocation 'C:\STAR.onebigsplash.com.pfx' -CertThumbprint '42f5de01be23db7a84e005f0a723419e138b0869' -CertPassword 'marketvision1'"
