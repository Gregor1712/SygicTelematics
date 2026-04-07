#!/bin/bash
# ============================================================
# Run this on the master node to set up a private Docker registry
# Usage: sudo bash 03-setup-registry.sh <master-ip>
# Example: sudo bash 03-setup-registry.sh 192.168.1.100
# ============================================================
set -euo pipefail

MASTER_IP=${1:?Usage: $0 <master-ip-address>}
REGISTRY="${MASTER_IP}:5000"

echo "=== Installing Docker (for building images) ==="
apt-get install -y docker.io
systemctl enable docker
systemctl start docker

echo "=== Starting private Docker registry ==="
docker run -d \
  --restart=always \
  --name registry \
  -p 5000:5000 \
  -v registry-data:/var/lib/registry \
  registry:2

echo "=== Configuring containerd to trust the registry ==="
# Add insecure registry to containerd
mkdir -p /etc/containerd/certs.d/"${REGISTRY}"
cat <<EOF > /etc/containerd/certs.d/"${REGISTRY}"/hosts.toml
[host."http://${REGISTRY}"]
  capabilities = ["pull", "resolve", "push"]
  skip_verify = true
EOF

# Also configure via config.toml
cat <<EOF >> /etc/containerd/config.toml

[plugins."io.containerd.grpc.v1.cri".registry.mirrors."${REGISTRY}"]
  endpoint = ["http://${REGISTRY}"]
[plugins."io.containerd.grpc.v1.cri".registry.configs."${REGISTRY}".tls]
  insecure_skip_verify = true
EOF

systemctl restart containerd

echo "=== Configuring Docker to trust the registry ==="
mkdir -p /etc/docker
cat <<EOF > /etc/docker/daemon.json
{
  "insecure-registries": ["${REGISTRY}"]
}
EOF

systemctl restart docker

echo ""
echo "============================================"
echo " Private registry running at ${REGISTRY}"
echo ""
echo " IMPORTANT: Run this on EACH worker node:"
echo "   Add to /etc/containerd/config.toml:"
echo "   [plugins.\"io.containerd.grpc.v1.cri\".registry.mirrors.\"${REGISTRY}\"]"
echo "     endpoint = [\"http://${REGISTRY}\"]"
echo "   Then: sudo systemctl restart containerd"
echo "============================================"
