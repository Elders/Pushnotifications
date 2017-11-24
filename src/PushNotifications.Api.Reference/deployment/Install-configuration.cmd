@echo off

SET WEBSITE_SOURCE=%~dp0..\content

powershell  -executionPolicy bypass -Command "& '.\.nyx\setup-website-webapp.ps1' -Tenant %1 -Website %2 -WebsiteSource '%WEBSITE_SOURCE%' -Company 'Elders' -App 'push-notifications' -AppHost 'reference'"
