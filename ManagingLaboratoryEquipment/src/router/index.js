import { createRouter, createWebHashHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'
import EquipmentDetailView from '../views/EquipmentDetailView.vue'
import AnalyticsView from '../views/AnalyticsView.vue'
import MaintenanceLogView from '../views/MaintenanceLogView.vue'
import SettingsView from '../views/SettingsView.vue'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: LoginView },
  { path: '/dashboard', component: DashboardView, meta: { requiresAuth: true } },
  { path: '/equipment', component: DashboardView, meta: { requiresAuth: true } },
  { path: '/equipment/:id', component: EquipmentDetailView, meta: { requiresAuth: true } },
  { path: '/analytics', component: AnalyticsView, meta: { requiresAuth: true } },
  { path: '/maintenance', component: MaintenanceLogView, meta: { requiresAuth: true } },
  { path: '/settings', component: SettingsView, meta: { requiresAuth: true } },
]

const router = createRouter({
  history: createWebHashHistory(),
  routes,
})

function hasValidToken() {
  const token = sessionStorage.getItem('ems_token')
  if (!token) return false

  try {
    const encodedPayload = token.split('.')[1]
    if (!encodedPayload) return false

    const base64 = encodedPayload.replace(/-/g, '+').replace(/_/g, '/')
    const paddedBase64 = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=')
    const payload = JSON.parse(atob(paddedBase64))

    return typeof payload.exp === 'number' && payload.exp * 1000 > Date.now()
  } catch {
    return false
  }
}

function clearStoredSession() {
  sessionStorage.removeItem('ems_user')
  sessionStorage.removeItem('ems_auth')
  sessionStorage.removeItem('ems_token')
}

router.beforeEach(to => {
  const hasSession = sessionStorage.getItem('ems_auth') === 'true'
  const isLoggedIn = hasSession && hasValidToken()

  if (hasSession && !isLoggedIn) clearStoredSession()

  if (to.meta.requiresAuth && !isLoggedIn) return '/login'
  if (to.path === '/login' && isLoggedIn) return '/dashboard'
})

export default router
