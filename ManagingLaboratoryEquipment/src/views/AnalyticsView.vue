<template>
  <div class="layout">
    <AppSidebar v-model="sidebarOpen" />
    <div class="layout__content">
      <TopNavigation @toggle-sidebar="sidebarOpen = !sidebarOpen" />
      <main class="main">

        <!-- Header -->
        <div class="page-header">
          <div>
            <h1 class="page-title">Analytics</h1>
            <p class="page-sub">Equipment and maintenance overview</p>
          </div>
          <div class="page-header__actions">
            <select v-model="categoryFilter" class="filter-select">
              <option value="">All Categories</option>
              <option v-for="cat in categories" :key="cat.categoryId" :value="cat.categoryId">{{ cat.name }}</option>
            </select>
          </div>
        </div>

        <!-- Loading -->
        <div v-if="loading" class="state-center">
          <div class="spinner" />
          <p>Loading analytics…</p>
        </div>

        <!-- Error -->
        <div v-else-if="error" class="state-center state-center--error">
          <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>
          </svg>
          <p>{{ error }}</p>
          <button class="btn btn--primary" @click="loadAnalytics">Retry</button>
        </div>

        <!-- Empty -->
        <div v-else-if="filteredItems.length === 0" class="state-center">
          <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color:#94a3b8">
            <path d="M12 2H8a2 2 0 00-2 2v16a2 2 0 002 2h8a2 2 0 002-2V8l-6-6z"/>
            <polyline points="12 2 12 8 18 8"/>
          </svg>
          <p>No equipment in this category.</p>
          <button class="btn btn--secondary" @click="categoryFilter = ''">Clear filter</button>
        </div>

        <template v-else>
          <!-- Summary cards -->
          <div class="summary-grid">
            <SummaryCard :value="filteredItems.length" label="Total Equipment" variant="default">
              <template #icon>
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M12 2H8a2 2 0 00-2 2v16a2 2 0 002 2h8a2 2 0 002-2V8l-6-6z"/>
                  <polyline points="12 2 12 8 18 8"/>
                </svg>
              </template>
            </SummaryCard>
            <SummaryCard :value="filteredActive" label="Active" variant="success">
              <template #icon>
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M22 11.08V12a10 10 0 11-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/>
                </svg>
              </template>
            </SummaryCard>
            <SummaryCard :value="filteredOverdue" label="Overdue Maintenance" variant="danger">
              <template #icon>
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                </svg>
              </template>
            </SummaryCard>
            <SummaryCard :value="filteredUpcoming" label="Due Within 7 Days" variant="warning">
              <template #icon>
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                  <line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/>
                  <line x1="3" y1="10" x2="21" y2="10"/>
                </svg>
              </template>
            </SummaryCard>
          </div>

          <div class="analytics-grid">
            <!-- Status breakdown -->
            <div class="card">
              <h2 class="card__title">Equipment by Status</h2>
              <div class="status-chart">
                <div
                  v-for="s in statusBreakdown" :key="s.status"
                  class="status-bar-row"
                >
                  <span class="status-bar-row__label">{{ s.label }}</span>
                  <div class="status-bar-row__track">
                    <div
                      class="status-bar-row__fill"
                      :class="`fill--${s.status}`"
                      :style="{ width: filteredItems.length ? (s.count / filteredItems.length * 100) + '%' : '0%' }"
                    />
                  </div>
                  <span class="status-bar-row__count">{{ s.count }}</span>
                </div>
              </div>
              <!-- Mini donut -->
              <div class="donut-wrap">
                <svg viewBox="0 0 80 80" width="110" height="110">
                  <template v-for="(seg, i) in donutSegments" :key="i">
                    <circle
                      cx="40" cy="40" r="28"
                      fill="none"
                      :stroke="seg.color"
                      stroke-width="12"
                      :stroke-dasharray="`${seg.dash} ${seg.gap}`"
                      :stroke-dashoffset="seg.offset"
                      transform="rotate(-90 40 40)"
                    />
                  </template>
                  <text x="40" y="44" text-anchor="middle" font-size="13" font-weight="700" fill="#1e293b">
                    {{ filteredItems.length }}
                  </text>
                </svg>
                <div class="donut-legend">
                  <div v-for="s in statusBreakdown" :key="s.status" class="donut-legend__item">
                    <span class="donut-legend__dot" :style="{ background: s.color }" />
                    <span>{{ s.label }} ({{ s.count }})</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Category breakdown -->
            <div class="card">
              <h2 class="card__title">Equipment by Category</h2>
              <div class="status-chart">
                <div
                  v-for="cat in categoryBreakdown" :key="cat.category"
                  class="status-bar-row"
                >
                  <span class="status-bar-row__label">{{ cat.category }}</span>
                  <div class="status-bar-row__track">
                    <div
                      class="status-bar-row__fill fill--primary"
                      :style="{ width: filteredItems.length ? (cat.count / filteredItems.length * 100) + '%' : '0%' }"
                    />
                  </div>
                  <span class="status-bar-row__count">{{ cat.count }}</span>
                </div>
              </div>
            </div>

            <!-- Monthly maintenance chart -->
            <div class="card card--wide">
              <h2 class="card__title">Monthly Maintenance Activity</h2>
              <p class="card__sub">Last 6 months · all categories</p>
              <div class="bar-chart">
                <div
                  v-for="m in monthlyActivity" :key="m.key"
                  class="bar-chart__col"
                >
                  <span class="bar-chart__value">{{ m.count }}</span>
                  <div class="bar-chart__track">
                    <div
                      class="bar-chart__fill"
                      :style="{ height: maxMonthly ? (m.count / maxMonthly * 100) + '%' : '0%' }"
                    />
                  </div>
                  <span class="bar-chart__label">{{ m.label }}</span>
                </div>
              </div>
            </div>

            <!-- Overdue maintenance -->
            <div class="card">
              <h2 class="card__title">Overdue Maintenance</h2>
              <div v-if="filteredOverdueItems.length === 0" class="card-empty">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                  <path d="M22 11.08V12a10 10 0 11-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/>
                </svg>
                All maintenance is up to date
              </div>
              <ul v-else class="alert-list">
                <li v-for="eq in filteredOverdueItems" :key="eq.equipmentId" class="alert-item alert-item--danger">
                  <RouterLink :to="`/equipment/${eq.equipmentId}`" class="alert-item__link">
                    <span class="alert-item__name">{{ eq.name }}</span>
                    <span class="alert-item__detail">Due: {{ formatDate(eq.nextMaintenance) }}</span>
                  </RouterLink>
                </li>
              </ul>
            </div>

            <!-- Upcoming maintenance -->
            <div class="card">
              <h2 class="card__title">Due Within 7 Days</h2>
              <div v-if="filteredUpcomingItems.length === 0" class="card-empty">
                No maintenance due in the next 7 days
              </div>
              <ul v-else class="alert-list">
                <li v-for="eq in filteredUpcomingItems" :key="eq.equipmentId" class="alert-item alert-item--warning">
                  <RouterLink :to="`/equipment/${eq.equipmentId}`" class="alert-item__link">
                    <span class="alert-item__name">{{ eq.name }}</span>
                    <span class="alert-item__detail">{{ daysUntil(eq.nextMaintenance) }} days — {{ formatDate(eq.nextMaintenance) }}</span>
                  </RouterLink>
                </li>
              </ul>
            </div>

            <!-- Recent activity -->
            <div class="card card--wide">
              <h2 class="card__title">Recent Maintenance Activity</h2>
              <div v-if="recentActivity.length === 0" class="card-empty">No maintenance records found.</div>
              <ul v-else class="activity-list">
                <li v-for="r in recentActivity" :key="r.maintenanceRecordId" class="activity-item">
                  <span class="activity-item__dot" :class="r.maintenanceType === 'Corrective' ? 'dot--danger' : 'dot--success'" />
                  <div class="activity-item__body">
                    <span class="activity-item__title">{{ r.equipmentName }}</span>
                    <span class="activity-item__type" :class="r.maintenanceType === 'Corrective' ? 'type--corrective' : 'type--preventive'">{{ r.maintenanceType }}</span>
                  </div>
                  <div class="activity-item__meta">
                    <span>{{ r.technicianName }}</span>
                    <span>{{ formatDate(r.maintenanceDate) }}</span>
                  </div>
                </li>
              </ul>
            </div>
          </div>
        </template>
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import AppSidebar from '../components/AppSidebar.vue'
import TopNavigation from '../components/TopNavigation.vue'
import SummaryCard from '../components/SummaryCard.vue'
import { useEquipmentStore } from '../stores/equipmentStore.js'
import { useMaintenanceStore } from '../stores/maintenanceStore.js'
import { useReferenceDataStore } from '../stores/referenceDataStore.js'

