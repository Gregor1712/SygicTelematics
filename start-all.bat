@echo off
echo Stopping existing services...
taskkill /F /IM "Identity.API.exe" 2>nul
taskkill /F /IM "Vehicle.API.exe" 2>nul
taskkill /F /IM "Location.API.exe" 2>nul
taskkill /F /IM "Battery.API.exe" 2>nul
taskkill /F /IM "Trip.API.exe" 2>nul
taskkill /F /IM "Telemetry.API.exe" 2>nul
taskkill /F /IM "Alert.API.exe" 2>nul
taskkill /F /IM "Gateway.API.exe" 2>nul
ping -n 3 127.0.0.1 >nul

echo Starting all services...
start /B dotnet run --project src/Services/Identity/Identity.API --urls http://localhost:5101 --environment Development
start /B dotnet run --project src/Services/Vehicle/Vehicle.API --urls http://localhost:5103 --environment Development
start /B dotnet run --project src/Services/Location/Location.API --urls http://localhost:5104 --environment Development
start /B dotnet run --project src/Services/Battery/Battery.API --urls http://localhost:5105 --environment Development
start /B dotnet run --project src/Services/Trip/Trip.API --urls http://localhost:5106 --environment Development
start /B dotnet run --project src/Services/Telemetry/Telemetry.API --urls http://localhost:5107 --environment Development
start /B dotnet run --project src/Services/Alert/Alert.API --urls http://localhost:5108 --environment Development
start /B dotnet run --project src/ApiGateway/Gateway.API --urls http://localhost:5100 --environment Development

echo.
echo All services started:
echo   Identity.API   - http://localhost:5101
echo   Vehicle.API    - http://localhost:5103
echo   Location.API   - http://localhost:5104
echo   Battery.API    - http://localhost:5105
echo   Trip.API       - http://localhost:5106
echo   Telemetry.API  - http://localhost:5107
echo   Alert.API      - http://localhost:5108
echo   Gateway.API    - http://localhost:5100
echo.
echo Press any key to stop all services...
pause >nul

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
