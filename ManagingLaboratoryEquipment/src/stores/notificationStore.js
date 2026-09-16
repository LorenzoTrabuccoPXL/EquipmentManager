import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { notificationService } from '../services/notificationService.js'

export const useNotificationStore = defineStore('notification', () => {
  const notifications = ref([])
  const isLoading = ref(false)
  const isUpdating = ref(false)
  const error = ref('')
  const unreadCount = computed(() => notifications.value.filter(item => !item.isRead).length)

  async function fetchAll() {
    isLoading.value = true
    error.value = ''
    try {
      const response = await notificationService.getAll()
      notifications.value = (Array.isArray(response) ? response : []).sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      return notifications.value
    } catch (requestError) {
      error.value = requestError.message || 'Notifications could not be loaded.'
      throw requestError
    } finally {
      isLoading.value = false
    }
  }

  async function markRead(id) {
    const notification = notifications.value.find(item => item.notificationId === Number(id))
    if (!notification || notification.isRead) return
    isUpdating.value = true
    error.value = ''
    try {
      await notificationService.update({ ...notification, isRead: true })
      notification.isRead = true
    } catch (requestError) {
      error.value = requestError.message || 'The notification could not be updated.'
      throw requestError
    } finally {
      isUpdating.value = false
    }
  }

  async function markAllRead() {
    const unread = notifications.value.filter(item => !item.isRead)
    if (!unread.length) return
    isUpdating.value = true
    error.value = ''
    try {
      await Promise.all(unread.map(item => notificationService.update({ ...item, isRead: true })))
      unread.forEach(item => { item.isRead = true })
    } catch (requestError) {
      error.value = requestError.message || 'Not all notifications could be updated.'
      await fetchAll().catch(() => {})
      throw requestError
    } finally {
      isUpdating.value = false
    }
  }

  async function remove(id) {
    isUpdating.value = true
    error.value = ''
    try {
      await notificationService.remove(Number(id))
      notifications.value = notifications.value.filter(item => item.notificationId !== Number(id))
    } catch (requestError) {
      error.value = requestError.message || 'The notification could not be removed.'
      throw requestError
    } finally {
      isUpdating.value = false
    }
  }

  return { notifications, unreadCount, isLoading, isUpdating, error, fetchAll, markRead, markAllRead, remove }
})
