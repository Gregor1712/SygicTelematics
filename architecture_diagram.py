import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
from matplotlib.patches import FancyBboxPatch, FancyArrowPatch
import matplotlib.patheffects as pe

fig, ax = plt.subplots(1, 1, figsize=(20, 14))
fig.patch.set_facecolor('#0f1729')
ax.set_xlim(0, 20)
ax.set_ylim(0, 14)
ax.set_aspect('equal')
ax.axis('off')

# Colors
C_BG = '#0f1729'
C_CLIENT = '#6366f1'      # indigo
C_GATEWAY = '#0ea5e9'     # sky blue
C_GRAPHQL = '#e91e8c'     # pink/magenta
C_REST = '#22c55e'        # green
C_GRPC = '#f59e0b'        # amber
C_SERVICE = '#8b5cf6'     # purple
C_DB = '#06b6d4'          # cyan
C_MQ = '#ef4444'          # red
C_YARP = '#14b8a6'        # teal
C_TEXT = '#f8fafc'
C_SUBTEXT = '#94a3b8'
C_BORDER = '#334155'

def draw_box(x, y, w, h, color, label, sublabel=None, radius=0.15, alpha=0.9, fontsize=10, icon=None):
    box = FancyBboxPatch((x, y), w, h, boxstyle=f"round,pad={radius}",
                         facecolor=color, edgecolor='white', linewidth=1.5, alpha=alpha, zorder=3)
    ax.add_patch(box)
    if icon:
        ax.text(x + w/2, y + h/2 + (0.15 if sublabel else 0), f"{icon}\n{label}",
                ha='center', va='center', fontsize=fontsize, fontweight='bold',
                color='white', zorder=4)
    else:
        ax.text(x + w/2, y + h/2 + (0.12 if sublabel else 0), label,
                ha='center', va='center', fontsize=fontsize, fontweight='bold',
                color='white', zorder=4)
    if sublabel:
        ax.text(x + w/2, y + h/2 - 0.22, sublabel,
                ha='center', va='center', fontsize=7, color='#e2e8f0', zorder=4, style='italic')

def draw_arrow(x1, y1, x2, y2, color, label=None, style='->', lw=2):
    ax.annotate('', xy=(x2, y2), xytext=(x1, y1),
                arrowprops=dict(arrowstyle=style, color=color, lw=lw, connectionstyle='arc3,rad=0'),
                zorder=2)
    if label:
        mx, my = (x1+x2)/2, (y1+y2)/2
        ax.text(mx, my + 0.2, label, ha='center', va='center', fontsize=7,
                color=color, fontweight='bold', zorder=5,
                bbox=dict(boxstyle='round,pad=0.15', facecolor=C_BG, edgecolor=color, alpha=0.9, linewidth=0.8))

def draw_db(x, y, label):
    from matplotlib.patches import Ellipse
    w, h = 1.2, 0.6
    # body
    rect = plt.Rectangle((x - w/2, y - 0.3), w, h, facecolor='#1e3a5f', edgecolor=C_DB, linewidth=1.5, zorder=3)
    ax.add_patch(rect)
    # top ellipse
    ell_top = Ellipse((x, y + 0.3), w, 0.3, facecolor='#1e3a5f', edgecolor=C_DB, linewidth=1.5, zorder=4)
    ax.add_patch(ell_top)
    # bottom ellipse
    ell_bot = Ellipse((x, y - 0.3), w, 0.3, facecolor='#1e3a5f', edgecolor=C_DB, linewidth=1.5, zorder=3)
    ax.add_patch(ell_bot)
    ax.text(x, y, label, ha='center', va='center', fontsize=6, color=C_DB, fontweight='bold', zorder=5)

# === TITLE ===
ax.text(10, 13.5, 'SYGIC TELEMATICS — MICROSERVICE ARCHITECTURE', ha='center', va='center',
        fontsize=18, fontweight='bold', color=C_TEXT, zorder=5,
        path_effects=[pe.withStroke(linewidth=3, foreground=C_BG)])
ax.text(10, 13.0, '.NET 8  |  gRPC  |  GraphQL  |  YARP  |  RabbitMQ  |  MassTransit',
        ha='center', va='center', fontsize=9, color=C_SUBTEXT, zorder=5)

# === CLIENT LAYER ===
draw_box(8, 11.2, 4, 1.0, C_CLIENT, 'Angular Client', 'http://localhost:4200', fontsize=12)

# === GATEWAY LAYER — outer container ===
gw_box = FancyBboxPatch((2.5, 7.0), 15, 3.2, boxstyle="round,pad=0.2",
                         facecolor='#1a2744', edgecolor=C_GATEWAY, linewidth=2, alpha=0.7, zorder=1, linestyle='--')
ax.add_patch(gw_box)
ax.text(10, 9.9, 'API GATEWAY  (localhost:5100 / 7100)', ha='center', va='center',
        fontsize=13, fontweight='bold', color=C_GATEWAY, zorder=5)

# Gateway sub-components
draw_box(3.0, 7.5, 3.0, 1.5, '#1e293b', 'GraphQL', '/graphql (HotChocolate)', fontsize=10)
# pink border for graphql
gql_border = FancyBboxPatch((3.0, 7.5), 3.0, 1.5, boxstyle="round,pad=0.15",
                             facecolor='none', edgecolor=C_GRAPHQL, linewidth=2, zorder=3)
ax.add_patch(gql_border)

draw_box(6.5, 7.5, 3.5, 1.5, '#1e293b', 'BFF Controllers', 'REST /api/bff/*', fontsize=10)
bff_border = FancyBboxPatch((6.5, 7.5), 3.5, 1.5, boxstyle="round,pad=0.15",
                             facecolor='none', edgecolor=C_REST, linewidth=2, zorder=3)
