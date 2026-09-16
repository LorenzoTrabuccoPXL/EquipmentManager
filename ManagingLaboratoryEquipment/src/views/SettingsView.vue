<template>
  <div class="layout">
    <AppSidebar v-model="sidebarOpen" />
    <div class="layout__content">
      <TopNavigation @toggle-sidebar="sidebarOpen = !sidebarOpen" />
      <main class="main">

        <div class="page-header">
          <div>
            <h1 class="page-title">Settings</h1>
            <p class="page-sub">Manage your profile, preferences, and notifications</p>
          </div>
        </div>

        <div v-if="loading" class="settings-state">
          <div class="spinner" />
          <span>Loading settings…</span>
        </div>
        <div v-else-if="loadError" class="settings-state settings-state--error" role="alert">
          <span>{{ loadError }}</span>
          <button type="button" class="btn btn--ghost" @click="loadSettings">Retry</button>
        </div>

        <!-- Toast -->
        <Transition name="toast">
          <div v-if="toastMsg" class="toast">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M22 11.08V12a10 10 0 11-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/>
            </svg>
            {{ toastMsg }}
          </div>
        </Transition>

        <div class="settings-grid">

          <!-- Profile -->
          <section class="settings-card">
            <div class="settings-card__header">
              <div class="settings-card__icon">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2"/>
                  <circle cx="12" cy="7" r="4"/>
                </svg>
              </div>
              <h2 class="settings-card__title">Profile</h2>
            </div>
            <form @submit.prevent novalidate>
              <div class="form-grid">
                <div class="form-group">
                  <label class="form-label">Full Name <span class="req">*</span></label>
                  <input :value="profile.name" type="text" class="form-input" disabled />
                </div>
                <div class="form-group">
                  <label class="form-label">Email Address <span class="req">*</span></label>
                  <input :value="profile.email" type="email" class="form-input" disabled />
                </div>
                <div class="form-group">
                  <label class="form-label">Role</label>
                  <input :value="profile.role" type="text" class="form-input" disabled />
                </div>
                <div class="form-group">
                  <label class="form-label">Phone</label>
                  <input :value="profile.phone" type="tel" class="form-input" disabled />
                </div>
              </div>
              <p class="profile-note">Profile details come from the account used to sign in.</p>
            </form>
          </section>

          <!-- Display Preferences -->
          <section class="settings-card">
            <div class="settings-card__header">
              <div class="settings-card__icon">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="2" y="3" width="20" height="14" rx="2" ry="2"/>
                  <line x1="8" y1="21" x2="16" y2="21"/><line x1="12" y1="17" x2="12" y2="21"/>
                </svg>
              </div>
              <h2 class="settings-card__title">Display Preferences</h2>
            </div>
            <form @submit.prevent="saveDisplay" novalidate>
              <div class="form-grid">
                <div class="form-group">
                  <label class="form-label">Items per Page</label>
                  <select v-model.number="display.itemsPerPage" class="form-input">
                    <option :value="5">5</option>
                    <option :value="10">10</option>
                    <option :value="20">20</option>
                    <option :value="50">50</option>
                  </select>
                </div>
                <div class="form-group">
                  <label class="form-label">Date Format</label>
                  <select v-model="display.dateFormat" class="form-input">
                    <option value="dd MMM yyyy">15 Sep 2026</option>
                    <option value="MM/dd/yyyy">09/15/2026</option>
                    <option value="yyyy-MM-dd">2026-09-15</option>
                  </select>
                </div>
              </div>
              <div class="form-actions">
                <button type="button" class="btn btn--ghost" @click="resetDisplay">Reset</button>
                <button type="submit" class="btn btn--primary" :disabled="settingsStore.isSaving">Save Display</button>
              </div>
            </form>
          </section>

          <!-- Notification Preferences -->
          <section class="settings-card">
            <div class="settings-card__header">
              <div class="settings-card__icon">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M18 8A6 6 0 006 8c0 7-3 9-3 9h18s-3-2-3-9"/>
                  <path d="M13.73 21a2 2 0 01-3.46 0"/>
                </svg>
              </div>
              <h2 class="settings-card__title">Notification Preferences</h2>
            </div>
            <form @submit.prevent="saveNotifications" novalidate>
              <div class="toggle-list">
                <label class="toggle-row">
                  <div class="toggle-row__text">
                    <span class="toggle-row__label">Overdue Maintenance Alerts</span>
                    <span class="toggle-row__desc">Alert when maintenance is past due</span>
                  </div>
                  <input type="checkbox" v-model="notifs.overdueAlerts" class="toggle-cb" />
                  <span class="toggle-track" :class="{ 'toggle-track--on': notifs.overdueAlerts }" />
                </label>
                <label class="toggle-row">
                  <div class="toggle-row__text">
                    <span class="toggle-row__label">Upcoming Maintenance Alerts</span>
                    <span class="toggle-row__desc">Alert when maintenance is due within 7 days</span>
                  </div>
                  <input type="checkbox" v-model="notifs.upcomingAlerts" class="toggle-cb" />
                  <span class="toggle-track" :class="{ 'toggle-track--on': notifs.upcomingAlerts }" />
                </label>
                <label class="toggle-row">
                  <div class="toggle-row__text">
                    <span class="toggle-row__label">Equipment Offline Alerts</span>
                    <span class="toggle-row__desc">Alert when equipment goes offline</span>
                  </div>
                  <input type="checkbox" v-model="notifs.offlineAlerts" class="toggle-cb" />
                  <span class="toggle-track" :class="{ 'toggle-track--on': notifs.offlineAlerts }" />
                </label>
                <label class="toggle-row">
                  <div class="toggle-row__text">
                    <span class="toggle-row__label">Email Digest</span>
                    <span class="toggle-row__desc">Receive a daily summary email</span>
                  </div>
                  <input type="checkbox" v-model="notifs.emailDigest" class="toggle-cb" />
                  <span class="toggle-track" :class="{ 'toggle-track--on': notifs.emailDigest }" />
                </label>
              </div>
              <div class="form-actions">
                <button type="button" class="btn btn--ghost" @click="resetNotifications">Reset</button>
                <button type="submit" class="btn btn--primary" :disabled="settingsStore.isSaving">Save Notifications</button>
              </div>
            </form>
          </section>

          <!-- Maintenance Reminders -->
          <section class="settings-card">
            <div class="settings-card__header">
              <div class="settings-card__icon">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M14.7 6.3a1 1 0 000 1.4l1.6 1.6a1 1 0 001.4 0l3.77-3.77a6 6 0 01-7.94 7.94l-6.91 6.91a2.12 2.12 0 01-3-3l6.91-6.91a6 6 0 017.94-7.94l-3.76 3.76z"/>
                </svg>
              </div>
              <h2 class="settings-card__title">Maintenance Reminders</h2>
            </div>
            <form @submit.prevent="saveMaintenanceSettings" novalidate>
              <div class="form-grid">
                <div class="form-group">
                  <label class="form-label">Advance Reminder (days)</label>
                  <div class="input-with-hint">
                    <input
                      v-model.number="maintenance.reminderDays"
                      type="number" min="1" max="90"
                      class="form-input"
                      :class="{ error: maintenanceErrors.reminderDays }"
                    />
                    <span class="input-hint">days before due date</span>
                  </div>
                  <span v-if="maintenanceErrors.reminderDays" class="field-error">{{ maintenanceErrors.reminderDays }}</span>
                </div>
                <div class="form-group">
                  <label class="form-label">Default Technician</label>
                  <select v-model.number="maintenance.defaultTechnicianId" class="form-input">
                    <option :value="0">No default</option>
                    <option v-for="t in technicians" :key="t.technicianId" :value="t.technicianId">{{ t.firstName }} {{ t.lastName }}</option>
                  </select>
                </div>
              </div>
              <div class="form-actions">
                <button type="button" class="btn btn--ghost" @click="resetMaintenanceSettings">Reset</button>
                <button type="submit" class="btn btn--primary" :disabled="settingsStore.isSaving">Save Reminders</button>
              </div>
            </form>
          </section>

        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import AppSidebar from '../components/AppSidebar.vue'
