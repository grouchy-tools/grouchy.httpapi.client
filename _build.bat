@echo off

powershell "Import-Module %~dp0\build\psake.psm1; Invoke-Psake %~dp0\build\build.ps1; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }"

pause