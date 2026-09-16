<template>
  <div class="table-card">
    <div class="table-card__header">
      <h2 class="table-card__title">Equipment List</h2>
      <div class="table-card__filters">
        <select v-model="filterStatus" class="filter-select">
          <option value="">All Statuses</option>
          <option value="active">Active</option>
          <option value="maintenance">Maintenance</option>
          <option value="offline">Offline</option>
        </select>
        <select v-model="filterCategory" class="filter-select">
          <option value="">All Categories</option>
          <option v-for="cat in categories" :key="cat.categoryId" :value="cat.categoryId">{{ cat.name }}</option>
        </select>
        <button class="btn btn--primary" @click="$emit('add')">
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/>
          </svg>
          Add Equipment
        </button>
      </div>
    </div>

    <div class="table-wrapper">
      <div v-if="loading" class="table-state">
        <div class="spinner" />
        <p>Loading equipment…</p>
      </div>

      <div v-else-if="filtered.length === 0" class="table-state">
        <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color:#94a3b8">
          <path d="M12 2H8a2 2 0 00-2 2v16a2 2 0 002 2h8a2 2 0 002-2V8l-6-6z"/>
          <polyline points="12 2 12 8 18 8"/>
        </svg>
        <p>No equipment found matching your filters.</p>
        <button class="btn btn--secondary" @click="clearFilters">Clear filters</button>
      </div>

      <table v-else class="table">
        <thead>
          <tr>
            <th @click="sort('name')" class="sortable">
              Name <span class="sort-icon">{{ sortIcon('name') }}</span>
            </th>
            <th>Category</th>
            <th>Location</th>
            <th @click="sort('status')" class="sortable">
              Status <span class="sort-icon">{{ sortIcon('status') }}</span>
            </th>
            <th @click="sort('nextMaintenance')" class="sortable">
              Next Service <span class="sort-icon">{{ sortIcon('nextMaintenance') }}</span>
            </th>
            <th>Serial Number</th>
            <th class="th--actions">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in paginated" :key="item.equipmentId" class="table-row" @click="$emit('view', item)">
            <td>
              <div class="cell-name">
                <span class="cell-name__text">{{ item.name }}</span>
                <span class="cell-name__model">{{ item.model }}</span>
              </div>
            </td>
            <td><span class="category-tag">{{ item.categoryName }}</span></td>
            <td class="cell-muted">{{ item.locationName }}</td>
            <td><StatusBadge :status="item.status" /></td>
            <td :class="['cell-date', isOverdue(item.nextMaintenance) && 'cell-date--overdue']">
              {{ formatDate(item.nextMaintenance) }}
              <span v-if="isOverdue(item.nextMaintenance)" class="overdue-label">Overdue</span>
            </td>
            <td class="cell-muted">{{ item.serialNumber }}</td>
            <td class="td--actions" @click.stop>
              <button class="action-btn action-btn--view" @click="$emit('view', item)" title="View details">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>
                </svg>
              </button>
              <button class="action-btn action-btn--edit" @click="$emit('edit', item)" title="Edit">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
                  <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/>
                </svg>
              </button>
              <button class="action-btn action-btn--delete" @click="$emit('delete', item)" title="Delete">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <polyline points="3 6 5 6 21 6"/>
                  <path d="M19 6l-1 14a2 2 0 01-2 2H8a2 2 0 01-2-2L5 6"/>
                  <path d="M10 11v6M14 11v6"/>
                </svg>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="filtered.length > 0" class="table-footer">
      <span class="table-footer__count">
        Showing {{ (currentPage - 1) * pageSize + 1 }}–{{ Math.min(currentPage * pageSize, filtered.length) }} of {{ filtered.length }}
      </span>
      <div class="pagination">
        <button class="page-btn" :disabled="currentPage === 1" @click="currentPage--">
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="15 18 9 12 15 6"/>
          </svg>
        </button>
        <button
          v-for="p in totalPages" :key="p"
          class="page-btn"
          :class="{ 'page-btn--active': p === currentPage }"
          @click="currentPage = p"
        >{{ p }}</button>
        <button class="page-btn" :disabled="currentPage === totalPages" @click="currentPage++">
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="9 18 15 12 9 6"/>
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import StatusBadge from './StatusBadge.vue'

const props = defineProps({
  items:   { type: Array, required: true },
  loading: { type: Boolean, default: false },
  search:  { type: String, default: '' },
  categories: { type: Array, default: () => [] },
  pageSize: { type: Number, default: 5 },
})

defineEmits(['add', 'edit', 'view', 'delete'])

const filterStatus   = ref('')
const filterCategory = ref('')
const sortKey        = ref('name')
const sortAsc        = ref(true)
const currentPage    = ref(1)

watch([filterStatus, filterCategory, () => props.search], () => { currentPage.value = 1 })

function sort(key) {
  if (sortKey.value === key) sortAsc.value = !sortAsc.value
  else { sortKey.value = key; sortAsc.value = true }
}

function sortIcon(key) {
  if (sortKey.value !== key) return '↕'
  return sortAsc.value ? '↑' : '↓'
}

