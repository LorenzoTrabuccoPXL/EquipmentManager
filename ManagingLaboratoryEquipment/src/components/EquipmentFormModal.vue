<template>
  <Teleport to="body">
    <div v-if="show" class="modal-backdrop" @click.self="$emit('close')">
      <div class="modal" role="dialog" aria-modal="true">
        <div class="modal__header">
          <h2 class="modal__title">{{ isEdit ? 'Edit Equipment' : 'Add New Equipment' }}</h2>
          <button class="modal__close" @click="$emit('close')" aria-label="Close">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
            </svg>
          </button>
        </div>

        <form class="modal__body" @submit.prevent="handleSubmit" novalidate>
          <div v-if="errorMsg || apiError" class="form-error-banner">{{ errorMsg || apiError }}</div>

          <div class="form-grid">
            <div class="form-group form-group--full">
              <label class="form-label">Equipment Name <span class="required">*</span></label>
              <input v-model="form.name" type="text" class="form-input" :class="{ error: errors.name }" placeholder="e.g. CNC Milling Machine A1" />
              <span v-if="errors.name" class="form-error">{{ errors.name }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Model <span class="required">*</span></label>
              <input v-model="form.model" type="text" class="form-input" :class="{ error: errors.model }" placeholder="e.g. Haas VF-2" />
              <span v-if="errors.model" class="form-error">{{ errors.model }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Serial Number <span class="required">*</span></label>
              <input v-model="form.serialNumber" type="text" class="form-input" :class="{ error: errors.serialNumber }" placeholder="e.g. HVF2-2021-0041" />
              <span v-if="errors.serialNumber" class="form-error">{{ errors.serialNumber }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Category <span class="required">*</span></label>
              <select v-model.number="form.categoryId" class="form-input" :class="{ error: errors.categoryId }">
                <option value="">Select a category…</option>
                <option v-for="cat in categories" :key="cat.categoryId" :value="cat.categoryId">{{ cat.name }}</option>
              </select>
              <span v-if="errors.categoryId" class="form-error">{{ errors.categoryId }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-input">
                <option value="active">Active</option>
                <option value="maintenance">Maintenance</option>
                <option value="offline">Offline</option>
              </select>
            </div>

            <div class="form-group form-group--full">
              <label class="form-label">Location <span class="required">*</span></label>
              <select v-model.number="form.locationId" class="form-input" :class="{ error: errors.locationId }">
                <option value="">Select a location…</option>
                <option v-for="location in locations" :key="location.locationId" :value="location.locationId">{{ location.name }}</option>
              </select>
              <span v-if="errors.locationId" class="form-error">{{ errors.locationId }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Purchase Price (£)</label>
              <input v-model.number="form.purchasePrice" type="number" class="form-input" placeholder="0" min="0" />
            </div>

            <div class="form-group">
              <label class="form-label">Warranty Expiry <span class="required">*</span></label>
              <input v-model="form.warrantyExpiry" type="date" class="form-input" :class="{ error: errors.warrantyExpiry }" />
              <span v-if="errors.warrantyExpiry" class="form-error">{{ errors.warrantyExpiry }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Last Maintenance <span class="required">*</span></label>
              <input v-model="form.lastMaintenance" type="date" class="form-input" :class="{ error: errors.lastMaintenance }" />
              <span v-if="errors.lastMaintenance" class="form-error">{{ errors.lastMaintenance }}</span>
            </div>

            <div class="form-group">
              <label class="form-label">Next Maintenance <span class="required">*</span></label>
              <input v-model="form.nextMaintenance" type="date" class="form-input" :class="{ error: errors.nextMaintenance }" />
              <span v-if="errors.nextMaintenance" class="form-error">{{ errors.nextMaintenance }}</span>
            </div>

            <div class="form-group form-group--full">
              <label class="form-label">Description</label>
              <textarea v-model="form.description" class="form-input form-textarea" rows="3" placeholder="Brief description of the equipment and its purpose…" />
            </div>
          </div>

          <div class="modal__footer">
            <button type="button" class="btn btn--cancel" @click="$emit('close')">Cancel</button>
            <button type="submit" class="btn btn--submit" :disabled="submitting">
              {{ submitting ? 'Saving…' : (isEdit ? 'Save Changes' : 'Add Equipment') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { computed, ref, watch } from 'vue'

const props = defineProps({
  show:      { type: Boolean, default: false },
  equipment: { type: Object, default: null },
  categories: { type: Array, default: () => [] },
  locations: { type: Array, default: () => [] },
  submitting: { type: Boolean, default: false },
  apiError: { type: String, default: '' },
})

const emit = defineEmits(['close', 'submit'])

const isEdit = computed(() => !!props.equipment)

const defaultForm = () => ({
  equipmentId: 0, name: '', model: '', serialNumber: '', categoryId: '',
  status: 'active', locationId: '', purchasePrice: 0, warrantyExpiry: '',
  lastMaintenance: '', nextMaintenance: '', description: '',
})

const dateInput = value => value ? String(value).slice(0, 10) : ''
const apiDate = value => `${value}T00:00:00`

const form    = ref(defaultForm())
const errors  = ref({})
const errorMsg = ref('')

watch(() => props.equipment, (eq) => {
  if (eq) {
    form.value = {
      ...defaultForm(),
      ...eq,
      warrantyExpiry: dateInput(eq.warrantyExpiry),
      lastMaintenance: dateInput(eq.lastMaintenance),
      nextMaintenance: dateInput(eq.nextMaintenance),
    }
  } else {
    form.value = defaultForm()
  }
  errors.value = {}
  errorMsg.value = ''
}, { immediate: true })

watch(() => props.show, (v) => {
  if (!v) { errors.value = {}; errorMsg.value = '' }
})

function validate() {
  const e = {}
  if (!form.value.name.trim()) e.name = 'Name is required.'
  if (!form.value.model.trim()) e.model = 'Model is required.'
  if (!form.value.serialNumber.trim()) e.serialNumber = 'Serial number is required.'
  if (!form.value.categoryId) e.categoryId = 'Category is required.'
  if (!form.value.locationId) e.locationId = 'Location is required.'
  if (!form.value.warrantyExpiry) e.warrantyExpiry = 'Warranty expiry is required.'
  if (!form.value.lastMaintenance) e.lastMaintenance = 'Last maintenance is required.'
  if (!form.value.nextMaintenance) e.nextMaintenance = 'Next maintenance is required.'
  errors.value = e
  return Object.keys(e).length === 0
}

function handleSubmit() {
  errorMsg.value = ''
  if (!validate()) {
    errorMsg.value = 'Please fix the errors below before saving.'
    return
  }
  emit('submit', {
    equipmentId: Number(form.value.equipmentId) || 0,
    name: form.value.name.trim(),
    model: form.value.model.trim(),
    serialNumber: form.value.serialNumber.trim(),
    categoryId: Number(form.value.categoryId),
    locationId: Number(form.value.locationId),
    status: form.value.status,
    purchasePrice: Number(form.value.purchasePrice) || 0,
    warrantyExpiry: apiDate(form.value.warrantyExpiry),
    lastMaintenance: apiDate(form.value.lastMaintenance),
    nextMaintenance: apiDate(form.value.nextMaintenance),
    description: form.value.description.trim(),
  })
}
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, .55);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 16px;
  overflow-y: auto;
}

.modal {
  background: var(--color-card);
  border-radius: var(--radius-lg);
  width: 100%;
  max-width: 640px;
  box-shadow: var(--shadow-lg);
  display: flex;
  flex-direction: column;
  max-height: calc(100vh - 32px);
  overflow: hidden;
}

.modal__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 18px 24px;
  border-bottom: 1px solid var(--color-border);
  flex-shrink: 0;
}

.modal__title { font-size: 15px; font-weight: 600; }

.modal__close {
  background: none;
  border: none;
  color: var(--color-text-muted);
  padding: 4px;
  border-radius: 4px;
  display: flex;
  cursor: pointer;
  transition: background .15s;
}
.modal__close:hover { background: var(--color-bg); color: var(--color-text); }

.modal__body {
  padding: 20px 24px;
  overflow-y: auto;
  flex: 1;
}

.form-error-banner {
  background: var(--color-danger-bg);
  color: var(--color-danger);
  border: 1px solid #fca5a5;
  border-radius: var(--radius-sm);
  padding: 10px 14px;
  font-size: 13px;
  margin-bottom: 16px;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;
}
@media (max-width: 560px) { .form-grid { grid-template-columns: 1fr; } }

.form-group { display: flex; flex-direction: column; gap: 5px; }
.form-group--full { grid-column: 1 / -1; }

.form-label {
  font-size: 12.5px;
  font-weight: 600;
  color: var(--color-text);
}
.required { color: var(--color-danger); }

.form-input {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 8px 12px;
  font-size: 13.5px;
  color: var(--color-text);
  background: var(--color-bg);
  outline: none;
  transition: border-color .15s;
  width: 100%;
}
.form-input:focus { border-color: var(--color-primary); background: white; }
.form-input.error { border-color: var(--color-danger); }

.form-textarea { resize: vertical; min-height: 70px; }

.form-error { font-size: 12px; color: var(--color-danger); }

.modal__footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid var(--color-border);
}

.btn {
  border-radius: var(--radius-sm);
  padding: 8px 20px;
  font-size: 13.5px;
  font-weight: 500;
  cursor: pointer;
  transition: all .15s;
  border: none;
}
.btn--cancel {
  background: var(--color-bg);
  color: var(--color-text-muted);
  border: 1px solid var(--color-border);
}
.btn--cancel:hover { background: var(--color-border); }
.btn--submit {
  background: var(--color-primary);
  color: white;
}
.btn--submit:hover { background: var(--color-primary-hover); }
</style>
