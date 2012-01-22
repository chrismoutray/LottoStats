@echo off

set msbuildpath=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

call :cleansolution
pause
call :lblbuildclientANYCPU

IF %ERRORLEVEL% EQU 0 pause
goto :EOF

:cleansolution
echo.
%msbuildpath% /t:Clean /v:q LottoStats.sln
del /Q /S .\bin
goto :EOF

:lblbuildclientANYCPU
echo.
echo Building
echo.
%msbuildpath% LottoStats.sln /m /p:platform="Any CPU",configuration="Debug"
IF %ERRORLEVEL% EQU 0 goto :EOF
Echo.
ECHO. Failed to build
pause
goto :EOF
