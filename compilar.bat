@echo off
chcp 65001 > nul
echo ===============================================================================
echo     Compilando Proyecto: Práctica 03 UEA - Conjuntos y Mapas en C#
echo ===============================================================================

set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if not exist %CSC% (
    set CSC="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
)

if exist %CSC% (
    echo [INFO] Utilizando compilador C#: %CSC%
    %CSC% /nologo /target:exe /out:TorneoFutbolUEA.exe Program.cs
    if %ERRORLEVEL% equ 0 (
        echo [EXITO] Compilación completada: TorneoFutbolUEA.exe generado correctamente.
    ) else (
        echo [ERROR] Ocurrió un error durante la compilación.
    )
) else (
    echo [INFO] Buscando dotnet en el sistema...
    dotnet build
)

pause
