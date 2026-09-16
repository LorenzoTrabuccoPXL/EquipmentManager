<template>
  <div class="detail">
    <div class="detail__header">
      <button class="back-btn" @click="$emit('back')">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polyline points="15 18 9 12 15 6"/>
        </svg>
        Back to Dashboard
      </button>
      <div class="detail__header-actions">
        <button class="btn btn--secondary" @click="$emit('edit', equipment)">
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
            <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/>
          </svg>
          Edit
        </button>
        <button class="btn btn--danger" @click="$emit('delete', equipment)">
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="3 6 5 6 21 6"/>
            <path d="M19 6l-1 14a2 2 0 01-2 2H8a2 2 0 01-2-2L5 6"/>
          </svg>
          Delete
        </button>
      </div>
    </div>

    <div class="detail__hero">
      <div class="detail__icon">
        <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M12 2H8a2 2 0 00-2 2v16a2 2 0 002 2h8a2 2 0 002-2V8l-6-6z"/>
          <polyline points="12 2 12 8 18 8"/>
        </svg>
      </div>
      <div class="detail__hero-info">
        <h1 class="detail__name">{{ equipment.name }}</h1>
        <p class="detail__model">{{ equipment.model }} · {{ equipment.serialNumber }}</p>
        <div class="detail__meta">
          <StatusBadge :status="equipment.status" />
          <span class="detail__category">{{ categoryName }}</span>
        </div>
      </div>
    </div>

    <div class="detail__grid">
      <div class="info-card">
        <h3 class="info-card__title">Details</h3>
        <dl class="info-list">
          <div class="info-row"><dt>Location</dt><dd>{{ locationName }}</dd></div>
          <div class="info-row"><dt>Serial Number</dt><dd>{{ equipment.serialNumber }}</dd></div>
          <div class="info-row"><dt>Purchase Price</dt><dd>{{ formatCurrency(equipment.purchasePrice) }}</dd></div>
          <div class="info-row"><dt>Warranty Expiry</dt>
            <dd :class="isExpired(equipment.warrantyExpiry) && 'text-danger'">
              {{ formatDate(equipment.warrantyExpiry) }}
              <span v-if="isExpired(equipment.warrantyExpiry)" class="expired-tag">Expired</span>
            </dd>
          </div>
        </dl>
      </div>

      <div class="info-card">
        <h3 class="info-card__title">Maintenance Schedule</h3>
        <dl class="info-list">
          <div class="info-row"><dt>Last Service</dt><dd>{{ formatDate(equipment.lastMaintenance) }}</dd></div>
          <div class="info-row">
            <dt>Next Service</dt>
            <dd :class="isOverdue(equipment.nextMaintenance) && 'text-danger'">
              {{ formatDate(equipment.nextMaintenance) }}
              <span v-if="isOverdue(equipment.nextMaintenance)" class="overdue-tag">Overdue</span>
            </dd>
          </div>
        </dl>
        <p class="info-desc">{{ equipment.description }}</p>
      </div>
    </div>

    <div class="history-card">
      <h3 class="history-card__title">Maintenance History</h3>
      <div v-if="!maintenanceRecords.length" class="history-empty">
        No maintenance records found.
      </div>
      <div v-else class="history-list">
        <div v-for="record in maintenanceRecords" :key="record.maintenanceRecordId" class="history-item">
          <div class="history-item__dot" :class="`dot--${typeClass(record.maintenanceType)}`" />
          <div class="history-item__body">
            <div class="history-item__header">
              <span class="history-item__type" :class="`type--${typeClass(record.maintenanceType)}`">{{ record.maintenanceType }}</span>
              <span class="history-item__date">{{ formatDate(record.maintenanceDate) }}</span>
              <span class="history-item__tech">{{ record.technicianName }}</span>
            </div>
            <p class="history-item__notes">{{ record.notes }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import StatusBadge from './StatusBadge.vue'

defineProps({
  equipment: { type: Object, required: true },
  categoryName: { type: String, default: 'Unknown category' },
  locationName: { type: String, default: 'Unknown location' },
  maintenanceRecords: { type: Array, default: () => [] },
})
defineEmits(['back', 'edit', 'delete'])

function formatDate(d) {
  if (!d) return '—'
  return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'long', year: 'numeric' })
}

function formatCurrency(n) {
  if (!n) return '—'
  return new Intl.NumberFormat('en-GB', { style: 'currency', currency: 'GBP', maximumFractionDigits: 0 }).format(n)
}

