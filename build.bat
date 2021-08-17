@echo off

REM Detects a (likely) launch from Windows GUI, thus requiring a pause when script exits

(((ECHO.%CMDCMDLINE%)|FIND /I "/C")>NUL)
IF %ERRORLEVEL% EQU 0 SET GUI=1

SET DefMSBuildPath="%WinDir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
SET DefMSBuild2015Path="%ProgramFiles(x86)%\MSBuild\14.0\bin\MSBuild.exe"

rem IF "%MSBUILD_PATH%" == ""        CALL :setdefmsbuildpath

echo Generate assets file
dotnet restore

echo **************************************************************************************
echo                                        BUILD
echo **************************************************************************************
msbuild StateMachine.sln /P:Configuration=Release
if %ERRORLEVEL%==0 goto test 
if not %ERRORLEVEL%==0 goto error

:test
echo **************************************************************************************
echo                                        TEST
echo **************************************************************************************
dotnet vstest Tests\bin\Release\netcoreapp2.1\StateMachine.Tests.dll
if %ERRORLEVEL%==0 goto finish 
if not %ERRORLEVEL%==0 goto error

:finish
echo:
xcopy /Y /e /q StateMachine\bin\Release\* Out\
echo Publish to BaGet: nuget push -Source https://nuget.steelseries.io/v3/index.json package.nupkg
goto success

REM Selects a default path to MSBuild 
:setdefmsbuildpath
IF %ToolsetVer% GEQ 140 (
	echo Set MSBUILD to 2015
	SET MSBUILD_PATH=%DefMSBuild2015Path%
) ELSE (
	SET MSBUILD_PATH=%DefMSBuildPath%
)
ECHO WARNING: MSBUILD_PATH was not defined, using default
GOTO :EOF

:success
ECHO ------------------------------------------------
ECHO --------------------SUCCESS---------------------
ECHO ------------------------------------------------
GOTO end

:error
ECHO ------------------------------------------------
ECHO ---------------------ERROR----------------------
ECHO ------------------------------------------------
GOTO end

:cancel
ECHO ------------------------------------------------
ECHO -------------------CANCELLED--------------------
ECHO ------------------------------------------------
GOTO end

:end
IF DEFINED GUI (
	IF "%NoPrompt%" == "false" PAUSE
)
EXIT /B %ERRORLEVEL%