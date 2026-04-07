#!/bin/bash
# ============================================================
# Run this script on ALL 3 nodes (master + worker1 + worker2)
# Usage: sudo bash 01-all-nodes-setup.sh <hostname>
# Example: sudo bash 01-all-nodes-setup.sh k8s-master
# ============================================================
set -euo pipefail

HOSTNAME=${1:?Usage: $0 <hostname> (k8s-master|k8s-worker1|k8s-worker2)}

echo "=== Setting hostname to $HOSTNAME ==="
hostnamectl set-hostname "$HOSTNAME"

echo "=== Updating system ==="
apt-get update && apt-get upgrade -y

echo "=== Disabling swap (Kubernetes requirement) ==="
swapoff -a
sed -i '/ swap / s/^/#/' /etc/fstab

echo "=== Loading required kernel modules ==="
cat <<EOF > /etc/modules-load.d/k8s.conf
overlay
br_netfilter
EOF

modprobe overlay
modprobe br_netfilter

echo "=== Configuring sysctl for Kubernetes networking ==="
cat <<EOF > /etc/sysctl.d/k8s.conf
net.bridge.bridge-nf-call-iptables  = 1
net.bridge.bridge-nf-call-ip6tables = 1
net.ipv4.ip_forward                 = 1
EOF

sysctl --system

echo "=== Installing containerd ==="
apt-get install -y apt-transport-https ca-certificates curl gpg containerd

mkdir -p /etc/containerd
containerd config default > /etc/containerd/config.toml
sed -i 's/SystemdCgroup = false/SystemdCgroup = true/' /etc/containerd/config.toml
systemctl restart containerd
systemctl enable containerd

echo "=== Installing kubeadm, kubelet, kubectl ==="
mkdir -p /etc/apt/keyrings
curl -fsSL https://pkgs.k8s.io/core:/stable:/v1.31/deb/Release.key | gpg --dearmor -o /etc/apt/keyrings/kubernetes-apt-keyring.gpg
echo 'deb [signed-by=/etc/apt/keyrings/kubernetes-apt-keyring.gpg] https://pkgs.k8s.io/core:/stable:/v1.31/deb/ /' > /etc/apt/sources.list.d/kubernetes.list

apt-get update
apt-get install -y kubelet kubeadm kubectl
apt-mark hold kubelet kubeadm kubectl

systemctl enable kubelet

echo "=== Installing useful tools ==="
apt-get install -y bash-completion net-tools

echo "=== Enabling kubectl bash completion ==="
kubectl completion bash > /etc/bash_completion.d/kubectl

echo ""
echo "============================================"
echo " Node '$HOSTNAME' setup complete!"
echo " Reboot recommended: sudo reboot"
echo "============================================"
