<template>
  <div class="layout">
    <AppSidebar v-model="sidebarOpen" />
    <div class="layout__content">
      <TopNavigation @toggle-sidebar="sidebarOpen = !sidebarOpen" />
      <main class="main">

        <div class="page-header">
          <div>
            <h1 class="page-title">Maintenance Log</h1>
            <p class="page-sub">{{ maintStore.records.length }} total records across all equipment</p>
          </div>
          <button class="btn btn--primary" @click="openAdd">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/>
            </svg>
            Add Record
          </button>
        </div>

        <!-- Filters -->
        <div class="filters-bar">
          <div class="search-box">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
            </svg>
            <input v-model="search" type="text" placeholder="Search by equipment, technician, notes…" />
          </div>
          <select v-model="filterType" class="filter-select">
            <option value="">All Types</option>
            <option value="Preventive">Preventive</option>
            <option value="Corrective">Corrective</option>
          </select>
          <select v-model="filterEquipment" class="filter-select">
            <option value="">All Equipment</option>
            <option v-for="eq in equipStore.items" :key="eq.equipmentId" :value="eq.equipmentId">{{ eq.name }}</option>
          </select>
          <input v-model="filterDateFrom" type="date" class="filter-select" title="From date" />
          <input v-model="filterDateTo"   type="date" class="filter-select" title="To date" />
          <button v-if="hasFilters" class="btn btn--secondary btn--sm" @click="clearFilters">Clear</button>
        </div>

        <!-- Loading -->
        <div v-if="loading" class="state-center">
          <div class="spinner" />
          <p>Loading records…</p>
        </div>

        <div v-else-if="loadError" class="state-center state-center--error">
          <p>{{ loadError }}</p>
          <button class="btn btn--primary" @click="loadData">Retry</button>
        </div>

        <!-- Empty -->
        <div v-else-if="filtered.length === 0" class="state-center">
          <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color:#94a3b8">
            <path d="M22 12h-4l-3 9L9 3l-3 9H2"/>
          </svg>
          <p>No maintenance records match your filters.</p>
          <button v-if="hasFilters" class="btn btn--secondary" @click="clearFilters">Clear filters</button>
        </div>

        <!-- Table -->
        <div v-else class="table-card">
          <div class="table-wrapper">
            <table class="table">
              <thead>
                <tr>
                  <th @click="sort('maintenanceDate')" class="sortable">Date <span class="sort-icon">{{ sortIcon('maintenanceDate') }}</span></th>
                  <th @click="sort('equipmentName')" class="sortable">Equipment <span class="sort-icon">{{ sortIcon('equipmentName') }}</span></th>
                  <th>Type</th>
                  <th @click="sort('technicianName')" class="sortable">Technician <span class="sort-icon">{{ sortIcon('technicianName') }}</span></th>
                  <th>Notes</th>
                  <th class="th--actions">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in paginated" :key="r.maintenanceRecordId" class="table-row">
                  <td class="td-date">{{ formatDate(r.maintenanceDate) }}</td>
                  <td>
                    <RouterLink :to="`/equipment/${r.equipmentId}`" class="equip-link">{{ r.equipmentName }}</RouterLink>
                  </td>
                  <td>
                    <span class="type-badge" :class="`type-badge--${typeClass(r.maintenanceType)}`">{{ r.maintenanceType }}</span>
                  </td>
                  <td class="td-muted">{{ r.technicianName }}</td>
                  <td class="td-notes">{{ r.notes }}</td>
                  <td class="td--actions" @click.stop>
                    <button class="action-btn action-btn--edit" @click="openEdit(r)" title="Edit">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
                        <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/>
                      </svg>
                    </button>
                    <button class="action-btn action-btn--delete" :disabled="maintStore.isSaving" @click="removeRecord(r)" title="Delete">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14a2 2 0 01-2 2H8a2 2 0 01-2-2L5 6"/>
                      </svg>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="table-footer">
            <span class="table-footer__count">
              Showing {{ (currentPage - 1) * pageSize + 1 }}–{{ Math.min(currentPage * pageSize, filtered.length) }} of {{ filtered.length }}
            </span>
            <div class="pagination">
              <button class="page-btn" :disabled="currentPage === 1" @click="currentPage--">‹</button>
              <button
                v-for="p in totalPages" :key="p"
                class="page-btn" :class="{ 'page-btn--active': p === currentPage }"
                @click="currentPage = p"
              >{{ p }}</button>
              <button class="page-btn" :disabled="currentPage === totalPages" @click="currentPage++">›</button>
            </div>
          </div>
        </div>

      </main>
    </div>

    <!-- Add / Edit Modal -->
    <Teleport to="body">
      <div v-if="showModal" class="modal-backdrop" @click.self="closeModal">
        <div class="modal">
          <div class="modal__header">
            <h2 class="modal__title">{{ editingRecord ? 'Edit Maintenance Record' : 'Add Maintenance Record' }}</h2>
            <button class="modal__close" @click="closeModal">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
              </svg>
            </button>
          </div>
          <form class="modal__body" @submit.prevent="handleSubmit" novalidate>
            <div v-if="formError" class="form-error-banner">{{ formError }}</div>

            <div class="form-group">
              <label class="form-label">Equipment <span class="req">*</span></label>
              <select v-model.number="form.equipmentId" class="form-input" :class="{ error: errors.equipmentId }" :disabled="!!editingRecord">
                <option value="">Select equipment…</option>
                <option v-for="eq in equipStore.items" :key="eq.equipmentId" :value="eq.equipmentId">{{ eq.name }}</option>
              </select>
              <span v-if="errors.equipmentId" class="field-error">{{ errors.equipmentId }}</span>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Type <span class="req">*</span></label>
                <select v-model="form.maintenanceType" class="form-input">
                  <option value="Preventive">Preventive</option>
                  <option value="Corrective">Corrective</option>
                </select>
              </div>
              <div class="form-group">
                <label class="form-label">Date <span class="req">*</span></label>
                <input v-model="form.maintenanceDate" type="date" class="form-input" :class="{ error: errors.maintenanceDate }" />
                <span v-if="errors.maintenanceDate" class="field-error">{{ errors.maintenanceDate }}</span>
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">Technician <span class="req">*</span></label>
              <select v-model.number="form.technicianId" class="form-input" :class="{ error: errors.technicianId }">
                <option value="">Select technician…</option>
                <option v-for="t in technicians" :key="t.technicianId" :value="t.technicianId">{{ t.firstName }} {{ t.lastName }}</option>
              </select>
              <span v-if="errors.technicianId" class="field-error">{{ errors.technicianId }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Notes <span class="req">*</span></label>
              <textarea v-model="form.notes" class="form-input form-textarea" rows="3"
                :class="{ error: errors.notes }"
                placeholder="Describe the work performed…" />
              <span v-if="errors.notes" class="field-error">{{ errors.notes }}</span>
            </div>

            <div class="modal__footer">
              <button type="button" class="btn btn--cancel" @click="closeModal">Cancel</button>
              <button type="submit" class="btn btn--submit" :disabled="maintStore.isSaving">
                {{ maintStore.isSaving ? 'Saving…' : (editingRecord ? 'Save Changes' : 'Add Record') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { RouterLink } from 'vue-router'
import AppSidebar from '../components/AppSidebar.vue'
import TopNavigation from '../components/TopNavigation.vue'
import { useEquipmentStore } from '../stores/equipmentStore.js'
import { useMaintenanceStore } from '../stores/maintenanceStore.js'
import { useReferenceDataStore } from '../stores/referenceDataStore.js'
import { useSettingsStore } from '../stores/settingsStore.js'

const equipStore = useEquipmentStore()
const maintStore = useMaintenanceStore()
const referenceStore = useReferenceDataStore()
const settingsStore = useSettingsStore()

const sidebarOpen    = ref(false)
const search         = ref('')
const filterType     = ref('')
const filterEquipment = ref('')
const filterDateFrom = ref('')
const filterDateTo   = ref('')
const sortKey        = ref('maintenanceDate')
const sortAsc        = ref(false)
const currentPage    = ref(1)
const pageSize       = computed(() => Number(settingsStore.settings?.itemsPerPage) || 10)
const technicians    = computed(() => referenceStore.technicians)
const loading        = computed(() => maintStore.isLoading || equipStore.isLoading || referenceStore.isLoading)
const loadError      = computed(() => maintStore.error || equipStore.error || referenceStore.error)

const showModal     = ref(false)
const editingRecord = ref(null)
const formError     = ref('')
const errors        = ref({})
const form          = ref(defaultForm())

function defaultForm() {
  return {
    equipmentId: '',
    maintenanceType: 'Preventive',
    maintenanceDate: new Date().toISOString().slice(0, 10),
    technicianId: '',
    notes: '',
  }
}

async function loadData() {
  await Promise.allSettled([
    referenceStore.fetchAll(),
    equipStore.fetchAll(),
    maintStore.fetchAll(),
    settingsStore.fetchAll(),
  ])
}

onMounted(loadData)

const hasFilters = computed(() =>
  search.value || filterType.value || filterEquipment.value || filterDateFrom.value || filterDateTo.value
)

const filtered = computed(() => {
  let list = maintStore.displayRecords
  if (search.value) {
    const q = search.value.toLowerCase()
    list = list.filter(r =>
      String(r.equipmentName || '').toLowerCase().includes(q) ||
      String(r.technicianName || '').toLowerCase().includes(q) ||
      String(r.notes || '').toLowerCase().includes(q)
    )
  }
  if (filterType.value)      list = list.filter(r => r.maintenanceType === filterType.value)
  if (filterEquipment.value) list = list.filter(r => r.equipmentId === Number(filterEquipment.value))
  if (filterDateFrom.value)  list = list.filter(r => toDateInput(r.maintenanceDate) >= filterDateFrom.value)
  if (filterDateTo.value)    list = list.filter(r => toDateInput(r.maintenanceDate) <= filterDateTo.value)

  return [...list].sort((a, b) => {
    const av = a[sortKey.value] ?? ''
    const bv = b[sortKey.value] ?? ''
    const c = String(av).localeCompare(String(bv))
    return sortAsc.value ? c : -c
  })
})

watch([search, filterType, filterEquipment, filterDateFrom, filterDateTo], () => { currentPage.value = 1 })

const totalPages = computed(() => Math.max(1, Math.ceil(filtered.value.length / pageSize.value)))
const paginated  = computed(() => filtered.value.slice((currentPage.value - 1) * pageSize.value, currentPage.value * pageSize.value))
watch(totalPages, total => { if (currentPage.value > total) currentPage.value = total })

function sort(key) {
  if (sortKey.value === key) sortAsc.value = !sortAsc.value
  else { sortKey.value = key; sortAsc.value = false }
}
function sortIcon(key) {
  if (sortKey.value !== key) return '↕'
  return sortAsc.value ? '↑' : '↓'
}

function clearFilters() {
  search.value = filterType.value = filterEquipment.value = filterDateFrom.value = filterDateTo.value = ''
}

function openAdd() {
  editingRecord.value = null
  form.value = defaultForm()
  errors.value = {}
  formError.value = ''
  showModal.value = true
}
function openEdit(r) {
  editingRecord.value = r
  form.value = {
    equipmentId: r.equipmentId,
    maintenanceType: r.maintenanceType,
    maintenanceDate: toDateInput(r.maintenanceDate),
    technicianId: r.technicianId,
    notes: r.notes,
  }
  errors.value = {}
  formError.value = ''
  showModal.value = true
}
function closeModal() { showModal.value = false }

function validate() {
  const e = {}
  if (!form.value.equipmentId) e.equipmentId = 'Equipment is required.'
  if (!form.value.maintenanceDate) e.maintenanceDate = 'Date is required.'
  if (!form.value.technicianId) e.technicianId = 'Technician is required.'
  if (!form.value.notes.trim()) e.notes = 'Notes are required.'
  errors.value = e
  return Object.keys(e).length === 0
}

async function handleSubmit() {
  formError.value = ''
  if (!validate()) { formError.value = 'Please fix the errors below.'; return }
  const record = {
    maintenanceRecordId: editingRecord.value?.maintenanceRecordId || 0,
    equipmentId: Number(form.value.equipmentId),
    maintenanceDate: `${form.value.maintenanceDate}T00:00:00`,
    maintenanceType: form.value.maintenanceType,
    technicianId: Number(form.value.technicianId),
    notes: form.value.notes.trim(),
  }
  try {
    if (editingRecord.value) await maintStore.updateRecord(record)
    else await maintStore.addRecord(record)
    closeModal()
  } catch (error) {
    formError.value = error.message || 'The maintenance record could not be saved.'
  }
}

async function removeRecord(record) {
  if (!window.confirm(`Delete the maintenance record for ${record.equipmentName}?`)) return
  try {
    await maintStore.deleteRecord(record.maintenanceRecordId)
  } catch {
    // The store exposes the API error in the page state.
  }
}

function toDateInput(value) {
  return value ? String(value).slice(0, 10) : ''
}

function typeClass(value) {
  return String(value || '').toLowerCase()
}

function formatDate(d) {
  if (!d) return '—'
  return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
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

.btn { display: inline-flex; align-items: center; gap: 6px; border: none; border-radius: var(--radius-sm); padding: 7px 14px; font-size: 13px; font-weight: 500; cursor: pointer; transition: all .15s; }
.btn--primary   { background: var(--color-primary); color: white; }
.btn--primary:hover { background: var(--color-primary-hover); }
.btn--secondary { background: white; color: var(--color-text-muted); border: 1px solid var(--color-border); }
.btn--secondary:hover { border-color: var(--color-primary); color: var(--color-primary); }
.btn--sm { padding: 5px 10px; font-size: 12px; }

.filters-bar {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
  background: white;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 12px 16px;
  box-shadow: var(--shadow-sm);
}
.search-box {
  display: flex; align-items: center; gap: 6px;
  background: var(--color-bg); border: 1px solid var(--color-border);
  border-radius: var(--radius-sm); padding: 0 10px; height: 32px;
  color: var(--color-text-muted); flex: 1; min-width: 200px;
}
.search-box input { flex: 1; border: none; background: none; outline: none; font-size: 13px; color: var(--color-text); }
.search-box input::placeholder { color: var(--color-text-light); }

.filter-select {
  border: 1px solid var(--color-border); border-radius: var(--radius-sm);
  padding: 5px 8px; font-size: 13px; background: var(--color-bg);
  color: var(--color-text); outline: none; cursor: pointer; height: 32px;
}
.filter-select:focus { border-color: var(--color-primary); }

.state-center { display: flex; flex-direction: column; align-items: center; gap: 12px; padding: 60px 20px; color: var(--color-text-muted); }
.state-center--error { color: var(--color-danger); }
.spinner { width: 32px; height: 32px; border: 3px solid var(--color-border); border-top-color: var(--color-primary); border-radius: 50%; animation: spin .7s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }

.table-card { background: white; border-radius: var(--radius-md); border: 1px solid var(--color-border); box-shadow: var(--shadow-sm); overflow: hidden; }
.table-wrapper { overflow-x: auto; }
.table { width: 100%; border-collapse: collapse; min-width: 700px; }
th { padding: 10px 16px; text-align: left; font-size: 12px; font-weight: 600; text-transform: uppercase; letter-spacing: .5px; color: var(--color-text-muted); background: #f8fafc; border-bottom: 1px solid var(--color-border); white-space: nowrap; }
th.sortable { cursor: pointer; user-select: none; }
th.sortable:hover { color: var(--color-primary); }
.sort-icon { opacity: .5; font-size: 11px; }
.th--actions { text-align: right; }

td { padding: 12px 16px; border-bottom: 1px solid var(--color-border); vertical-align: middle; }
.table-row:hover td { background: #f8fafc; }
.table-row:last-child td { border-bottom: none; }

.td-date  { font-size: 13px; white-space: nowrap; }
.td-muted { font-size: 13px; color: var(--color-text-muted); }
.td-notes { font-size: 12.5px; color: var(--color-text-muted); max-width: 280px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.td--actions { text-align: right; }

.equip-link { font-weight: 500; color: var(--color-primary); text-decoration: none; }
.equip-link:hover { text-decoration: underline; }

.type-badge { display: inline-block; font-size: 11px; font-weight: 700; padding: 2px 8px; border-radius: 4px; text-transform: uppercase; letter-spacing: .4px; }
.type-badge--preventive { background: var(--color-success-bg); color: var(--color-success); }
.type-badge--corrective  { background: var(--color-danger-bg);  color: var(--color-danger); }

.action-btn { background: none; border: 1px solid var(--color-border); border-radius: 5px; padding: 5px; display: inline-flex; color: var(--color-text-muted); cursor: pointer; transition: all .15s; }
.action-btn--edit:hover  { border-color: var(--color-warning); color: var(--color-warning); background: var(--color-warning-bg); }
.action-btn--delete { margin-left: 4px; }
.action-btn--delete:hover { border-color: var(--color-danger); color: var(--color-danger); background: var(--color-danger-bg); }

.table-footer { display: flex; align-items: center; justify-content: space-between; padding: 12px 16px; border-top: 1px solid var(--color-border); gap: 12px; flex-wrap: wrap; }
.table-footer__count { font-size: 13px; color: var(--color-text-muted); }
.pagination { display: flex; gap: 4px; }
.page-btn { min-width: 30px; height: 30px; border: 1px solid var(--color-border); border-radius: var(--radius-sm); background: none; color: var(--color-text-muted); font-size: 13px; display: flex; align-items: center; justify-content: center; padding: 0 6px; cursor: pointer; transition: all .15s; }
.page-btn:hover:not(:disabled) { border-color: var(--color-primary); color: var(--color-primary); }
.page-btn--active { background: var(--color-primary); border-color: var(--color-primary); color: white !important; }
.page-btn:disabled { opacity: .4; cursor: not-allowed; }

/* Modal */
.modal-backdrop { position: fixed; inset: 0; background: rgba(15,23,42,.55); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 16px; }
.modal { background: white; border-radius: var(--radius-lg); width: 100%; max-width: 520px; box-shadow: var(--shadow-lg); max-height: calc(100vh - 32px); overflow: hidden; display: flex; flex-direction: column; }
.modal__header { display: flex; align-items: center; justify-content: space-between; padding: 18px 24px; border-bottom: 1px solid var(--color-border); flex-shrink: 0; }
.modal__title  { font-size: 15px; font-weight: 600; }
.modal__close  { background: none; border: none; color: var(--color-text-muted); padding: 4px; border-radius: 4px; display: flex; cursor: pointer; }
.modal__close:hover { background: var(--color-bg); }
.modal__body   { padding: 20px 24px; overflow-y: auto; flex: 1; display: flex; flex-direction: column; gap: 14px; }
.modal__footer { display: flex; justify-content: flex-end; gap: 10px; margin-top: 8px; padding-top: 16px; border-top: 1px solid var(--color-border); }

.form-error-banner { background: var(--color-danger-bg); color: var(--color-danger); border: 1px solid #fca5a5; border-radius: var(--radius-sm); padding: 10px 14px; font-size: 13px; }
.form-group { display: flex; flex-direction: column; gap: 5px; }
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; }
.form-label { font-size: 12.5px; font-weight: 600; }
.req { color: var(--color-danger); }
.form-input { border: 1px solid var(--color-border); border-radius: var(--radius-sm); padding: 8px 12px; font-size: 13.5px; color: var(--color-text); background: var(--color-bg); outline: none; width: 100%; transition: border-color .15s; }
.form-input:focus { border-color: var(--color-primary); background: white; }
.form-input.error { border-color: var(--color-danger); }
.form-textarea { resize: vertical; min-height: 70px; }
.field-error { font-size: 12px; color: var(--color-danger); }

.btn--cancel { background: var(--color-bg); color: var(--color-text-muted); border: 1px solid var(--color-border); border-radius: var(--radius-sm); padding: 8px 20px; font-size: 13.5px; font-weight: 500; cursor: pointer; }
.btn--cancel:hover { background: var(--color-border); }
.btn--submit { background: var(--color-primary); color: white; border: none; border-radius: var(--radius-sm); padding: 8px 20px; font-size: 13.5px; font-weight: 500; cursor: pointer; }
.btn--submit:hover { background: var(--color-primary-hover); }
</style>
