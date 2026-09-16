<template>
  <header class="topnav">
    <button class="topnav__menu-btn" @click="$emit('toggle-sidebar')" aria-label="Toggle menu">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/>
      </svg>
    </button>

    <div class="topnav__search">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
      </svg>
      <input
        v-model="search"
        type="text"
        placeholder="Search equipment…"
        @input="$emit('search', search)"
      />
    </div>

    <div class="topnav__actions">
      <!-- Notification bell -->
      <div class="notif-wrap" ref="notifWrapRef">
        <button
          class="topnav__icon-btn"
          :class="{ 'topnav__icon-btn--active': dropdownOpen }"
          @click="toggleDropdown"
          title="Notifications"
          aria-label="Notifications"
        >
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 8A6 6 0 006 8c0 7-3 9-3 9h18s-3-2-3-9"/>
            <path d="M13.73 21a2 2 0 01-3.46 0"/>
          </svg>
          <span v-if="unreadCount > 0" class="topnav__badge">{{ unreadCount > 9 ? '9+' : unreadCount }}</span>
        </button>

        <!-- Dropdown -->
        <div v-if="dropdownOpen" class="notif-dropdown">
          <div class="notif-dropdown__header">
            <span class="notif-dropdown__title">Notifications</span>
            <button
              v-if="notifications.length > 0"
              class="notif-dropdown__mark-all"
              :disabled="notifStore.isUpdating || unreadCount === 0"
              @click="markAll"
            >Mark all read</button>
          </div>

          <div v-if="notifStore.isLoading" class="notif-dropdown__empty">
            <span class="notif-spinner" />
            <p>Loading notifications…</p>
          </div>

          <div v-else-if="notifStore.error" class="notif-dropdown__empty notif-dropdown__empty--error">
            <p>{{ notifStore.error }}</p>
            <button class="notif-dropdown__mark-all" @click="loadNotifications">Retry</button>
          </div>

          <div v-else-if="notifications.length === 0" class="notif-dropdown__empty">
            <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color:#94a3b8">
              <path d="M18 8A6 6 0 006 8c0 7-3 9-3 9h18s-3-2-3-9"/>
              <path d="M13.73 21a2 2 0 01-3.46 0"/>
            </svg>
            <p>No notifications</p>
          </div>

          <ul v-else class="notif-list">
            <li
              v-for="n in notifications"
              :key="n.notificationId"
              class="notif-item"
              :class="{ 'notif-item--unread': !n.isRead, [`notif-item--${severityClass(n.severity)}`]: true }"
            >
              <button class="notif-item__body" @click="handleNotifClick(n)">
                <span class="notif-item__dot" />
                <div class="notif-item__content">
                  <span class="notif-item__title">{{ n.title }}</span>
                  <span class="notif-item__msg">{{ n.message }}</span>
                  <span class="notif-item__time">{{ formatTime(n.createdAt) }}</span>
                </div>
              </button>
              <button class="notif-item__remove" :disabled="notifStore.isUpdating" @click.stop="removeNotification(n.notificationId)" title="Dismiss">
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                  <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
                </svg>
              </button>
            </li>
          </ul>
        </div>
      </div>

      <button class="topnav__logout" @click="logout">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4"/>
          <polyline points="16 17 21 12 16 7"/>
          <line x1="21" y1="12" x2="9" y2="12"/>
        </svg>
        Logout
      </button>
    </div>
  </header>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '../stores/notificationStore.js'
import { useAuthStore } from '../stores/authStore.js'

defineEmits(['toggle-sidebar', 'search'])
const router = useRouter()
const search = ref('')
const dropdownOpen = ref(false)
const notifWrapRef = ref(null)

const notifStore = useNotificationStore()
const authStore = useAuthStore()
const notifications = computed(() => notifStore.notifications)
const unreadCount   = computed(() => notifStore.unreadCount)

function toggleDropdown() {
  dropdownOpen.value = !dropdownOpen.value
}

async function handleNotifClick(n) {
  try {
    await notifStore.markRead(n.notificationId)
  } catch {
    return
  }
  if (n.equipmentId) router.push(`/equipment/${n.equipmentId}`)
  dropdownOpen.value = false
}

function loadNotifications() {
  return notifStore.fetchAll().catch(() => {})
}

function markAll() {
  return notifStore.markAllRead().catch(() => {})
}

function removeNotification(id) {
  return notifStore.remove(id).catch(() => {})
}

function severityClass(value) {
  const severity = String(value || '').toLowerCase()
  if (['danger', 'error', 'critical', 'high'].includes(severity)) return 'danger'
  return 'warning'
}

function handleOutsideClick(e) {
  if (notifWrapRef.value && !notifWrapRef.value.contains(e.target)) {
    dropdownOpen.value = false
  }
}

function handleKeydown(e) {
  if (e.key === 'Escape') dropdownOpen.value = false
}

function formatTime(iso) {
  if (!iso) return ''
  const d = new Date(iso)
  return d.toLocaleDateString('en-GB', { day: 'numeric', month: 'short' }) + ' ' +
    d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })
}

function logout() {
  authStore.logout()
  router.push('/login')
}

