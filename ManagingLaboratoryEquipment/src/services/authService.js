import { apiRequest } from './apiClient.js'

export const authService = {
  login(credentials) {
    return apiRequest('/api/Auth/login', { method: 'POST', body: credentials })
  },
}
