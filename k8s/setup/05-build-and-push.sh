#!/bin/bash
# ============================================================
# Run this on the master node (where Docker + registry are)
# Builds all images and pushes to the private registry
# Usage: bash 05-build-and-push.sh <master-ip> <path-to-source>
# Example: bash 05-build-and-push.sh 192.168.1.100 /home/user/SygicTelematics
# ============================================================
set -euo pipefail

MASTER_IP=${1:?Usage: $0 <master-ip> <source-path>}
SRC=${2:?Usage: $0 <master-ip> <source-path>}
REGISTRY="${MASTER_IP}:5000"

SERVICES=(
  "gateway-api:src/ApiGateway/Gateway.API/Dockerfile"
  "identity-api:src/Services/Identity/Identity.API/Dockerfile"
  "catalog-api:src/Services/Catalog/Catalog.API/Dockerfile"
  "vehicle-api:src/Services/Vehicle/Vehicle.API/Dockerfile"
  "location-api:src/Services/Location/Location.API/Dockerfile"
  "battery-api:src/Services/Battery/Battery.API/Dockerfile"
  "trip-api:src/Services/Trip/Trip.API/Dockerfile"
  "telemetry-api:src/Services/Telemetry/Telemetry.API/Dockerfile"
  "alert-api:src/Services/Alert/Alert.API/Dockerfile"
)

cd "$SRC"

for entry in "${SERVICES[@]}"; do
  NAME="${entry%%:*}"
  DOCKERFILE="${entry##*:}"
  TAG="${REGISTRY}/${NAME}:latest"

  echo ""
  echo "=== Building ${NAME} ==="
  docker build -t "$TAG" -f "$DOCKERFILE" .

  echo "=== Pushing ${NAME} to ${REGISTRY} ==="
  docker push "$TAG"
done

echo ""
echo "============================================"
echo " All images built and pushed to ${REGISTRY}"
echo "============================================"
echo ""
docker images | grep "$REGISTRY"
