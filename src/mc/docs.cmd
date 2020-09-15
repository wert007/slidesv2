@echo off
for %%f in (.\sldsrc\docs\*.sld) do (
    dotnet .\bin\Debug\netcoreapp2.1\mc.dll %%f docs\%%~nf -q
    : 0 is all good
    : 1 is warnings found
    if ERRORLEVEL 1 goto:eof 
)

:no_fail
python docs_test_success.py
if ERRORLEVEL 1 echo Slides didn't compile right