const equipStore  = useEquipmentStore()
const maintStore  = useMaintenanceStore()
const referenceStore = useReferenceDataStore()

const sidebarOpen    = ref(false)
const categoryFilter = ref('')
const categories = computed(() => referenceStore.categories)
const loading = computed(() => equipStore.isLoading || maintStore.isLoading || referenceStore.isLoading)
const error = computed(() => equipStore.error || maintStore.error || referenceStore.error)

async function loadAnalytics() {
  await Promise.allSettled([
    referenceStore.fetchAll(),
    equipStore.fetchAll(),
    maintStore.fetchAll(),
  ])
}

onMounted(loadAnalytics)

const now = new Date()
const weekLater = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000)

const filteredItems = computed(() => {
  if (!categoryFilter.value) return equipStore.items
  return equipStore.items.filter(e => e.categoryId === Number(categoryFilter.value))
})

const normalizedStatus = value => String(value || '').toLowerCase()
const filteredActive   = computed(() => filteredItems.value.filter(e => normalizedStatus(e.status) === 'active').length)
const filteredOverdue  = computed(() => filteredOverdueItems.value.length)
const filteredUpcoming = computed(() => filteredUpcomingItems.value.length)

const filteredOverdueItems = computed(() =>
  filteredItems.value.filter(e => e.nextMaintenance && new Date(e.nextMaintenance) < now)
)
const filteredUpcomingItems = computed(() =>
  filteredItems.value.filter(e => {
    if (!e.nextMaintenance) return false
    const d = new Date(e.nextMaintenance)
    return d >= now && d <= weekLater
  })
)