const filtered = computed(() => {
  let list = props.items
  if (props.search) {
    const q = props.search.toLowerCase()
    list = list.filter(i =>
      String(i.name || '').toLowerCase().includes(q) ||
      String(i.model || '').toLowerCase().includes(q) ||
      String(i.locationName || '').toLowerCase().includes(q) ||
      String(i.serialNumber || '').toLowerCase().includes(q)
    )
  }
  if (filterStatus.value) list = list.filter(i => String(i.status || '').toLowerCase() === filterStatus.value)
  if (filterCategory.value) list = list.filter(i => i.categoryId === Number(filterCategory.value))
  return [...list].sort((a, b) => {
    const av = a[sortKey.value] ?? ''
    const bv = b[sortKey.value] ?? ''
    const cmp = String(av).localeCompare(String(bv))
    return sortAsc.value ? cmp : -cmp
  })
})

const totalPages = computed(() => Math.max(1, Math.ceil(filtered.value.length / props.pageSize)))
const paginated  = computed(() => {
  const start = (currentPage.value - 1) * props.pageSize
  return filtered.value.slice(start, start + props.pageSize)
})
watch(totalPages, total => { if (currentPage.value > total) currentPage.value = total })

function formatDate(d) {
  if (!d) return '—'
  return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
}

function isOverdue(d) {
  if (!d) return false
  return new Date(d) < new Date()
}

function clearFilters() {
  filterStatus.value = ''
  filterCategory.value = ''
}
</script>

<style scoped>
.table-card {
  background: var(--color-card);
  border-radius: var(--radius-md);
  border: 1px solid var(--color-border);
  overflow: hidden;
  box-shadow: var(--shadow-sm);
}

.table-card__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 16px 20px;
  border-bottom: 1px solid var(--color-border);
  flex-wrap: wrap;
}

.table-card__title {
  font-size: 15px;
  font-weight: 600;
}

.table-card__filters {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.filter-select {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 6px 10px;
  font-size: 13px;
  color: var(--color-text);
  background: var(--color-bg);
  outline: none;
  cursor: pointer;
}
.filter-select:focus { border-color: var(--color-primary); }

.btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  border: none;
  border-radius: var(--radius-sm);
  padding: 7px 14px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all .15s;
}
.btn--primary {
  background: var(--color-primary);
  color: white;
}
.btn--primary:hover { background: var(--color-primary-hover); }
.btn--secondary {
  background: var(--color-bg);
  color: var(--color-text-muted);
  border: 1px solid var(--color-border);
}
.btn--secondary:hover { border-color: var(--color-primary); color: var(--color-primary); }

.table-wrapper { overflow-x: auto; }

.table-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 48px 20px;
  color: var(--color-text-muted);
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid var(--color-border);
  border-top-color: var(--color-primary);
  border-radius: 50%;
  animation: spin .7s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

.table {
  width: 100%;
  border-collapse: collapse;
  min-width: 700px;
}

th {
  padding: 10px 16px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: .5px;
  color: var(--color-text-muted);
  background: #f8fafc;
  border-bottom: 1px solid var(--color-border);
  white-space: nowrap;
}
th.sortable { cursor: pointer; user-select: none; }
th.sortable:hover { color: var(--color-primary); }
.sort-icon { opacity: .5; font-size: 11px; }
.th--actions { text-align: right; }

td {
  padding: 12px 16px;
  border-bottom: 1px solid var(--color-border);
  vertical-align: middle;
}

.table-row { cursor: pointer; transition: background .1s; }
.table-row:hover td { background: #f8fafc; }
.table-row:last-child td { border-bottom: none; }

.cell-name { display: flex; flex-direction: column; gap: 1px; }
.cell-name__text { font-weight: 500; }
.cell-name__model { font-size: 12px; color: var(--color-text-muted); }

.category-tag {
  display: inline-block;
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-radius: 4px;
  padding: 2px 8px;
  font-size: 12px;
  font-weight: 500;
}

.cell-muted { color: var(--color-text-muted); font-size: 13px; }

.cell-date { font-size: 13px; }
.cell-date--overdue { color: var(--color-danger); }
.overdue-label {
  display: inline-block;
  background: var(--color-danger-bg);
  color: var(--color-danger);
  font-size: 10px;
  font-weight: 700;
  padding: 1px 5px;
  border-radius: 3px;
  margin-left: 4px;
}

.td--actions { text-align: right; }

.action-btn {
  background: none;
  border: 1px solid var(--color-border);
  border-radius: 5px;
  padding: 5px;
  margin-left: 4px;
  display: inline-flex;
  color: var(--color-text-muted);
  transition: all .15s;
}
.action-btn--view:hover  { border-color: var(--color-primary); color: var(--color-primary); background: var(--color-primary-light); }
.action-btn--edit:hover  { border-color: var(--color-warning); color: var(--color-warning); background: var(--color-warning-bg); }
.action-btn--delete:hover { border-color: var(--color-danger); color: var(--color-danger); background: var(--color-danger-bg); }

.table-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border-top: 1px solid var(--color-border);
  gap: 12px;
  flex-wrap: wrap;
}

.table-footer__count { font-size: 13px; color: var(--color-text-muted); }

.pagination { display: flex; gap: 4px; }
.page-btn {
  min-width: 30px;
  height: 30px;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  background: none;
  color: var(--color-text-muted);
  font-size: 13px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 6px;
  transition: all .15s;
}
.page-btn:hover:not(:disabled) { border-color: var(--color-primary); color: var(--color-primary); }
.page-btn--active { background: var(--color-primary); border-color: var(--color-primary); color: white !important; }
.page-btn:disabled { opacity: .4; cursor: not-allowed; }
</style>
