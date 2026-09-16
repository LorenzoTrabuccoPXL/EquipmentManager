import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { equipmentService } from '../services/equipmentService.js'
import { useReferenceDataStore } from './referenceDataStore.js'

const statusOf = item => String(item?.status || '').toLowerCase()

function hasDate(value) {
  if (!value) return false
  const date = new Date(value)
  return !Number.isNaN(date.getTime()) && date.getUTCFullYear() > 1
}

function equipmentPayload(item) {
  return {
    equipmentId: Number(item.equipmentId) || 0,
    name: String(item.name || '').trim(),
    model: String(item.model || '').trim(),
    serialNumber: String(item.serialNumber || '').trim(),
    categoryId: Number(item.categoryId),
    locationId: Number(item.locationId),
    status: String(item.status || '').toLowerCase(),
    purchasePrice: Number(item.purchasePrice) || 0,
    warrantyExpiry: item.warrantyExpiry,
    lastMaintenance: item.lastMaintenance,
    nextMaintenance: item.nextMaintenance,
    description: String(item.description || '').trim(),
  }
}

export const useEquipmentStore = defineStore('equipment', () => {
  const referenceStore = useReferenceDataStore()
  const items = ref([])
  const selectedItem = ref(null)
  const isLoading = ref(false)
  const isSaving = ref(false)
  const error = ref('')

  const displayItems = computed(() => items.value.map(item => ({
    ...item,
    categoryName: referenceStore.getCategoryName(item.categoryId),
    locationName: referenceStore.getLocationName(item.locationId),
  })))
  const activeCount = computed(() => items.value.filter(item => statusOf(item) === 'active').length)
  const maintenanceCount = computed(() => items.value.filter(item => statusOf(item) === 'maintenance').length)
  const offlineCount = computed(() => items.value.filter(item => statusOf(item) === 'offline').length)
  const overdueItems = computed(() => {
    const now = new Date()
    return items.value.filter(item => hasDate(item.nextMaintenance) && new Date(item.nextMaintenance) < now)
  })
  const upcomingItems = computed(() => {
    const now = new Date()
    const weekLater = new Date(now.getTime() + 7 * 86400000)
    return items.value.filter(item => {
      if (!hasDate(item.nextMaintenance)) return false
      const date = new Date(item.nextMaintenance)
      return date >= now && date <= weekLater
    })
  })

  async function fetchAll() {
    isLoading.value = true
    error.value = ''
    try {
      const response = await equipmentService.getAll()
      items.value = Array.isArray(response) ? response : []
      return items.value
    } catch (requestError) {
      error.value = requestError.message || 'Equipment could not be loaded.'
      throw requestError
    } finally {
      isLoading.value = false
    }
  }

  async function fetchById(id) {
    isLoading.value = true
    error.value = ''
    selectedItem.value = null
    try {
      const response = await equipmentService.getById(Number(id))
      selectedItem.value = response || null
      if (response) {
        const index = items.value.findIndex(item => item.equipmentId === response.equipmentId)
        if (index === -1) items.value.push(response)
        else items.value[index] = response
      }
      return selectedItem.value
    } catch (requestError) {
      error.value = requestError.message || 'Equipment details could not be loaded.'
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
      error.value = requestError.message || 'The equipment change could not be saved.'
      throw requestError
    } finally {
      isSaving.value = false
    }
  }

  const addItem = item => runMutation(() => equipmentService.add(equipmentPayload(item)))

  async function updateItem(item) {
    const payload = equipmentPayload(item)
    await runMutation(() => equipmentService.update(payload))
    selectedItem.value = items.value.find(entry => entry.equipmentId === payload.equipmentId) || null
  }

  async function deleteItem(id) {
    await runMutation(() => equipmentService.remove(Number(id)))
    if (selectedItem.value?.equipmentId === Number(id)) selectedItem.value = null
  }

  return {
    items, selectedItem, displayItems, isLoading, isSaving, error,
    activeCount, maintenanceCount, offlineCount, overdueItems, upcomingItems,
    fetchAll, fetchById, addItem, updateItem, deleteItem,
  }
})
