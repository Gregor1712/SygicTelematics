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
start "Identity.API" dotnet run --project src/Services/Identity/Identity.API --urls http://localhost:5101 --environment Development
start "Vehicle.API" dotnet run --project src/Services/Vehicle/Vehicle.API --environment Development
start "Location.API" dotnet run --project src/Services/Location/Location.API --environment Development
start "Battery.API" dotnet run --project src/Services/Battery/Battery.API --environment Development
start "Trip.API" dotnet run --project src/Services/Trip/Trip.API --environment Development
start "Telemetry.API" dotnet run --project src/Services/Telemetry/Telemetry.API --environment Development
start "Alert.API" dotnet run --project src/Services/Alert/Alert.API --environment Development
start "Gateway.API" dotnet run --project src/ApiGateway/Gateway.API --urls http://localhost:5100 --environment Development

echo.
echo All services started:
echo   Identity.API   - http://localhost:5101
echo   Vehicle.API    - http://localhost:5103 (gRPC: 6103)
echo   Location.API   - http://localhost:5104 (gRPC: 6104)
echo   Battery.API    - http://localhost:5105 (gRPC: 6105)
echo   Trip.API       - http://localhost:5106 (gRPC: 6106)
echo   Telemetry.API  - http://localhost:5107 (gRPC: 6107)
echo   Alert.API      - http://localhost:5108 (gRPC: 6108)
echo   Gateway.API    - http://localhost:5100
echo   GraphQL        - http://localhost:5100/graphql
echo.
echo To stop all services, run: stop-all.bat
