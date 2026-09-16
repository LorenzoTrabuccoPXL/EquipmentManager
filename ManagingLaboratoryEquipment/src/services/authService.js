import { apiRequest } from './apiClient.js'

export const authService = {
  login(credentials) {
    return apiRequest('/login', { method: 'POST', body: credentials })
  },
}
