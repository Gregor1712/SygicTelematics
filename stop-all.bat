@echo off
echo Stopping all services...
taskkill /F /IM "Identity.API.exe" 2>nul
taskkill /F /IM "Vehicle.API.exe" 2>nul
taskkill /F /IM "Location.API.exe" 2>nul
taskkill /F /IM "Battery.API.exe" 2>nul
taskkill /F /IM "Trip.API.exe" 2>nul
taskkill /F /IM "Telemetry.API.exe" 2>nul
taskkill /F /IM "Alert.API.exe" 2>nul
taskkill /F /IM "Gateway.API.exe" 2>nul
echo All services stopped.
