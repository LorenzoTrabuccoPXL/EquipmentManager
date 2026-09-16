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

router.beforeEach((to, _from, next) => {
  const hasSession = sessionStorage.getItem('ems_auth') === 'true'
  const hasToken = Boolean(sessionStorage.getItem('ems_token'))
  const isLoggedIn = hasSession && hasToken
  if (hasSession && !hasToken) sessionStorage.removeItem('ems_auth')
  if (to.meta.requiresAuth && !isLoggedIn) {
    next('/login')
  } else if (to.path === '/login' && isLoggedIn) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router