import TopNavigation from '../components/TopNavigation.vue'
import { useAuthStore } from '../stores/authStore.js'
import { useReferenceDataStore } from '../stores/referenceDataStore.js'
import { useSettingsStore } from '../stores/settingsStore.js'

const fallback = {
  userSettingsId: 0,
  itemsPerPage: 10,
  dateFormat: 'dd MMM yyyy',
  overdueAlerts: true,
  upcomingAlerts: true,
  offlineAlerts: true,
  emailDigest: false,
  reminderDays: 7,
  defaultTechnicianId: 0,
}

const sidebarOpen   = ref(false)
const settingsStore = useSettingsStore()
const referenceStore = useReferenceDataStore()
const authStore = useAuthStore()
const toastMsg = ref('')
let toastTimer = null

const display = ref({ itemsPerPage: fallback.itemsPerPage, dateFormat: fallback.dateFormat })
const notifs = ref({
  overdueAlerts: fallback.overdueAlerts,
  upcomingAlerts: fallback.upcomingAlerts,
  offlineAlerts: fallback.offlineAlerts,
  emailDigest: fallback.emailDigest,
})
const maintenance = ref({
  reminderDays: fallback.reminderDays,
  defaultTechnicianId: fallback.defaultTechnicianId,
})
const maintenanceErrors = ref({})

