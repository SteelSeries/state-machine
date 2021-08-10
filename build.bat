@echo off

echo **************************************************************************************
echo                                        BUILD
echo **************************************************************************************
msbuild StateMachine.sln /P:Configuration=Release
set BUILD_STATUS=%ERRORLEVEL% 
if %BUILD_STATUS%==0 goto TEST 
if not %BUILD_STATUS%==0 goto FAIL

:TEST
echo **************************************************************************************
echo                                        TEST
echo **************************************************************************************
dotnet vstest Tests\bin\Release\netcoreapp2.1\StateMachine.Tests.dll
set TEST_STATUS=%ERRORLEVEL% 
if %TEST_STATUS%==0 goto FINISH 
if not %TEST_STATUS%==0 goto FAIL
goto FINISH

:FINISH
echo:
echo SUCCEED
xcopy /Y /e /q StateMachine\bin\Release\* Out\
echo Publish to BaGet: nuget push -Source https://nuget.steelseries.io/v3/index.json package.nupkg
pause 
exit /b 1

:FAIL
pause 
exit /b 1 