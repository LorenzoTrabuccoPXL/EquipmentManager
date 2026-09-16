<template>
  <div class="layout">
    <AppSidebar v-model="sidebarOpen" />
    <div class="layout__content">
      <TopNavigation @toggle-sidebar="sidebarOpen = !sidebarOpen" />
      <main class="main">
        <div v-if="loading" class="state-center">
          <div class="spinner" />
          <p>Loading equipment…</p>
        </div>
        <div v-else-if="loadError" class="state-center state-center--error">
          <p>{{ loadError }}</p>
          <button class="btn btn--primary" @click="loadDetails">Retry</button>
        </div>
        <div v-else-if="!item" class="state-center">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color:#94a3b8">
            <circle cx="12" cy="12" r="10"/>
            <line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/>
          </svg>
          <p>Equipment not found.</p>
          <button class="btn btn--primary" @click="router.push('/dashboard')">Back to Dashboard</button>
        </div>
        <EquipmentDetail
          v-else
          :equipment="item"
          :category-name="referenceStore.getCategoryName(item.categoryId)"
          :location-name="referenceStore.getLocationName(item.locationId)"
          :maintenance-records="maintenanceRecords"
          @back="router.push('/dashboard')"
          @edit="openModal"
          @delete="confirmDelete"
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
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppSidebar from '../components/AppSidebar.vue'
import TopNavigation from '../components/TopNavigation.vue'
import EquipmentDetail from '../components/EquipmentDetail.vue'
import EquipmentFormModal from '../components/EquipmentFormModal.vue'
import { useEquipmentStore } from '../stores/equipmentStore.js'
import { useMaintenanceStore } from '../stores/maintenanceStore.js'
import { useReferenceDataStore } from '../stores/referenceDataStore.js'

const route      = useRoute()
const router     = useRouter()
const equipStore = useEquipmentStore()
const maintStore = useMaintenanceStore()
const referenceStore = useReferenceDataStore()

const sidebarOpen  = ref(false)
const showModal    = ref(false)
const editingItem  = ref(null)
const deleteTarget = ref(null)
const formError = ref('')
const loading = ref(false)

const item = computed(() => {
  const id = parseInt(route.params.id)
  if (equipStore.selectedItem?.equipmentId === id) return equipStore.selectedItem
  return equipStore.items.find(e => e.equipmentId === id) || null
})
const maintenanceRecords = computed(() =>
  maintStore.displayRecords.filter(record => record.equipmentId === Number(route.params.id))
)
const loadError = computed(() => equipStore.error || maintStore.error || referenceStore.error)

async function loadDetails() {
  loading.value = true
  try {
    await Promise.all([
      referenceStore.fetchAll(),
      maintStore.fetchAll(),
      equipStore.fetchById(Number(route.params.id)),
    ])
  } catch {
    // Individual stores expose their error message to the page.
  } finally {
    loading.value = false
  }
}

onMounted(loadDetails)
watch(() => route.params.id, loadDetails)

function openModal(eq) {
  editingItem.value = { ...eq }
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
    await equipStore.updateItem(data)
    await equipStore.fetchById(data.equipmentId)
    closeModal()
  } catch (error) {
    formError.value = error.message || 'The equipment could not be saved.'
  }
}
function confirmDelete(eq) { deleteTarget.value = eq }
async function executeDelete() {
  if (!deleteTarget.value) return
  try {
    await equipStore.deleteItem(deleteTarget.value.equipmentId)
    deleteTarget.value = null
    await router.push('/equipment')
  } catch {
    // The store keeps the server message available for the retry state.
  }
}
</script>

<style scoped>
.layout { display: flex; min-height: 100%; }
.layout__content {
  flex: 1; margin-left: var(--sidebar-width);
  display: flex; flex-direction: column; min-height: 100%;
}
@media (max-width: 899px) { .layout__content { margin-left: 0; } }
.main { padding: 20px; padding-top: calc(var(--topnav-height) + 20px); }
.state-center {
  display: flex; flex-direction: column; align-items: center;
  justify-content: center; gap: 14px; padding: 60px 20px;
  color: var(--color-text-muted);
}
.state-center--error { color: var(--color-danger); }
.spinner { width: 32px; height: 32px; border: 3px solid var(--color-border); border-top-color: var(--color-primary); border-radius: 50%; animation: spin .7s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
.confirm-backdrop {
  position: fixed; inset: 0; background: rgba(15,23,42,.5);
  display: flex; align-items: center; justify-content: center;
  z-index: 900; padding: 16px;
}
.confirm-dialog {
  background: white; border-radius: var(--radius-lg);
  padding: 28px; max-width: 380px; width: 100%;
  box-shadow: var(--shadow-lg); text-align: center;
}
.confirm-icon {
  width: 52px; height: 52px; background: var(--color-warning-bg);
  color: var(--color-warning); border-radius: 50%;
  display: flex; align-items: center; justify-content: center; margin: 0 auto 14px;
}
.confirm-title { font-size: 16px; font-weight: 700; margin-bottom: 8px; }
.confirm-msg { font-size: 13.5px; color: var(--color-text-muted); line-height: 1.55; margin-bottom: 20px; }
.confirm-actions { display: flex; gap: 10px; justify-content: center; }
.btn {
  border-radius: var(--radius-sm); padding: 8px 20px;
  font-size: 13.5px; font-weight: 500; cursor: pointer; transition: all .15s; border: none;
}
.btn--primary { background: var(--color-primary); color: white; }
.btn--primary:hover { background: var(--color-primary-hover); }
.btn--cancel { background: var(--color-bg); color: var(--color-text-muted); border: 1px solid var(--color-border); }
.btn--cancel:hover { background: var(--color-border); }
.btn--danger { background: var(--color-danger); color: white; }
.btn--danger:hover { background: #b91c1c; }
</style>
