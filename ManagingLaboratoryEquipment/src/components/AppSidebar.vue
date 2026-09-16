<template>
  <aside class="sidebar" :class="{ 'sidebar--open': modelValue }">
    <div class="sidebar__overlay" @click="$emit('update:modelValue', false)" />
    <div class="sidebar__panel">
      <div class="sidebar__logo">
        <svg width="28" height="28" viewBox="0 0 28 28" fill="none">
          <rect width="28" height="28" rx="7" fill="#4361ee"/>
          <path d="M7 14h14M14 7v14" stroke="white" stroke-width="2.5" stroke-linecap="round"/>
          <circle cx="14" cy="14" r="4" stroke="white" stroke-width="2"/>
        </svg>
        <span>EquipManager</span>
      </div>

      <nav class="sidebar__nav">
        <p class="sidebar__section-label">Main</p>
        <RouterLink to="/dashboard" class="sidebar__link" active-class="sidebar__link--active" @click="closeMobile">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/>
            <rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/>
          </svg>
          Dashboard
        </RouterLink>
        <RouterLink to="/equipment" class="sidebar__link" active-class="sidebar__link--active" @click="closeMobile">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 2H8a2 2 0 00-2 2v16a2 2 0 002 2h8a2 2 0 002-2V8l-6-6z"/>
            <polyline points="12 2 12 8 18 8"/>
          </svg>
          Equipment
        </RouterLink>

        <p class="sidebar__section-label" style="margin-top: 16px;">Reports</p>
        <RouterLink to="/analytics" class="sidebar__link" active-class="sidebar__link--active" @click="closeMobile">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="20" x2="18" y2="10"/><line x1="12" y1="20" x2="12" y2="4"/>
            <line x1="6" y1="20" x2="6" y2="14"/>
          </svg>
          Analytics
        </RouterLink>
        <RouterLink to="/maintenance" class="sidebar__link" active-class="sidebar__link--active" @click="closeMobile">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M22 12h-4l-3 9L9 3l-3 9H2"/>
          </svg>
          Maintenance Log
        </RouterLink>

        <p class="sidebar__section-label" style="margin-top: 16px;">System</p>
        <RouterLink to="/settings" class="sidebar__link" active-class="sidebar__link--active" @click="closeMobile">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="3"/>
            <path d="M19.07 4.93a10 10 0 010 14.14M4.93 4.93a10 10 0 000 14.14"/>
          </svg>
          Settings
        </RouterLink>
      </nav>

      <div class="sidebar__user">
        <div class="sidebar__avatar">{{ initials }}</div>
        <div class="sidebar__user-info">
          <span class="sidebar__user-name">{{ displayName }}</span>
          <span class="sidebar__user-role">{{ authStore.currentUser?.role || 'User' }}</span>
        </div>
      </div>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/authStore.js'

defineProps({ modelValue: { type: Boolean, default: false } })
const emit = defineEmits(['update:modelValue'])

const authStore = useAuthStore()
const displayName = computed(() => {
  const user = authStore.currentUser
  return [user?.firstName, user?.lastName].filter(Boolean).join(' ') || user?.email || 'Signed-in user'
})

const initials = computed(() => {
  const name = displayName.value || 'EU'
  return name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
})

function closeMobile() {
  emit('update:modelValue', false)
}
</script>

<style scoped>
.sidebar {
  position: fixed;
  inset: 0;
  z-index: 200;
  pointer-events: none;
}
.sidebar--open { pointer-events: auto; }

.sidebar__overlay {
  display: none;
  position: absolute;
  inset: 0;
  background: rgba(0,0,0,.45);
}

.sidebar__panel {
  position: absolute;
  top: 0;
  left: 0;
  bottom: 0;
  width: var(--sidebar-width);
  background: var(--color-sidebar);
  display: flex;
  flex-direction: column;
  padding: 0;
  transform: translateX(-100%);
  transition: transform .25s ease;
}

.sidebar--open .sidebar__panel { transform: translateX(0); }
.sidebar--open .sidebar__overlay { display: block; }

@media (min-width: 900px) {
  .sidebar {
    pointer-events: auto;
    position: fixed;
    inset-inline-end: unset;
    width: var(--sidebar-width);
  }
  .sidebar__overlay { display: none !important; }
  .sidebar__panel {
    transform: translateX(0);
    position: fixed;
    top: 0; left: 0; bottom: 0;
    width: var(--sidebar-width);
  }
}

.sidebar__logo {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 20px 20px 16px;
  color: white;
  font-size: 16px;
  font-weight: 700;
  letter-spacing: -.3px;
  border-bottom: 1px solid rgba(255,255,255,.08);
}

.sidebar__nav {
  flex: 1;
  padding: 16px 12px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.sidebar__section-label {
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: .8px;
  color: rgba(255,255,255,.35);
  padding: 8px 8px 4px;
}

.sidebar__link {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 9px 12px;
  border-radius: var(--radius-sm);
  color: rgba(255,255,255,.65);
  font-size: 13.5px;
  font-weight: 500;
  transition: background .15s, color .15s;
  text-decoration: none;
}
.sidebar__link:hover {
  background: var(--color-sidebar-hover);
  color: rgba(255,255,255,.9);
}
.sidebar__link--active {
  background: var(--color-sidebar-active) !important;
  color: white !important;
}

.sidebar__user {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 14px 16px;
  border-top: 1px solid rgba(255,255,255,.08);
}

.sidebar__avatar {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  background: var(--color-primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
  flex-shrink: 0;
}

.sidebar__user-info {
  display: flex;
  flex-direction: column;
  gap: 1px;
  overflow: hidden;
}
.sidebar__user-name {
  font-size: 13px;
  font-weight: 600;
  color: rgba(255,255,255,.9);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.sidebar__user-role {
  font-size: 11px;
  color: rgba(255,255,255,.4);
}
</style>
