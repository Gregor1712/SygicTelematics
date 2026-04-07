#!/bin/bash
# ============================================================
# Run this on EACH worker node to trust the private registry
# Usage: sudo bash 04-worker-registry.sh <master-ip>
# Example: sudo bash 04-worker-registry.sh 192.168.1.100
# ============================================================
set -euo pipefail

MASTER_IP=${1:?Usage: $0 <master-ip-address>}
REGISTRY="${MASTER_IP}:5000"

echo "=== Configuring containerd to trust registry at ${REGISTRY} ==="

mkdir -p /etc/containerd/certs.d/"${REGISTRY}"
cat <<EOF > /etc/containerd/certs.d/"${REGISTRY}"/hosts.toml
[host."http://${REGISTRY}"]
  capabilities = ["pull", "resolve"]
  skip_verify = true
EOF

cat <<EOF >> /etc/containerd/config.toml

[plugins."io.containerd.grpc.v1.cri".registry.mirrors."${REGISTRY}"]
  endpoint = ["http://${REGISTRY}"]
[plugins."io.containerd.grpc.v1.cri".registry.configs."${REGISTRY}".tls]
  insecure_skip_verify = true
EOF

systemctl restart containerd

echo "=== Done! Worker can now pull from ${REGISTRY} ==="
