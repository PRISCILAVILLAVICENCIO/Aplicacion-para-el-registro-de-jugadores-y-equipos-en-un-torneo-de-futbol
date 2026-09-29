@echo off
chcp 65001 > nul
if not exist "TorneoFutbolUEA.exe" (
    echo [INFO] El ejecutable no existe. Compilando primero...
    call compilar.bat
)

if exist "TorneoFutbolUEA.exe" (
    cls
    TorneoFutbolUEA.exe
) else (
    echo [ERROR] No se pudo encontrar ni generar TorneoFutbolUEA.exe
    pause
)
