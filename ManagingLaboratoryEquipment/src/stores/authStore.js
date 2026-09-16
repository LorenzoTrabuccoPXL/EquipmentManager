import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { authService } from '../services/authService.js'

const USER_KEY = 'ems_user'

function storedUser() {
  try {
    return JSON.parse(sessionStorage.getItem(USER_KEY) || 'null')
  } catch {
    return null
  }
}

export const useAuthStore = defineStore('auth', () => {
  const currentUser = ref(storedUser())
  const isLoading = ref(false)
  const error = ref('')
  const isAuthenticated = computed(() =>
    sessionStorage.getItem('ems_auth') === 'true' && Boolean(sessionStorage.getItem('ems_token'))
  )

  async function login(credentials) {
    isLoading.value = true
    error.value = ''
    try {
      const response = await authService.login(credentials)
      if (!response || typeof response !== 'object') {
        throw new Error('The login response did not contain account information.')
      }
      currentUser.value = {
        hashedId: response.hashedId || '',
        firstName: response.firstName || '',
        lastName: response.lastName || '',
        email: response.email || credentials.email,
        role: response.role || '',
        phone: response.phone || '',
      }
      sessionStorage.setItem(USER_KEY, JSON.stringify(currentUser.value))
      sessionStorage.setItem('ems_auth', 'true')
      if (response.token) sessionStorage.setItem('ems_token', response.token)
      return currentUser.value
    } catch (requestError) {
      error.value = requestError.message || 'Unable to sign in.'
      throw requestError
    } finally {
      isLoading.value = false
    }
  }

  function logout() {
    currentUser.value = null
    sessionStorage.removeItem(USER_KEY)
    sessionStorage.removeItem('ems_auth')
    sessionStorage.removeItem('ems_token')
  }

  return { currentUser, isAuthenticated, isLoading, error, login, logout }
})
