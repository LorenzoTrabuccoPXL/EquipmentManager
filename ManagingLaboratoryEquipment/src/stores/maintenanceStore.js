import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { maintenanceService } from '../services/maintenanceService.js'
import { useEquipmentStore } from './equipmentStore.js'
import { useReferenceDataStore } from './referenceDataStore.js'

function recordPayload(record) {
  return {
    maintenanceRecordId: Number(record.maintenanceRecordId) || 0,
    equipmentId: Number(record.equipmentId),
    maintenanceDate: record.maintenanceDate,
    maintenanceType: String(record.maintenanceType || '').trim(),
    technicianId: Number(record.technicianId),
    notes: String(record.notes || '').trim(),
  }
}

export const useMaintenanceStore = defineStore('maintenance', () => {
  const equipmentStore = useEquipmentStore()
  const referenceStore = useReferenceDataStore()
  const records = ref([])
  const isLoading = ref(false)
  const isSaving = ref(false)
  const error = ref('')

  const displayRecords = computed(() => records.value.map(record => ({
    ...record,
    equipmentName: equipmentStore.items.find(item => item.equipmentId === record.equipmentId)?.name || 'Unknown equipment',
    technicianName: referenceStore.getTechnicianName(record.technicianId),
  })).sort((a, b) => new Date(b.maintenanceDate) - new Date(a.maintenanceDate)))

  const recordsByMonth = computed(() => {
    const result = {}
    records.value.forEach(record => {
      const key = String(record.maintenanceDate || '').slice(0, 7)
      if (key) result[key] = (result[key] || 0) + 1
    })
    return result
  })

  const last6MonthsActivity = computed(() => {
    const now = new Date()
    return Array.from({ length: 6 }, (_, index) => {
      const date = new Date(now.getFullYear(), now.getMonth() - (5 - index), 1)
      const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`
      return { key, label: date.toLocaleDateString('en-GB', { month: 'short', year: '2-digit' }), count: recordsByMonth.value[key] || 0 }
    })
  })

  async function fetchAll() {
    isLoading.value = true
    error.value = ''
    try {
      const response = await maintenanceService.getAll()
      records.value = Array.isArray(response) ? response : []
      return records.value
    } catch (requestError) {
      error.value = requestError.message || 'Maintenance records could not be loaded.'
      throw requestError
    } finally {
      isLoading.value = false
    }
  }

  async function runMutation(action) {
    isSaving.value = true
    error.value = ''
    try {
      await action()
      await fetchAll()
    } catch (requestError) {
      error.value = requestError.message || 'The maintenance change could not be saved.'
      throw requestError
    } finally {
      isSaving.value = false
    }
  }

  const addRecord = record => runMutation(() => maintenanceService.add(recordPayload(record)))
  const updateRecord = record => runMutation(() => maintenanceService.update(recordPayload(record)))
  const deleteRecord = id => runMutation(() => maintenanceService.remove(Number(id)))

  return { records, displayRecords, last6MonthsActivity, isLoading, isSaving, error, fetchAll, addRecord, updateRecord, deleteRecord }
})
