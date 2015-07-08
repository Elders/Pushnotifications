@echo off

SETLOCAL

for /f "delims=\" %%a in ("%cd%") do SET APP_NAME=%%~nxa

echo Application name: %APP_NAME%

SET OUTPUT=..\..\bin\release\%APP_NAME%\_publishedConfiguration\%APP_NAME%
SET SOURCE=public

xcopy %SOURCE% %OUTPUT% /D /Y /I /s