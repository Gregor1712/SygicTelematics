#!/bin/bash
# ============================================================
# Run this script ONLY on the master node (k8s-master)
# Usage: sudo bash 02-master-init.sh <master-ip>
# Example: sudo bash 02-master-init.sh 192.168.1.100
# ============================================================
set -euo pipefail

MASTER_IP=${1:?Usage: $0 <master-ip-address>}

echo "=== Initializing Kubernetes control plane ==="
kubeadm init \
  --apiserver-advertise-address="$MASTER_IP" \
  --pod-network-cidr=10.244.0.0/16 \
  --control-plane-endpoint="$MASTER_IP:6443"

echo "=== Setting up kubeconfig for current user ==="
export HOME=/root
mkdir -p "$HOME/.kube"
cp /etc/kubernetes/admin.conf "$HOME/.kube/config"
chown "$(id -u):$(id -g)" "$HOME/.kube/config"

# Also set up for the non-root user if exists
if [ -n "${SUDO_USER:-}" ]; then
  USER_HOME=$(eval echo ~"$SUDO_USER")
  mkdir -p "$USER_HOME/.kube"
  cp /etc/kubernetes/admin.conf "$USER_HOME/.kube/config"
  chown "$SUDO_USER:$SUDO_USER" "$USER_HOME/.kube/config"
fi

echo "=== Installing Flannel CNI ==="
kubectl apply -f https://github.com/flannel-io/flannel/releases/latest/download/kube-flannel.yml

echo "=== Installing NGINX Ingress Controller ==="
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/baremetal/deploy.yaml

echo "=== Waiting for control plane to be ready ==="
kubectl wait --for=condition=Ready node/"$(hostname)" --timeout=120s

echo ""
echo "============================================"
echo " Master node initialized!"
echo ""
echo " To join worker nodes, run this on each worker:"
echo ""
kubeadm token create --print-join-command
echo ""
echo "============================================"