const statusBreakdown = computed(() => [
  { status: 'active',      label: 'Active',      color: '#16a34a', count: filteredItems.value.filter(e => normalizedStatus(e.status) === 'active').length },
  { status: 'maintenance', label: 'Maintenance', color: '#d97706', count: filteredItems.value.filter(e => normalizedStatus(e.status) === 'maintenance').length },
  { status: 'offline',     label: 'Offline',     color: '#dc2626', count: filteredItems.value.filter(e => normalizedStatus(e.status) === 'offline').length },
])

const categoryBreakdown = computed(() => {
  return categories.value.map(cat => ({
    category: cat.name,
    count: filteredItems.value.filter(e => e.categoryId === cat.categoryId).length,
  })).filter(c => c.count > 0)
})

// Donut chart
const circumference = 2 * Math.PI * 28
const donutSegments = computed(() => {
  const total = filteredItems.value.length
  if (!total) return []
  let offset = 0
  return statusBreakdown.value.map(s => {
    const frac = s.count / total
    const dash = frac * circumference
    const seg = { color: s.color, dash, gap: circumference - dash, offset: -offset }
    offset += dash
    return seg
  })
})

const monthlyActivity = computed(() => maintStore.last6MonthsActivity)
const maxMonthly = computed(() => Math.max(1, ...monthlyActivity.value.map(m => m.count)))

const recentActivity = computed(() =>
  maintStore.displayRecords.slice(0, 8)
)

function formatDate(d) {
  if (!d) return '—'
  return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
}
function daysUntil(d) {
  return Math.ceil((new Date(d) - now) / 86400000)
}
</script>

<style scoped>
.layout { display: flex; min-height: 100%; }
.layout__content { flex: 1; margin-left: var(--sidebar-width); display: flex; flex-direction: column; min-height: 100%; }
@media (max-width: 899px) { .layout__content { margin-left: 0; } }
.main { padding: 20px; padding-top: calc(var(--topnav-height) + 20px); display: flex; flex-direction: column; gap: 20px; }

.page-header { display: flex; align-items: flex-start; justify-content: space-between; gap: 12px; flex-wrap: wrap; }
.page-title  { font-size: 20px; font-weight: 700; }
.page-sub    { font-size: 13px; color: var(--color-text-muted); margin-top: 2px; }
.page-header__actions { display: flex; gap: 8px; }

.filter-select {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 6px 10px;
  font-size: 13px;
  background: white;
  outline: none;
  cursor: pointer;
}
.filter-select:focus { border-color: var(--color-primary); }

.state-center { display: flex; flex-direction: column; align-items: center; gap: 12px; padding: 60px 20px; color: var(--color-text-muted); }
.state-center--error { color: var(--color-danger); }
.spinner { width: 32px; height: 32px; border: 3px solid var(--color-border); border-top-color: var(--color-primary); border-radius: 50%; animation: spin .7s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }

.summary-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
@media (max-width: 1100px) { .summary-grid { grid-template-columns: repeat(2, 1fr); } }
@media (max-width: 560px)  { .summary-grid { grid-template-columns: 1fr; } }

.analytics-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
@media (max-width: 900px) { .analytics-grid { grid-template-columns: 1fr; } }

.card {
  background: white;
  border-radius: var(--radius-md);
  border: 1px solid var(--color-border);
  padding: 20px;
  box-shadow: var(--shadow-sm);
}
.card--wide { grid-column: 1 / -1; }
.card__title { font-size: 14px; font-weight: 600; margin-bottom: 16px; }
.card__sub { font-size: 12px; color: var(--color-text-muted); margin-top: -12px; margin-bottom: 16px; }