ax.add_patch(bff_border)

draw_box(10.5, 7.5, 3.0, 1.5, '#1e293b', 'YARP Proxy', 'Reverse Proxy', fontsize=10)
yarp_border = FancyBboxPatch((10.5, 7.5), 3.0, 1.5, boxstyle="round,pad=0.15",
                              facecolor='none', edgecolor=C_YARP, linewidth=2, zorder=3)
ax.add_patch(yarp_border)

draw_box(14.0, 7.5, 3.0, 1.5, '#1e293b', 'Auth (JWT)', 'Bearer Token', fontsize=10)
auth_border = FancyBboxPatch((14.0, 7.5), 3.0, 1.5, boxstyle="round,pad=0.15",
                              facecolor='none', edgecolor='#a78bfa', linewidth=2, zorder=3)
ax.add_patch(auth_border)

# === Arrows: Client -> Gateway ===
draw_arrow(9.2, 11.2, 4.5, 9.0, C_GRAPHQL, 'GraphQL', lw=2.5)
draw_arrow(10, 11.2, 8.25, 9.0, C_REST, 'REST', lw=2.5)
draw_arrow(10.8, 11.2, 12.0, 9.0, C_YARP, 'Proxy', lw=2.5)

# === MICROSERVICES LAYER ===
services = [
    ('Identity', '5101', 0),
    ('Vehicle', '5103', 1),
    ('Location', '5104', 2),
    ('Battery', '5105', 3),
    ('Trip', '5106', 4),
    ('Telemetry', '5107', 5),
    ('Alert', '5108', 6),
]

svc_y = 4.0
svc_h = 1.3
svc_w = 2.0
start_x = 1.0
gap = 2.6

for name, port, i in services:
    x = start_x + i * gap
    draw_box(x, svc_y, svc_w, svc_h, C_SERVICE, name, f':{port}', fontsize=9)
    # DB below each service
    draw_db(x + svc_w/2, svc_y - 1.0, f'{name}DB')

# === Arrows: Gateway -> Services (gRPC) ===
for name, port, i in services[1:]:  # Skip Identity (uses HTTP)
    x = start_x + i * gap + svc_w/2
    draw_arrow(4.5 + (i-1)*1.5, 7.5, x, svc_y + svc_h, C_GRPC, lw=1.5)

# Identity uses HTTP
draw_arrow(8.25, 7.5, start_x + svc_w/2, svc_y + svc_h, C_REST, lw=1.5)

# gRPC label
ax.text(5.5, 6.4, 'gRPC (HTTP/2)', ha='center', va='center', fontsize=9,
        color=C_GRPC, fontweight='bold', zorder=5,
        bbox=dict(boxstyle='round,pad=0.2', facecolor=C_BG, edgecolor=C_GRPC, alpha=0.9, linewidth=1))

# HTTP label for Identity
ax.text(2.5, 6.4, 'HTTP', ha='center', va='center', fontsize=8,
        color=C_REST, fontweight='bold', zorder=5,
        bbox=dict(boxstyle='round,pad=0.15', facecolor=C_BG, edgecolor=C_REST, alpha=0.9, linewidth=1))

# === MESSAGE BUS ===
mq_y = 1.5
mq_box = FancyBboxPatch((3.0, mq_y), 14, 0.8, boxstyle="round,pad=0.15",
                          facecolor='#2d1215', edgecolor=C_MQ, linewidth=2, alpha=0.8, zorder=3, linestyle='--')
ax.add_patch(mq_box)
ax.text(10, mq_y + 0.4, 'RabbitMQ  +  MassTransit  (Async Event Bus)', ha='center', va='center',
        fontsize=11, fontweight='bold', color=C_MQ, zorder=5)

# Arrows from services to message bus
for name, port, i in services[1:]:
    x = start_x + i * gap + svc_w/2
    ax.annotate('', xy=(x, mq_y + 0.8), xytext=(x, svc_y - 1.4),
                arrowprops=dict(arrowstyle='<->', color=C_MQ, lw=1, alpha=0.5, linestyle='dashed'),
                zorder=2)

# === LEGEND ===
legend_items = [
    (C_GRAPHQL, 'GraphQL (HotChocolate)'),
    (C_REST, 'REST / HTTP'),
    (C_GRPC, 'gRPC (HTTP/2 + Protobuf)'),
    (C_YARP, 'YARP Reverse Proxy'),
    (C_MQ, 'RabbitMQ Events'),
    (C_SERVICE, 'Microservice'),
    (C_DB, 'PostgreSQL Database'),
]

legend_x = 15.5
legend_y = 12.8
ax.text(legend_x + 1.2, legend_y + 0.3, 'LEGEND', ha='center', va='center',
        fontsize=9, fontweight='bold', color=C_SUBTEXT, zorder=5)
for idx, (color, label) in enumerate(legend_items):
    y = legend_y - idx * 0.32
    circle = plt.Circle((legend_x, y), 0.1, color=color, zorder=5)
    ax.add_patch(circle)
    ax.text(legend_x + 0.25, y, label, ha='left', va='center', fontsize=7.5, color=C_TEXT, zorder=5)

plt.tight_layout(pad=0.5)
plt.savefig('C:/Users/grego/RiderProjects/SygicTelematics/architecture.pdf',
            format='pdf', dpi=300, facecolor=C_BG, edgecolor='none', bbox_inches='tight')
plt.savefig('C:/Users/grego/RiderProjects/SygicTelematics/architecture.png',
            format='png', dpi=200, facecolor=C_BG, edgecolor='none', bbox_inches='tight')
print("Done! Saved architecture.pdf and architecture.png")
