@echo off

powershell "Import-Module %~dp0\build\psake.psm1 ; Invoke-Psake %~dp0\build\build.ps1 ; exit $LASTEXITCODE;"

pause