/* Status/category bars */
.status-chart { display: flex; flex-direction: column; gap: 10px; margin-bottom: 16px; }
.status-bar-row { display: flex; align-items: center; gap: 10px; }
.status-bar-row__label { width: 90px; font-size: 12.5px; color: var(--color-text-muted); flex-shrink: 0; }
.status-bar-row__track { flex: 1; height: 8px; background: var(--color-bg); border-radius: 4px; overflow: hidden; }
.status-bar-row__fill  { height: 100%; border-radius: 4px; transition: width .4s ease; }
.fill--active      { background: #16a34a; }
.fill--maintenance { background: #d97706; }
.fill--offline     { background: #dc2626; }
.fill--primary     { background: var(--color-primary); }
.status-bar-row__count { width: 24px; text-align: right; font-size: 12.5px; font-weight: 600; color: var(--color-text-muted); }

/* Donut */
.donut-wrap { display: flex; align-items: center; gap: 20px; }
.donut-legend { display: flex; flex-direction: column; gap: 8px; }
.donut-legend__item { display: flex; align-items: center; gap: 6px; font-size: 12.5px; color: var(--color-text-muted); }
.donut-legend__dot  { width: 10px; height: 10px; border-radius: 50%; flex-shrink: 0; }

/* Bar chart */
.bar-chart {
  display: flex;
  align-items: flex-end;
  gap: 12px;
  height: 140px;
  padding-top: 20px;
}
.bar-chart__col { flex: 1; display: flex; flex-direction: column; align-items: center; gap: 4px; height: 100%; }
.bar-chart__value { font-size: 11px; font-weight: 600; color: var(--color-text-muted); flex-shrink: 0; }
.bar-chart__track { flex: 1; width: 100%; display: flex; align-items: flex-end; }
.bar-chart__fill  { width: 100%; background: var(--color-primary); border-radius: 4px 4px 0 0; min-height: 4px; transition: height .4s ease; opacity: .85; }
.bar-chart__label { font-size: 11px; color: var(--color-text-muted); flex-shrink: 0; }

/* Alert list */
.card-empty { display: flex; align-items: center; gap: 8px; color: var(--color-text-muted); font-size: 13px; padding: 8px 0; }
.alert-list { list-style: none; display: flex; flex-direction: column; gap: 6px; }
.alert-item { border-radius: var(--radius-sm); }
.alert-item__link { display: flex; justify-content: space-between; align-items: center; padding: 8px 12px; border-radius: var(--radius-sm); text-decoration: none; transition: background .1s; }
.alert-item--danger .alert-item__link  { background: var(--color-danger-bg);  }
.alert-item--warning .alert-item__link { background: var(--color-warning-bg); }
.alert-item__link:hover { filter: brightness(.96); }
.alert-item__name   { font-size: 13px; font-weight: 500; color: var(--color-text); }
.alert-item--danger .alert-item__detail  { font-size: 12px; color: var(--color-danger); }
.alert-item--warning .alert-item__detail { font-size: 12px; color: var(--color-warning); }

/* Activity list */
.activity-list { list-style: none; display: flex; flex-direction: column; }
.activity-item {
  display: flex; align-items: center; gap: 12px;
  padding: 10px 0;
  border-bottom: 1px solid var(--color-border);
}
.activity-item:last-child { border-bottom: none; }
.activity-item__dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
.dot--success { background: var(--color-success); }
.dot--danger  { background: var(--color-danger); }
.activity-item__body { flex: 1; display: flex; flex-direction: column; gap: 2px; }
.activity-item__title { font-size: 13px; font-weight: 500; }
.activity-item__type  { display: inline-block; font-size: 10px; font-weight: 700; padding: 1px 6px; border-radius: 3px; text-transform: uppercase; width: fit-content; }
.type--preventive { background: var(--color-success-bg); color: var(--color-success); }
.type--corrective  { background: var(--color-danger-bg);  color: var(--color-danger); }
.activity-item__meta { display: flex; flex-direction: column; gap: 1px; text-align: right; font-size: 12px; color: var(--color-text-muted); }

.btn { border-radius: var(--radius-sm); padding: 7px 16px; font-size: 13px; font-weight: 500; cursor: pointer; border: none; transition: all .15s; }
.btn--primary { background: var(--color-primary); color: white; }
.btn--primary:hover { background: var(--color-primary-hover); }
.btn--secondary { background: white; color: var(--color-text-muted); border: 1px solid var(--color-border); }
.btn--secondary:hover { border-color: var(--color-primary); color: var(--color-primary); }
</style>
