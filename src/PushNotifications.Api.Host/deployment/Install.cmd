@echo off
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "New-Item -ItemType directory -Path .nyx\ -Force; (New-Object System.Net.WebClient).DownloadFile('https://raw.githubusercontent.com/Elders/Nyx/master/.nyx/nyx.cmd','.nyx\nyx.cmd')"
call .nyx\nyx.cmd

call .nyx\get-pandora.cli.cmd
call Install-configuration.cmd %1 %2