function isOverdue(d) { return d && new Date(d) < new Date() }
function isExpired(d) { return d && new Date(d) < new Date() }
function typeClass(value) { return String(value || '').toLowerCase() }
</script>

<style scoped>
.detail { display: flex; flex-direction: column; gap: 20px; }

.detail__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
}

.back-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  background: none;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 7px 14px;
  font-size: 13px;
  color: var(--color-text-muted);
  cursor: pointer;
  transition: all .15s;
}
.back-btn:hover { border-color: var(--color-primary); color: var(--color-primary); }

.detail__header-actions { display: flex; gap: 8px; }

.btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  border-radius: var(--radius-sm);
  padding: 7px 14px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all .15s;
}
.btn--secondary {
  background: none;
  border: 1px solid var(--color-border);
  color: var(--color-text-muted);
}
.btn--secondary:hover { border-color: var(--color-warning); color: var(--color-warning); background: var(--color-warning-bg); }
.btn--danger {
  background: none;
  border: 1px solid var(--color-border);
  color: var(--color-text-muted);
}
.btn--danger:hover { border-color: var(--color-danger); color: var(--color-danger); background: var(--color-danger-bg); }

.detail__hero {
  background: var(--color-card);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 24px;
  display: flex;
  align-items: flex-start;
  gap: 18px;
  box-shadow: var(--shadow-sm);
}

.detail__icon {
  width: 60px;
  height: 60px;
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.detail__hero-info { flex: 1; }
.detail__name { font-size: 20px; font-weight: 700; margin-bottom: 4px; }
.detail__model { color: var(--color-text-muted); font-size: 13px; margin-bottom: 10px; }
.detail__meta { display: flex; align-items: center; gap: 8px; }
.detail__category {
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-radius: 4px;
  padding: 2px 8px;
  font-size: 12px;
  font-weight: 500;
}

.detail__grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
@media (max-width: 700px) { .detail__grid { grid-template-columns: 1fr; } }

.info-card {
  background: var(--color-card);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 20px;
  box-shadow: var(--shadow-sm);
}
.info-card__title {
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 14px;
  color: var(--color-text);
}

.info-list { display: flex; flex-direction: column; gap: 10px; }
.info-row {
  display: flex;
  gap: 12px;
  font-size: 13.5px;
}
.info-row dt { width: 130px; flex-shrink: 0; color: var(--color-text-muted); }
.info-row dd { font-weight: 500; }

.text-danger { color: var(--color-danger) !important; }

.expired-tag, .overdue-tag {
  display: inline-block;
  font-size: 10px;
  font-weight: 700;
  padding: 1px 5px;
  border-radius: 3px;
  margin-left: 4px;
}
.expired-tag { background: var(--color-danger-bg); color: var(--color-danger); }
.overdue-tag { background: var(--color-danger-bg); color: var(--color-danger); }

.info-desc {
  margin-top: 14px;
  padding-top: 14px;
  border-top: 1px solid var(--color-border);
  font-size: 13.5px;
  color: var(--color-text-muted);
  line-height: 1.6;
}

.history-card {
  background: var(--color-card);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 20px;
  box-shadow: var(--shadow-sm);
}
.history-card__title { font-size: 14px; font-weight: 600; margin-bottom: 16px; }
.history-empty { color: var(--color-text-muted); font-size: 13px; padding: 16px 0; text-align: center; }

.history-list { display: flex; flex-direction: column; gap: 0; }
.history-item {
  display: flex;
  gap: 14px;
  padding: 14px 0;
  border-bottom: 1px solid var(--color-border);
  position: relative;
}
.history-item:last-child { border-bottom: none; }

.history-item__dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  flex-shrink: 0;
  margin-top: 4px;
}
.dot--preventive { background: var(--color-success); }
.dot--corrective  { background: var(--color-danger); }

.history-item__body { flex: 1; }
.history-item__header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 6px;
  flex-wrap: wrap;
}

.history-item__type {
  display: inline-block;
  font-size: 11px;
  font-weight: 700;
  padding: 2px 7px;
  border-radius: 4px;
  text-transform: uppercase;
  letter-spacing: .4px;
}
.type--preventive { background: var(--color-success-bg); color: var(--color-success); }
.type--corrective  { background: var(--color-danger-bg);  color: var(--color-danger); }

.history-item__date { font-size: 13px; font-weight: 500; }
.history-item__tech { font-size: 13px; color: var(--color-text-muted); margin-left: auto; }
.history-item__notes { font-size: 13.5px; color: var(--color-text-muted); line-height: 1.55; }
</style>