const profile = computed(() => {
  const user = authStore.currentUser || {}
  return {
    name: [user.firstName, user.lastName].filter(Boolean).join(' ') || 'Signed-in user',
    email: user.email || '',
    role: user.role || '',
    phone: user.phone || '',
  }
})
const technicians = computed(() => referenceStore.technicians)
const loading = computed(() => settingsStore.isLoading || referenceStore.isLoading)
const loadError = computed(() => settingsStore.error || referenceStore.error)

function currentSettings() {
  return { ...fallback, ...settingsStore.settings }
}

function syncDrafts() {
  const saved = currentSettings()
  display.value = { itemsPerPage: saved.itemsPerPage, dateFormat: saved.dateFormat }
  notifs.value = {
    overdueAlerts: saved.overdueAlerts,
    upcomingAlerts: saved.upcomingAlerts,
    offlineAlerts: saved.offlineAlerts,
    emailDigest: saved.emailDigest,
  }
  maintenance.value = {
    reminderDays: saved.reminderDays,
    defaultTechnicianId: saved.defaultTechnicianId || 0,
  }
}

async function loadSettings() {
  await Promise.allSettled([settingsStore.fetchAll(), referenceStore.fetchAll()])
  syncDrafts()
}

onMounted(loadSettings)

function showToast(message) {
  toastMsg.value = message
  clearTimeout(toastTimer)
  toastTimer = setTimeout(() => { toastMsg.value = '' }, 3000)
}

function buildPayload() {
  return {
    ...currentSettings(),
    itemsPerPage: Number(display.value.itemsPerPage),
    dateFormat: display.value.dateFormat,
    overdueAlerts: Boolean(notifs.value.overdueAlerts),
    upcomingAlerts: Boolean(notifs.value.upcomingAlerts),
    offlineAlerts: Boolean(notifs.value.offlineAlerts),
    emailDigest: Boolean(notifs.value.emailDigest),
    reminderDays: Number(maintenance.value.reminderDays),
    defaultTechnicianId: Number(maintenance.value.defaultTechnicianId) || 0,
  }
}

async function persist(message) {
  try {
    await settingsStore.save(buildPayload())
    syncDrafts()
    showToast(message)
  } catch (error) {
    showToast(error.message || 'The settings could not be saved.')
  }
}

function saveDisplay() { return persist('Display preferences saved.') }
function resetDisplay() {
  const saved = currentSettings()
  display.value = { itemsPerPage: saved.itemsPerPage, dateFormat: saved.dateFormat }
  showToast('Unsaved display changes reset.')
}

function saveNotifications() { return persist('Notification preferences saved.') }
function resetNotifications() {
  const saved = currentSettings()
  notifs.value = {
    overdueAlerts: saved.overdueAlerts,
    upcomingAlerts: saved.upcomingAlerts,
    offlineAlerts: saved.offlineAlerts,
    emailDigest: saved.emailDigest,
  }
  showToast('Unsaved notification changes reset.')
}

function validateMaintenance() {
  const errors = {}
  const days = Number(maintenance.value.reminderDays)
  if (!days || days < 1 || days > 90) errors.reminderDays = 'Enter a value between 1 and 90.'
  maintenanceErrors.value = errors
  return Object.keys(errors).length === 0
}
function saveMaintenanceSettings() {
  if (!validateMaintenance()) return
  return persist('Maintenance reminder settings saved.')
}
function resetMaintenanceSettings() {
  const saved = currentSettings()
  maintenance.value = {
    reminderDays: saved.reminderDays,
    defaultTechnicianId: saved.defaultTechnicianId || 0,
  }
  maintenanceErrors.value = {}
  showToast('Unsaved maintenance changes reset.')
}
</script>

<style scoped>
.layout { display: flex; min-height: 100%; }
.layout__content { flex: 1; margin-left: var(--sidebar-width); display: flex; flex-direction: column; min-height: 100%; }
@media (max-width: 899px) { .layout__content { margin-left: 0; } }
.main { padding: 20px; padding-top: calc(var(--topnav-height) + 20px); display: flex; flex-direction: column; gap: 20px; }

