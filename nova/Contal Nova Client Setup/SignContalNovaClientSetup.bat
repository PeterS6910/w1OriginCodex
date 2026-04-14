@echo off

setlocal EnableDelayedExpansion

if "%~1"=="" (
  set "TARGET_DIR=%CD%"
) else (
  set "TARGET_DIR=%~1"
)

set "OUTPUT_DIR=%TARGET_DIR%\Release - Setup"

call :sign "%OUTPUT_DIR%\Contal Nova Client Setup.msi"
call :sign "%OUTPUT_DIR%\setup.exe"

endlocal
exit /b 0

:sign
if not exist "%~1" (
  echo File not found: "%~1"
  exit /b 1
)

set "ENCODED_FILENAME=%~nx1"
set "ENCODED_FILENAME=!ENCODED_FILENAME: =%%20!"

curl -sS -X POST "https://sign.hour.sk/sign/?filename=!ENCODED_FILENAME!&url=https%3A%2F%2Fwww.contal.sk" ^
  -H "X-API-Key: %SIGN_API_KEY%" ^
  -H "X-Username: %SIGN_API_USER%" ^
  -F "file=@%~1" -o "%~1.signed"

if errorlevel 1 exit /b 1

move /Y "%~1.signed" "%~1"
if errorlevel 1 exit /b 1

exit /b 0