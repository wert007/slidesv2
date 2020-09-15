@echo off

call docs.cmd
if ERRORLEVEL 1 goto:tests_failed

setlocal
::%~p1
REM Set a string with an arbitrary number of substrings separated by semi colons
set directory=%~p1

REM Do something with each substring
:stringLOOP
    REM Stop when the string is empty
    if "%directory%" EQU "" goto END

    for /f "delims=\" %%a in ("%directory%") do set last_directory=%%a


REM Now strip off the leading substring
:striploop
    set stripchar=%directory:~0,1%
    set directory=%directory:~1%

    if "%directory%" EQU "" goto stringloop

    if "%stripchar%" NEQ "\" goto striploop

    goto stringloop
)

:END
dotnet .\bin\Debug\netcoreapp2.1\mc.dll %1 %last_directory% %2 %3 %4 %5
goto:eof
endlocal


:tests_failed
echo a test failed. pls check before running again!