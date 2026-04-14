@echo off

setlocal

REM Should always be specified WITHOUT quotes !!!
set MAINPATH=C:\Sources\ContalNova
set BUILDLOG=build.log
if exist %BUILDLOG% erase %BUILDLOG%

if not exist %MAINPATH% (
	echo Standard layout for build machine is required.
	echo ContalNova sources must be situated in %MAINPATH% directory
	echo.
	
	PAUSE
	
	exit
)

svn --username CONTAL\developer update --accept tc %MAINPATH% 2>&1 | wtee -a %BUILDLOG%

REM PAUSE

set MSBUILD="C:\Windows\Microsoft.NET\Framework\v3.5\MsBuild.exe"


echo Cleaning solution - Release ... | wtee -a %BUILDLOG%
echo.
call %MSBUILD% "%MAINPATH%\CGP-NCAS 2.1.sln" /v:m /clp:ErrorsOnly /t:clean /p:Configuration="Release" 2>&1 | wtee -a %BUILDLOG%

echo Cleaning solution - Release - CodeWall ... | wtee -a %BUILDLOG%
echo.
call %MSBUILD% "%MAINPATH%\CGP-NCAS 2.1.sln" /v:m /clp:ErrorsOnly /t:clean /p:Configuration="Release - CodeWall" 2>&1 | wtee -a %BUILDLOG%

REM PAUSE

echo Building Client ... | wtee -a %BUILDLOG%
echo.
call %MSBUILD% "%MAINPATH%\Cgp.NCAS.Client\Cgp.NCAS.Client.csproj" /v:m /clp:ErrorsOnly /p:Configuration="Release - CodeWall" /p:SolutionDir="%MAINPATH%\\" /p:OutputPath="bin\Release - CodeWall" /p:DefineConstants="PC,CSharp" 2>&1 | wtee -a %BUILDLOG%
REM | wtee -a %BUILDLOG% 2>&1

echo Getting current .NET version | wtee -a %BUILDLOG%
echo.

set commandForOutput=C:\sources\ContalNova\Cgp.NCAS.CCU\Binaries\FilePackerTool.exe /outversion "%MAINPATH%\Cgp.NCAS.Client\bin\Release - Codewall\Secured\Contal Nova Client.exe"

for /f "delims=" %%a in ('%commandForOutput% ^| findstr /v "cokolvek"') do @set VERSION=%%a

echo Extracted Client .NET version : %VERSION% | wtee -a %BUILDLOG%

REM PAUSE



echo Building License manager | wtee -a %BUILDLOG%
echo.
call %MSBUILD% "%MAINPATH%\Cgp\Cgp.ContalNovaLicenseManager\Cgp.ContalNovaLicenseManager.csproj" /v:m /clp:ErrorsOnly /p:Configuration="Release - CodeWall" /p:SolutionDir="%MAINPATH%\\" /p:OutputPath="bin\Release - CodeWall" /p:DefineConstants="PC,CSharp" 2>&1 | wtee -a %BUILDLOG% 

REM PAUSE

echo Building Server | wtee -a %BUILDLOG%
echo.
call %MSBUILD% "%MAINPATH%\Cgp.NCAS.Server\Cgp.NCAS.Server.csproj" /v:m /clp:ErrorsOnly /p:Configuration="Release - CodeWall" /p:SolutionDir="%MAINPATH%\\" /p:OutputPath="bin\Release - CodeWall" /p:DefineConstants="PC,CSharp" 2>&1 | wtee -a %BUILDLOG% 

set commandForOutput=C:\sources\ContalNova\Cgp.NCAS.CCU\Binaries\FilePackerTool.exe /outversion "%MAINPATH%\Cgp.NCAS.Server\bin\Release - Codewall\Secured\Contal Nova Server.exe"

for /f "delims=" %%a in ('%commandForOutput% ^| findstr /v "cokolvek"') do @set VERSIONS=%%a

echo Extracted Server .NET version : %VERSIONS% | wtee -a %BUILDLOG%

REM PAUSE

