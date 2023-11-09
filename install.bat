@ECHO OFF

SETLOCAL
@rem locate visua studio
for /F "tokens=*" %%i in ('^""%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath^"') DO (
    SET "VSINSTALLDIR=%%i"
)

@rem example visual studio path
@rem C:\Program Files\Microsoft Visual Studio\2022\Professional

@rem example csi path
@rem C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\Roslyn\csi.exe

"%VSINSTALLDIR%\Msbuild\Current\Bin\Roslyn\csi.exe" install.csx %USERPROFILE%\.kdiff3rc .kdiff3rc