.page-header { display: flex; align-items: flex-start; justify-content: space-between; gap: 12px; }
.page-title  { font-size: 20px; font-weight: 700; }
.page-sub    { font-size: 13px; color: var(--color-text-muted); margin-top: 2px; }

.settings-state { display: flex; align-items: center; gap: 10px; padding: 12px 16px; background: white; border: 1px solid var(--color-border); border-radius: var(--radius-sm); color: var(--color-text-muted); }
.settings-state--error { justify-content: space-between; color: var(--color-danger); border-color: #fca5a5; background: var(--color-danger-bg); }
.spinner { width: 20px; height: 20px; border: 2px solid var(--color-border); border-top-color: var(--color-primary); border-radius: 50%; animation: spin .7s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
.profile-note { margin-top: 4px; font-size: 12px; color: var(--color-text-muted); }

/* Toast */
.toast {
  position: fixed;
  bottom: 24px;
  right: 24px;
  background: #1e293b;
  color: white;
  padding: 12px 18px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13.5px;
  font-weight: 500;
  box-shadow: var(--shadow-lg);
  z-index: 2000;
}
.toast-enter-active, .toast-leave-active { transition: all .25s ease; }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(12px); }

.settings-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
@media (max-width: 900px) { .settings-grid { grid-template-columns: 1fr; } }

.settings-card {
  background: white;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 22px;
  box-shadow: var(--shadow-sm);
}

.settings-card__header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 18px;
  padding-bottom: 14px;
  border-bottom: 1px solid var(--color-border);
}
.settings-card__icon {
  width: 36px; height: 36px;
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-radius: var(--radius-sm);
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0;
}
.settings-card__title { font-size: 14px; font-weight: 600; }

.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; margin-bottom: 18px; }
@media (max-width: 560px) { .form-grid { grid-template-columns: 1fr; } }

.form-group { display: flex; flex-direction: column; gap: 5px; }
.form-label { font-size: 12.5px; font-weight: 600; }
.req { color: var(--color-danger); }

.form-input {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 8px 12px;
  font-size: 13.5px;
  color: var(--color-text);
  background: var(--color-bg);
  outline: none;
  width: 100%;
  transition: border-color .15s;
}
.form-input:focus { border-color: var(--color-primary); background: white; }
.form-input.error { border-color: var(--color-danger); }
.form-input:disabled { opacity: .75; cursor: not-allowed; }
.field-error { font-size: 12px; color: var(--color-danger); }

.input-with-hint { display: flex; align-items: center; gap: 8px; }
.input-with-hint .form-input { width: 80px; }
.input-hint { font-size: 13px; color: var(--color-text-muted); white-space: nowrap; }

.form-actions { display: flex; justify-content: flex-end; gap: 8px; }

.btn { display: inline-flex; align-items: center; gap: 6px; border: none; border-radius: var(--radius-sm); padding: 8px 16px; font-size: 13px; font-weight: 500; cursor: pointer; transition: all .15s; }
.btn--primary { background: var(--color-primary); color: white; }
.btn--primary:hover { background: var(--color-primary-hover); }
.btn--ghost { background: none; color: var(--color-text-muted); border: 1px solid var(--color-border); }
.btn--ghost:hover { background: var(--color-bg); }

/* Toggle switches */
.toggle-list { display: flex; flex-direction: column; gap: 0; margin-bottom: 18px; }
.toggle-row {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 0;
  border-bottom: 1px solid var(--color-border);
  cursor: pointer;
}
.toggle-row:last-child { border-bottom: none; }
.toggle-row__text { flex: 1; display: flex; flex-direction: column; gap: 2px; }
.toggle-row__label { font-size: 13.5px; font-weight: 500; }
.toggle-row__desc  { font-size: 12px; color: var(--color-text-muted); }
.toggle-cb { position: absolute; opacity: 0; width: 0; height: 0; pointer-events: none; }
.toggle-track {
  width: 40px; height: 22px;
  background: var(--color-border);
  border-radius: 11px;
  flex-shrink: 0;
  position: relative;
  transition: background .2s;
}
.toggle-track::after {
  content: '';
  position: absolute;
  top: 3px; left: 3px;
  width: 16px; height: 16px;
  background: white;
  border-radius: 50%;
  transition: transform .2s;
  box-shadow: 0 1px 3px rgba(0,0,0,.2);
}
.toggle-track--on { background: var(--color-primary); }
.toggle-track--on::after { transform: translateX(18px); }
</style>
