# Sygic Telematics - API Documentation

## Architecture Overview

This project is a **microservices-based vehicle telematics platform** built with .NET 8, RabbitMQ, and SQL Server.

```
┌─────────────────────────────────────────────────────────────────┐
│                    Gateway API (:5100)                          │
│              BFF (Backend For Frontend)                         │
│         JWT Authentication + Request Routing                    │
└──────┬──────┬──────┬──────┬──────┬──────┬──────┬───────────────┘
       │      │      │      │      │      │      │
       ▼      ▼      ▼      ▼      ▼      ▼      ▼
  Identity Vehicle Location Battery  Trip Telemetry Alert
  (:5101) (:5103) (:5104)  (:5105) (:5106) (:5107) (:5108)
                                      │              ▲
                                      │  RabbitMQ    │
                                      └──────────────┘
                                    TelemetryAlertEvent
                                    (threshold exceeded)
```

### Services

| Service | Port | Description |
|---------|------|-------------|
| **Gateway.API** | 5100 | BFF gateway, JWT auth, aggregates responses |
| **Identity.API** | 5101 | User registration, login, JWT tokens |
| **Vehicle.API** | 5103 | Vehicle CRUD with filtering, sorting, pagination |
| **Location.API** | 5104 | GPS location tracking |
| **Battery.API** | 5105 | Battery status monitoring |
| **Trip.API** | 5106 | Trip recording |
| **Telemetry.API** | 5107 | Sensor data (engine temp, RPM, tire pressure, etc.) |
| **Alert.API** | 5108 | Alerts generated from telemetry thresholds + email notifications |

### Event-Driven Communication (RabbitMQ)

| Event | Publisher | Consumer | Action |
|-------|-----------|----------|--------|
| `LocationUpdatedEvent` | Location.API | Vehicle.API | Updates `CurrentLocationId` on vehicle |
| `BatteryStatusUpdatedEvent` | Battery.API | Vehicle.API | Updates `BatteryStatusId` on vehicle |
| `TripCompletedEvent` | Trip.API | Vehicle.API | Updates `LastTripId` on vehicle |
| `TelemetryAlertEvent` | Telemetry.API | Alert.API | Creates alert + sends email notification |

### Alert Thresholds

When telemetry values exceed these thresholds, an alert is automatically created and an email notification is sent:

| Telemetry Type | Condition | Alert Type |
|----------------|-----------|------------|
| `engine_temp` | > 100°C | HIGH_TEMP |
| `coolant_temp` | > 105°C | HIGH_TEMP |
| `oil_pressure` | < 1.0 bar | LOW_OIL_PRESSURE |
| `tire_pressure_fl/fr/rl/rr` | < 1.8 bar | LOW_TIRE_PRESSURE |
| `rpm` | > 6000 | HIGH_RPM |

---

## Starting All Services

Run from the project root:

```
start-all.bat
```

Press any key to stop all services.

---

## API Endpoints & Postman Examples

### 1. Authentication

#### Register a new user

```
POST http://localhost:5100/api/account/register
Content-Type: application/json

{
  "displayName": "John Doe",
  "email": "john@example.com",
  "password": "MyPassword123!"
}
```

#### Login

```
POST http://localhost:5100/api/account/login
Content-Type: application/json

{
  "email": "user100@gmail.com",
  "password": "User100#123"
}
```

**Response:**
```json
{
  "id": "08cf4a1c-648b-4564-899c-a3b699b5176e",
  "email": "user100@gmail.com",
  "displayName": "user100@gmail.com",
  "token": "eyJhbGciOiJIUzUxMiIs...",
  "roles": ["User"]
}
```

Copy the `token` value. In Postman, go to **Authorization tab** → Type: **Bearer Token** → paste the token.

---

### 2. Vehicles (via Gateway - requires auth)

#### Get all vehicles (with filtering, sorting, pagination)

```
GET http://localhost:5100/api/bff/vehicles?PageNumber=1&PageSize=10
Authorization: Bearer <token>
```

#### Filter by manufacturer (Contains)

```
GET http://localhost:5100/api/bff/vehicles?Manufacturer.Values=BMW&Manufacturer.Operator=Contains
Authorization: Bearer <token>
```

#### Filter by manufacturer (Equals) + sort by year

```
GET http://localhost:5100/api/bff/vehicles?Manufacturer.Values=BMW&Manufacturer.Operator=Equals&SortProperty=Year&SortDirection=Descending&PageNumber=1&PageSize=10
Authorization: Bearer <token>
```

#### Filter by VIN (StartsWith)

```
GET http://localhost:5100/api/bff/vehicles?Vin.Values=SLWA&Vin.Operator=StartsWith
Authorization: Bearer <token>
```

#### Filter by multiple manufacturers (In)

```
GET http://localhost:5100/api/bff/vehicles?Manufacturer.Values=BMW&Manufacturer.Values=Audi&Manufacturer.Operator=In
Authorization: Bearer <token>
```

**Available filter operators:** `Equals`, `NotEquals`, `Contains`, `StartsWith`, `EndsWith`, `In`, `NotIn`, `IsNull`, `IsNotNull`

**Available sort directions:** `Ascending`, `Descending`

**Sortable properties:** `Vin`, `Model`, `Manufacturer`, `Year`

#### Get aggregated vehicle detail

```
GET http://localhost:5100/api/bff/vehicles/{vehicleId}
```

