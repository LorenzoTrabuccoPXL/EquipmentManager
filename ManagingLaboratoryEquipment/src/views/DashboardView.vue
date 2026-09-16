<template>
  <div class="layout">
    <AppSidebar v-model="sidebarOpen" />

    <div class="layout__content">
      <TopNavigation @toggle-sidebar="sidebarOpen = !sidebarOpen" @search="searchQuery = $event" />

      <main class="main">
        <div class="page-header">
          <div>
            <h1 class="page-title">{{ pageTitle }}</h1>
            <p class="page-sub">{{ today }} · {{ equipStore.items.length }} total equipment items</p>
          </div>
        </div>

        <div v-if="loadError" class="error-banner" role="alert">
          <span>{{ loadError }}</span>
          <button type="button" @click="loadDashboard">Retry</button>
        </div>

        <div class="summary-grid">
          <SummaryCard :value="equipStore.items.length" label="Total Equipment" variant="default" :trend="2">
            <template #icon>
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M12 2H8a2 2 0 00-2 2v16a2 2 0 002 2h8a2 2 0 002-2V8l-6-6z"/>
                <polyline points="12 2 12 8 18 8"/>
              </svg>
            </template>
          </SummaryCard>
          <SummaryCard :value="equipStore.activeCount" label="Active" variant="success" :trend="1">
            <template #icon>
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M22 11.08V12a10 10 0 11-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/>
              </svg>
            </template>
          </SummaryCard>
          <SummaryCard :value="equipStore.maintenanceCount" label="Under Maintenance" variant="warning">
            <template #icon>
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M14.7 6.3a1 1 0 000 1.4l1.6 1.6a1 1 0 001.4 0l3.77-3.77a6 6 0 01-7.94 7.94l-6.91 6.91a2.12 2.12 0 01-3-3l6.91-6.91a6 6 0 017.94-7.94l-3.76 3.76z"/>
              </svg>
            </template>
          </SummaryCard>
          <SummaryCard :value="equipStore.offlineCount" label="Offline" variant="danger">
            <template #icon>
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/>
              </svg>
            </template>
          </SummaryCard>
        </div>

        <EquipmentTable
          :items="equipStore.displayItems"
          :search="searchQuery"
          :loading="equipStore.isLoading"
          :categories="referenceStore.categories"
          :page-size="settingsStore.settings?.itemsPerPage || 5"
          @add="openModal(null)"
          @edit="openModal($event)"
          @view="goToDetail($event)"
          @delete="confirmDelete($event)"
        />
      </main>
    </div>

    <EquipmentFormModal
      :show="showModal"
      :equipment="editingItem"
      :categories="referenceStore.categories"
      :locations="referenceStore.locations"
      :submitting="equipStore.isSaving"
      :api-error="formError"
      @close="closeModal"
      @submit="handleFormSubmit"
    />

    <div v-if="deleteTarget" class="confirm-backdrop" @click.self="deleteTarget = null">
      <div class="confirm-dialog">
        <div class="confirm-icon">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/>
            <line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/>
          </svg>
        </div>
        <h3 class="confirm-title">Delete Equipment?</h3>
        <p class="confirm-msg">
          Are you sure you want to delete <strong>{{ deleteTarget.name }}</strong>?
          This action cannot be undone.
        </p>
        <div class="confirm-actions">
          <button class="btn btn--cancel" @click="deleteTarget = null">Cancel</button>
          <button class="btn btn--danger" :disabled="equipStore.isSaving" @click="executeDelete">
            {{ equipStore.isSaving ? 'Deleting…' : 'Delete' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppSidebar from '../components/AppSidebar.vue'
import TopNavigation from '../components/TopNavigation.vue'
import SummaryCard from '../components/SummaryCard.vue'
import EquipmentTable from '../components/EquipmentTable.vue'
import EquipmentFormModal from '../components/EquipmentFormModal.vue'
import { useEquipmentStore } from '../stores/equipmentStore.js'
import { useReferenceDataStore } from '../stores/referenceDataStore.js'
import { useSettingsStore } from '../stores/settingsStore.js'

const route       = useRoute()
const router      = useRouter()
const equipStore  = useEquipmentStore()
const referenceStore = useReferenceDataStore()
const settingsStore = useSettingsStore()
const sidebarOpen = ref(false)
const searchQuery = ref('')
const showModal   = ref(false)
const editingItem = ref(null)
const deleteTarget = ref(null)
const formError = ref('')

const today = new Date().toLocaleDateString('en-GB', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })
const pageTitle = computed(() => route.path === '/equipment' ? 'Equipment' : 'Dashboard')
const loadError = computed(() => equipStore.error || referenceStore.error)

async function loadDashboard() {
  await Promise.allSettled([
    referenceStore.fetchAll(),
    equipStore.fetchAll(),
    settingsStore.fetchAll(),
  ])
}

onMounted(loadDashboard)

function openModal(item) {
  editingItem.value = item ? { ...item } : null
  formError.value = ''
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  editingItem.value = null
  formError.value = ''
}

async function handleFormSubmit(data) {
  formError.value = ''
  try {
    if (data.equipmentId) await equipStore.updateItem(data)
    else                  await equipStore.addItem(data)
    closeModal()
  } catch (error) {
    formError.value = error.message || 'The equipment could not be saved.'
  }
}

function confirmDelete(item) {
  deleteTarget.value = item
}

async function executeDelete() {
  if (!deleteTarget.value) return
  try {
    await equipStore.deleteItem(deleteTarget.value.equipmentId)
    deleteTarget.value = null
  } catch {
    // The store exposes the API message through equipStore.error.
  }
}

function goToDetail(item) {
  router.push(`/equipment/${item.equipmentId}`)
}

</script>

<style scoped>
.layout { display: flex; min-height: 100%; }
.layout__content {
  flex: 1;
  margin-left: var(--sidebar-width);
  display: flex;
  flex-direction: column;
  min-height: 100%;
}
@media (max-width: 899px) { .layout__content { margin-left: 0; } }

.main {
  padding: 20px;
  padding-top: calc(var(--topnav-height) + 20px);
  display: flex;
  flex-direction: column;
  gap: 20px;
}
.page-header { display: flex; align-items: flex-start; justify-content: space-between; gap: 12px; }
.page-title  { font-size: 20px; font-weight: 700; }
.page-sub    { font-size: 13px; color: var(--color-text-muted); margin-top: 2px; }

.error-banner {
  display: flex; align-items: center; justify-content: space-between; gap: 12px;
  padding: 10px 14px; border: 1px solid #fca5a5; border-radius: var(--radius-sm);
  background: var(--color-danger-bg); color: var(--color-danger); font-size: 13px;
}
.error-banner button { border: none; background: none; color: inherit; font-weight: 600; text-decoration: underline; }

.summary-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}
@media (max-width: 1100px) { .summary-grid { grid-template-columns: repeat(2, 1fr); } }
@media (max-width: 560px)  { .summary-grid { grid-template-columns: 1fr; } }

.confirm-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(15,23,42,.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 900;
  padding: 16px;
}
.confirm-dialog {
  background: white;
  border-radius: var(--radius-lg);
  padding: 28px;
  max-width: 380px;
  width: 100%;
  box-shadow: var(--shadow-lg);
  text-align: center;
}
.confirm-icon {
  width: 52px; height: 52px;
  background: var(--color-warning-bg);
  color: var(--color-warning);
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  margin: 0 auto 14px;
}
.confirm-title { font-size: 16px; font-weight: 700; margin-bottom: 8px; }
.confirm-msg   { font-size: 13.5px; color: var(--color-text-muted); line-height: 1.55; margin-bottom: 20px; }
.confirm-actions { display: flex; gap: 10px; justify-content: center; }
.btn {
  border-radius: var(--radius-sm); padding: 8px 20px;
  font-size: 13.5px; font-weight: 500; cursor: pointer; transition: all .15s; border: none;
}
.btn--cancel { background: var(--color-bg); color: var(--color-text-muted); border: 1px solid var(--color-border); }
.btn--cancel:hover { background: var(--color-border); }
.btn--danger { background: var(--color-danger); color: white; }
.btn--danger:hover { background: #b91c1c; }
</style>
