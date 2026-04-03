# Testing Guide

## Prerequisites

1. **SQL Server** running on localhost (Trusted Connection)
2. **RabbitMQ** running on localhost:5672
3. **.NET 8 SDK** installed

## 1. Start RabbitMQ (Docker)

```bash
docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:management
```

RabbitMQ Management UI: http://localhost:15672 (guest / guest)

## 2. Start All Services

Run from the project root directory:

```bash
cd C:\Users\grego\RiderProjects\SygicTelematics

start /B dotnet run --project src/Services/Vehicle/Vehicle.API --urls http://localhost:5103 --environment Development
start /B dotnet run --project src/Services/Location/Location.API --urls http://localhost:5104 --environment Development
start /B dotnet run --project src/Services/Battery/Battery.API --urls http://localhost:5105 --environment Development
start /B dotnet run --project src/Services/Trip/Trip.API --urls http://localhost:5106 --environment Development
start /B dotnet run --project src/Services/Telemetry/Telemetry.API --urls http://localhost:5107 --environment Development
start /B dotnet run --project src/Services/Alert/Alert.API --urls http://localhost:5108 --environment Development
start /B dotnet run --project src/ApiGateway/Gateway.API --urls http://localhost:5100 --environment Development
```

### Service Ports

| Service      | Port | Swagger UI                          |
|-------------|------|-------------------------------------|
| Gateway BFF | 5100 | http://localhost:5100/swagger       |
| Vehicle     | 5103 | http://localhost:5103/swagger       |
| Location    | 5104 | http://localhost:5104/swagger       |
| Battery     | 5105 | http://localhost:5105/swagger       |
| Trip        | 5106 | http://localhost:5106/swagger       |
| Telemetry   | 5107 | http://localhost:5107/swagger       |
| Alert       | 5108 | http://localhost:5108/swagger       |

## 3. Stop All Services

```bash
taskkill /F /IM "Vehicle.API.exe"
taskkill /F /IM "Location.API.exe"
taskkill /F /IM "Battery.API.exe"
taskkill /F /IM "Trip.API.exe"
taskkill /F /IM "Telemetry.API.exe"
taskkill /F /IM "Alert.API.exe"
taskkill /F /IM "Gateway.API.exe"
```

---

## 4. Testing Commands (curl)

Use a sample vehicle ID from seed data:
```
VEHICLE_ID = 2c33e6b8-f7cc-478d-b5be-f14ee7a768da
```

### GET - Read Data

```bash
# Get all vehicles
curl http://localhost:5103/api/vehicles

# Get single vehicle
curl http://localhost:5103/api/vehicles/2c33e6b8-f7cc-478d-b5be-f14ee7a768da

# Get vehicle locations
curl http://localhost:5104/api/locations/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da

# Get vehicle battery statuses
curl http://localhost:5105/api/battery/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da

# Get vehicle trips
curl http://localhost:5106/api/trips/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da

# Get vehicle telemetry
curl http://localhost:5107/api/telemetry/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da

# Get vehicle alerts
curl http://localhost:5108/api/alerts/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da

# BFF - Get all vehicles
curl http://localhost:5100/api/bff/vehicles

# BFF - Get aggregated vehicle detail (all data in one call)
curl http://localhost:5100/api/bff/vehicles/2c33e6b8-f7cc-478d-b5be-f14ee7a768da
```

### POST - Create Data (triggers RabbitMQ events)

```bash
# Post new location -> updates Vehicle.CurrentLocationId via RabbitMQ
curl -X POST http://localhost:5104/api/locations -H "Content-Type: application/json" -d "{\"vehicleId\":\"2c33e6b8-f7cc-478d-b5be-f14ee7a768da\",\"latitude\":48.1486,\"longitude\":17.1077,\"speed\":65.5}"

# Post new battery status -> updates Vehicle.BatteryStatusId via RabbitMQ
curl -X POST http://localhost:5105/api/battery -H "Content-Type: application/json" -d "{\"vehicleId\":\"2c33e6b8-f7cc-478d-b5be-f14ee7a768da\",\"percentage\":45,\"voltage\":395.2,\"temperature\":32.1,\"isCharging\":false}"

# Post new trip -> updates Vehicle.LastTripId via RabbitMQ
curl -X POST http://localhost:5106/api/trips -H "Content-Type: application/json" -d "{\"vehicleId\":\"2c33e6b8-f7cc-478d-b5be-f14ee7a768da\",\"startTime\":\"2026-04-03T08:00:00\",\"endTime\":\"2026-04-03T09:30:00\",\"distanceKm\":72.5,\"averageSpeed\":48.3,\"energyConsumed\":14.2}"

# Verify cached IDs updated on vehicle
curl http://localhost:5103/api/vehicles/2c33e6b8-f7cc-478d-b5be-f14ee7a768da
```

---

## 5. Postman Collection

Import these URLs into Postman:

### GET Requests

| Name | Method | URL |
|------|--------|-----|
| All Vehicles | GET | `http://localhost:5103/api/vehicles` |
| Vehicle by ID | GET | `http://localhost:5103/api/vehicles/2c33e6b8-f7cc-478d-b5be-f14ee7a768da` |
| Vehicle Locations | GET | `http://localhost:5104/api/locations/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da` |
| Vehicle Battery | GET | `http://localhost:5105/api/battery/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da` |
| Vehicle Trips | GET | `http://localhost:5106/api/trips/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da` |
| Vehicle Telemetry | GET | `http://localhost:5107/api/telemetry/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da` |
| Vehicle Alerts | GET | `http://localhost:5108/api/alerts/vehicle/2c33e6b8-f7cc-478d-b5be-f14ee7a768da` |
| BFF - All Vehicles | GET | `http://localhost:5100/api/bff/vehicles` |
| BFF - Vehicle Detail | GET | `http://localhost:5100/api/bff/vehicles/2c33e6b8-f7cc-478d-b5be-f14ee7a768da` |

### POST Requests (set Content-Type: application/json)

**POST Location** `http://localhost:5104/api/locations`
```json
{
  "vehicleId": "2c33e6b8-f7cc-478d-b5be-f14ee7a768da",
  "latitude": 48.1486,
  "longitude": 17.1077,
  "speed": 65.5
}
```

**POST Battery Status** `http://localhost:5105/api/battery`
```json
{
  "vehicleId": "2c33e6b8-f7cc-478d-b5be-f14ee7a768da",
  "percentage": 45,
  "voltage": 395.2,
  "temperature": 32.1,
  "isCharging": false
}
```

**POST Trip** `http://localhost:5106/api/trips`
```json
{
  "vehicleId": "2c33e6b8-f7cc-478d-b5be-f14ee7a768da",
  "startTime": "2026-04-03T08:00:00",
  "endTime": "2026-04-03T09:30:00",
  "distanceKm": 72.5,
  "averageSpeed": 48.3,
  "energyConsumed": 14.2
}
```

---

## 6. Event-Driven Flow (RabbitMQ)

```
POST /api/locations ──► Location DB ──► RabbitMQ ──► Vehicle Service ──► Vehicle.CurrentLocationId updated
POST /api/battery   ──► Battery DB  ──► RabbitMQ ──► Vehicle Service ──► Vehicle.BatteryStatusId updated
POST /api/trips     ──► Trip DB     ──► RabbitMQ ──► Vehicle Service ──► Vehicle.LastTripId updated
```

To verify: after any POST, call `GET /api/vehicles/{id}` and check the cached ID fields.