#!/bin/bash
# ============================================================
# Run this on the master node to deploy everything to K8s
# Usage: bash 06-deploy-to-k8s.sh <path-to-k8s-manifests>
# Example: bash 06-deploy-to-k8s.sh /home/user/SygicTelematics/k8s
# ============================================================
set -euo pipefail

K8S_DIR=${1:?Usage: $0 <path-to-k8s-manifests-dir>}

echo "=== Creating namespace ==="
kubectl apply -f "$K8S_DIR/namespace.yml"

echo "=== Deploying ConfigMap & Secrets ==="
kubectl apply -f "$K8S_DIR/configmap.yml"

echo "=== Deploying Infrastructure (SQL Server + RabbitMQ) ==="
kubectl apply -f "$K8S_DIR/infrastructure/"

echo "=== Waiting for SQL Server to be ready ==="
kubectl -n sygic-telematics wait --for=condition=Available deployment/sqlserver --timeout=120s

echo "=== Waiting for RabbitMQ to be ready ==="
kubectl -n sygic-telematics wait --for=condition=Available deployment/rabbitmq --timeout=120s

echo "=== Deploying microservices ==="
kubectl apply -f "$K8S_DIR/services/"

echo "=== Deploying Ingress ==="
kubectl apply -f "$K8S_DIR/ingress.yml"

echo ""
echo "=== Waiting for all deployments ==="
kubectl -n sygic-telematics get deployments

echo ""
echo "============================================"
echo " Deployment complete!"
echo " Run: kubectl -n sygic-telematics get pods"
echo "============================================"