onMounted(() => {
  loadNotifications()
  document.addEventListener('click', handleOutsideClick, true)
  document.addEventListener('keydown', handleKeydown)
})
onBeforeUnmount(() => {
  document.removeEventListener('click', handleOutsideClick, true)
  document.removeEventListener('keydown', handleKeydown)
})
</script>

<style scoped>
.topnav {
  position: fixed;
  top: 0;
  left: var(--sidebar-width);
  right: 0;
  height: var(--topnav-height);
  background: var(--color-card);
  border-bottom: 1px solid var(--color-border);
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 0 20px;
  z-index: 100;
}
@media (max-width: 899px) { .topnav { left: 0; } }

.topnav__menu-btn {
  display: none;
  background: none;
  border: none;
  color: var(--color-text-muted);
  padding: 6px;
  border-radius: var(--radius-sm);
  flex-shrink: 0;
  cursor: pointer;
}
.topnav__menu-btn:hover { background: var(--color-bg); }
@media (max-width: 899px) { .topnav__menu-btn { display: flex; } }

.topnav__search {
  flex: 1;
  max-width: 380px;
  display: flex;
  align-items: center;
  gap: 8px;
  background: var(--color-bg);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 0 12px;
  height: 36px;
  color: var(--color-text-muted);
}
.topnav__search input {
  flex: 1;
  border: none;
  background: none;
  outline: none;
  font-size: 13.5px;
  color: var(--color-text);
}
.topnav__search input::placeholder { color: var(--color-text-light); }

.topnav__actions {
  margin-left: auto;
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Notification wrap */
.notif-wrap {
  position: relative;
}

.topnav__icon-btn {
  position: relative;
  background: none;
  border: none;
  color: var(--color-text-muted);
  padding: 7px;
  border-radius: var(--radius-sm);
  display: flex;
  cursor: pointer;
  transition: background .15s, color .15s;
}
.topnav__icon-btn:hover,
.topnav__icon-btn--active { background: var(--color-bg); color: var(--color-text); }

.topnav__badge {
  position: absolute;
  top: 2px;
  right: 2px;
  min-width: 16px;
  height: 16px;
  border-radius: 8px;
  background: var(--color-danger);
  color: white;
  font-size: 9px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 3px;
  pointer-events: none;
}

/* Dropdown */
.notif-dropdown {
  position: absolute;
  top: calc(100% + 8px);
  right: 0;
  width: 340px;
  background: white;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-lg);
  z-index: 500;
  overflow: hidden;
}

.notif-dropdown__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border-bottom: 1px solid var(--color-border);
}
.notif-dropdown__title { font-size: 13px; font-weight: 600; }
.notif-dropdown__mark-all {
  background: none;
  border: none;
  font-size: 12px;
  color: var(--color-primary);
  cursor: pointer;
  padding: 2px 4px;
}
.notif-dropdown__mark-all:hover { text-decoration: underline; }

.notif-dropdown__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 28px 16px;
  color: var(--color-text-muted);
  font-size: 13px;
}
.notif-dropdown__empty--error { color: var(--color-danger); text-align: center; }
.notif-spinner { width: 22px; height: 22px; border: 2px solid var(--color-border); border-top-color: var(--color-primary); border-radius: 50%; animation: notif-spin .7s linear infinite; }
@keyframes notif-spin { to { transform: rotate(360deg); } }

.notif-list {
  list-style: none;
  max-height: 360px;
  overflow-y: auto;
}

.notif-item {
  display: flex;
  align-items: flex-start;
  border-bottom: 1px solid var(--color-border);
  transition: background .1s;
}
.notif-item:last-child { border-bottom: none; }
.notif-item--unread { background: #fafbff; }
.notif-item:hover { background: var(--color-bg); }

.notif-item__body {
  flex: 1;
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 12px 12px 12px 14px;
  background: none;
  border: none;
  text-align: left;
  cursor: pointer;
  width: 100%;
}

.notif-item__dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
  margin-top: 4px;
}
.notif-item--warning .notif-item__dot { background: var(--color-warning); }
.notif-item--danger .notif-item__dot  { background: var(--color-danger); }
.notif-item--unread .notif-item__dot  { opacity: 1; }
.notif-item:not(.notif-item--unread) .notif-item__dot { opacity: .3; }

.notif-item__content { flex: 1; display: flex; flex-direction: column; gap: 2px; }
.notif-item__title { font-size: 12.5px; font-weight: 600; color: var(--color-text); }
.notif-item__msg { font-size: 12px; color: var(--color-text-muted); line-height: 1.4; }
.notif-item__time { font-size: 11px; color: var(--color-text-light); margin-top: 2px; }

.notif-item__remove {
  flex-shrink: 0;
  background: none;
  border: none;
  color: var(--color-text-light);
  padding: 12px 10px;
  cursor: pointer;
  display: flex;
  align-items: center;
  opacity: 0;
  transition: opacity .15s, color .15s;
}
.notif-item:hover .notif-item__remove { opacity: 1; }
.notif-item__remove:hover { color: var(--color-danger); }

.topnav__logout {
  display: flex;
  align-items: center;
  gap: 6px;
  background: none;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 6px 12px;
  font-size: 13px;
  color: var(--color-text-muted);
  cursor: pointer;
  transition: all .15s;
}
.topnav__logout:hover {
  background: var(--color-danger-bg);
  border-color: var(--color-danger);
  color: var(--color-danger);
}
</style>