echo Building CCUupgrader | wtee -a %BUILDLOG%
echo.
call %MSBUILD% "%MAINPATH%\Cgp.NCAS.CCUUpgrader\Cgp.NCAS.CCUUpgrader.csproj" /v:m /clp:ErrorsOnly /p:Configuration="Release" /p:SolutionDir="%MAINPATH%\\" 2>&1 | wtee -a %BUILDLOG% 

REM PAUSE


echo Building CCU | wtee -a %BUILDLOG%
echo.
call %MSBUILD% "%MAINPATH%\Cgp.NCAS.CCU\Cgp.NCAS.CCU.csproj" /v:m /clp:ErrorsOnly /p:Configuration="Release" /p:SolutionDir="%MAINPATH%\\" 2>&1 | wtee -a %BUILDLOG% 

echo.
echo Build had reached the end of processing... | wtee -a %BUILDLOG%
echo ... Now continues the setup assembly | wtee -a %BUILDLOG%
echo.

REM PAUSE

echo Building Server setup | wtee -a %BUILDLOG%
echo.
set VSEXE="C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"
set TMPLOG="c:\tmp\tmp.log"

if exist %TMPLOG% del %TMPLOG%

call %VSEXE% "%MAINPATH%\Contal Nova Server Setup\Contal Nova Server Setup.vdproj" /Build "Release - Setup" /Out %TMPLOG% 2>&1 | wtee -a %BUILDLOG%
type %TMPLOG%  | wtee -a %BUILDLOG%

echo Building Client setup %MAINPATH% | wtee -a %BUILDLOG%
echo.
set VSEXE="C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"
set TMPLOG="c:\tmp\tmp.log"

if exist %TMPLOG% del %TMPLOG%

call %VSEXE% "%MAINPATH%\Contal Nova Client Setup\Contal Nova Client Setup.vdproj" /Build "Release - Setup" /Out %TMPLOG%  2>&1 | wtee -a %BUILDLOG% 
type %TMPLOG%  | wtee -a %BUILDLOG%

echo.
echo Setup assembly had reached the end of processing... | wtee -a %BUILDLOG%
echo.

REM Should always be specified WITHOUT quotes !!!
set ROOTPATH=\\vmfileserver\ContalNova\_RELEASES

echo Creating automated build structure | wtee -a %BUILDLOG%
echo.
set DSTFOLDER="%ROOTPATH%\%VERSION% Automated SECURED"
md %DSTFOLDER% %DSTFOLDER%\CE %DSTFOLDER%\CR %DSTFOLDER%\CCU %DSTFOLDER%\DCU %DSTFOLDER%\Client %DSTFOLDER%\Server 2>&1  | wtee -a %BUILDLOG%

echo Copying binaries into automated build structure | wtee -a %BUILDLOG%
echo.
copy /Y /Z %MAINPATH%\Cgp.NCAS.CCU\bin\*%VERSION%*.gz %DSTFOLDER%\CCU\ 2>&1 | wtee -a %BUILDLOG%

copy /Y /Z  "%MAINPATH%\Contal Nova Server Setup\Release - Setup\*.exe" %DSTFOLDER%\Server\  1>nul 2>&1 | wtee -a %BUILDLOG%
copy /Y /Z  "%MAINPATH%\Contal Nova Server Setup\Release - Setup\*.msi" %DSTFOLDER%\Server\  1>nul 2>&1 | wtee -a %BUILDLOG%

copy /Y /Z  "%MAINPATH%\Contal Nova Client Setup\Release - Setup\*.exe" %DSTFOLDER%\Client\  1>nul 2>&1 | wtee -a %BUILDLOG%
copy /Y /Z  "%MAINPATH%\Contal Nova Client Setup\Release - Setup\*.msi" %DSTFOLDER%\Client\  1>nul 2>&1 | wtee -a %BUILDLOG%

copy /Y /Z "%MAINPATH%\build.log" %DSTFOLDER%\

endlocal

if /I not "%1" == "auto" PAUSE
