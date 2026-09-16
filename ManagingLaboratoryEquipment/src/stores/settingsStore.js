import { ref } from 'vue'
import { defineStore } from 'pinia'
import { userSettingsService } from '../services/userSettingsService.js'

const defaults = () => ({
  userSettingsId: 0,
  itemsPerPage: 10,
  dateFormat: 'dd MMM yyyy',
  overdueAlerts: true,
  upcomingAlerts: true,
  offlineAlerts: true,
  emailDigest: false,
  reminderDays: 7,
  defaultTechnicianId: 0,
})

function payloadOf(value = {}) {
  return {
    ...defaults(),
    ...value,
    userSettingsId: Number(value.userSettingsId) || 0,
    itemsPerPage: Number(value.itemsPerPage) || 10,
    reminderDays: Number(value.reminderDays) || 7,
    defaultTechnicianId: Number(value.defaultTechnicianId) || 0,
  }
}

export const useSettingsStore = defineStore('settings', () => {
  const settings = ref(defaults())
  const isLoading = ref(false)
  const isSaving = ref(false)
  const error = ref('')

  async function fetchAll() {
    isLoading.value = true
    error.value = ''
    try {
      const response = await userSettingsService.getAll()
      const records = Array.isArray(response) ? response : []
      // UserSettings currently has no UserId, so the backend's single/first record is used.
      settings.value = payloadOf(records[0] || defaults())
      return settings.value
    } catch (requestError) {
      error.value = requestError.message || 'Settings could not be loaded.'
      throw requestError
    } finally {
      isLoading.value = false
    }
  }

  async function save(value) {
    const payload = payloadOf(value)
    isSaving.value = true
    error.value = ''
    try {
      if (payload.userSettingsId) await userSettingsService.update(payload)
      else await userSettingsService.add(payload)
      await fetchAll()
      return settings.value
    } catch (requestError) {
      error.value = requestError.message || 'Settings could not be saved.'
      throw requestError
    } finally {
      isSaving.value = false
    }
  }

  return { settings, isLoading, isSaving, error, fetchAll, save }
})
