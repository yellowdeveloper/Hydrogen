@echo off

setlocal

set NATIVE_DIR=%~dp0
set PROJECT_DIR=%~dp0..

set OUT_DIR_DEB=%PROJECT_DIR%\bin\Debug
set OUT_DIR_REL=%PROJECT_DIR%\bin\Release

call "C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/Tools/VsDevCmd.bat"

if "%1"=="Debug" goto DEBUG
if "%1"=="Release" goto RELEASE

:DEBUG

cl /Zi /Od /c %NATIVE_DIR%main.c

link main.obj ^
    User32.lib ^
    /DEBUG ^
    /OUT:%OUT_DIR_DEB%\calib_debug.exe ^
    /SUBSYSTEM:CONSOLE

goto END

:RELEASE

cl /c %NATIVE_DIR%main.c

link main.obj ^
    User32.lib ^
    /OUT:%OUT_DIR_REL%\calibration.exe ^
    /SUBSYSTEM:CONSOLE

:END

del "%NATIVE_DIR%main.obj"

endlocal