Returns vehicle + location + battery + trips + telemetry + alerts in a single response.

**Example:**
```
GET http://localhost:5100/api/bff/vehicles/4a689ee2-fa42-47fe-a72f-02213a4def17
```

---

### 3. Location (via Gateway)

#### Get locations by vehicle

```
GET http://localhost:5100/api/bff/locations/vehicle/{vehicleId}
```

#### Create location (publishes LocationUpdatedEvent → updates vehicle)

```
POST http://localhost:5100/api/bff/locations
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "latitude": 48.1486,
  "longitude": 17.1077,
  "speed": 65.5
}
```

---

### 4. Battery (via Gateway)

#### Get battery status by vehicle

```
GET http://localhost:5100/api/bff/battery/vehicle/{vehicleId}
```

#### Create battery status (publishes BatteryStatusUpdatedEvent → updates vehicle)

```
POST http://localhost:5100/api/bff/battery
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "percentage": 85,
  "voltage": 398.5,
  "temperature": 28.3,
  "isCharging": false
}
```

---

### 5. Trips (via Gateway)

#### Get trips by vehicle

```
GET http://localhost:5100/api/bff/trips/vehicle/{vehicleId}
```

#### Create trip (publishes TripCompletedEvent if endTime is set → updates vehicle)

```
POST http://localhost:5100/api/bff/trips
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "startTime": "2026-04-05T08:00:00Z",
  "endTime": "2026-04-05T09:30:00Z",
  "distanceKm": 45.2,
  "averageSpeed": 30.1,
  "fuelConsumed": null,
  "energyConsumed": 12.5
}
```

---

### 6. Telemetry (via Gateway)

#### Get telemetry by vehicle

```
GET http://localhost:5100/api/bff/telemetry/vehicle/{vehicleId}
```

#### Create telemetry record (publishes TelemetryAlertEvent if threshold exceeded)

**Normal value (no alert):**
```
POST http://localhost:5100/api/bff/telemetry
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "type": "engine_temp",
  "value": 85.0
}
```

**High engine temperature (triggers HIGH_TEMP alert + email):**
```
POST http://localhost:5100/api/bff/telemetry
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "type": "engine_temp",
  "value": 115.5
}
```

**Low oil pressure (triggers LOW_OIL_PRESSURE alert + email):**
```
POST http://localhost:5100/api/bff/telemetry
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "type": "oil_pressure",
  "value": 0.5
}
```

**Low tire pressure (triggers LOW_TIRE_PRESSURE alert + email):**
```
POST http://localhost:5100/api/bff/telemetry
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "type": "tire_pressure_fl",
  "value": 1.2
}
```

**High RPM (triggers HIGH_RPM alert + email):**
```
POST http://localhost:5100/api/bff/telemetry
Content-Type: application/json

{
  "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
  "type": "rpm",
  "value": 7200
}
```

**Available telemetry types:** `engine_temp`, `coolant_temp`, `oil_pressure`, `tire_pressure_fl`, `tire_pressure_fr`, `tire_pressure_rl`, `tire_pressure_rr`, `rpm`, `fuel_level`

---

### 7. Alerts (via Gateway)

#### Get alerts by vehicle

```
GET http://localhost:5100/api/bff/alerts/vehicle/{vehicleId}
```

**Example response:**
```json
[
  {
    "id": "a77ca593-e4bf-49ce-afa7-c28eb0ad99f7",
    "vehicleId": "4a689ee2-fa42-47fe-a72f-02213a4def17",
    "type": "HIGH_TEMP",
    "message": "Engine temperature above 100°C (value: 115.50)",
    "isResolved": false,
    "createdAt": "2026-04-05T09:29:21"
  }
]
```

---

## End-to-End Test Scenario

### Scenario: Vehicle sends high engine temperature → alert created → email sent

All requests go through the **Gateway** at `http://localhost:5100`.

1. **Login** to get a JWT token:
   ```
   POST http://localhost:5100/api/account/login
   Content-Type: application/json

   {"email": "user100@gmail.com", "password": "User100#123"}
   ```

2. **Get vehicles** to find a vehicle ID:
   ```
   GET http://localhost:5100/api/bff/vehicles?PageNumber=1&PageSize=1
   Authorization: Bearer <token>
   ```

3. **Send telemetry** with engine temp above threshold:
   ```
   POST http://localhost:5100/api/bff/telemetry
   Content-Type: application/json

   {"vehicleId": "<vehicleId>", "type": "engine_temp", "value": 120}
   ```

4. **Check alerts** were created:
   ```
   GET http://localhost:5100/api/bff/alerts/vehicle/<vehicleId>
   ```

5. **Check email** at https://ethereal.email (login: `nce6duw2qv6pbchc@ethereal.email` / `94VFUDDTe1ugEMQj1X`)

6. **View full vehicle detail** via gateway (aggregated from all services):
   ```
   GET http://localhost:5100/api/bff/vehicles/<vehicleId>
   ```

---

## Swagger UI

Each service has Swagger UI available in Development mode:

- Gateway: http://localhost:5100/swagger
- Vehicle: http://localhost:5103/swagger
- Location: http://localhost:5104/swagger
- Battery: http://localhost:5105/swagger
- Trip: http://localhost:5106/swagger
- Telemetry: http://localhost:5107/swagger
- Alert: http://localhost:5108/swagger
