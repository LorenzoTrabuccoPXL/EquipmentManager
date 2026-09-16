import { ref } from 'vue'
import { defineStore } from 'pinia'
import { categoriesService, locationsService, techniciansService } from '../services/referenceService.js'

function asArray(value) {
  return Array.isArray(value) ? value : []
}

export const useReferenceDataStore = defineStore('referenceData', () => {
  const categories = ref([])
  const locations = ref([])
  const technicians = ref([])
  const isLoading = ref(false)
  const error = ref('')
  const loaded = ref(false)

  async function fetchAll(force = false) {
    if (loaded.value && !force) return
    isLoading.value = true
    error.value = ''
    try {
      const [categoryData, locationData, technicianData] = await Promise.all([
        categoriesService.getAll(),
        locationsService.getAll(),
        techniciansService.getAll(),
      ])
      categories.value = asArray(categoryData)
      locations.value = asArray(locationData)
      technicians.value = asArray(technicianData)
      loaded.value = true
    } catch (requestError) {
      error.value = requestError.message || 'Reference data could not be loaded.'
      throw requestError
    } finally {
      isLoading.value = false
    }
  }

  function getCategoryName(id) {
    return categories.value.find(item => item.categoryId === Number(id))?.name || 'Unknown category'
  }

  function getLocationName(id) {
    return locations.value.find(item => item.locationId === Number(id))?.name || 'Unknown location'
  }

  function getTechnicianName(id) {
    const technician = technicians.value.find(item => item.technicianId === Number(id))
    return technician ? `${technician.firstName} ${technician.lastName}`.trim() : 'Unknown technician'
  }

  return { categories, locations, technicians, isLoading, error, fetchAll, getCategoryName, getLocationName, getTechnicianName }
})
