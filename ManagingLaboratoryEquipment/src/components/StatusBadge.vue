<template>
  <span class="badge" :class="badgeClass">{{ label }}</span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  status: { type: String, required: true },
})

const config = {
  active:      { label: 'Active',      cls: 'badge--active' },
  maintenance: { label: 'Maintenance', cls: 'badge--maintenance' },
  offline:     { label: 'Offline',     cls: 'badge--offline' },
}

const normalized = computed(() => String(props.status || '').toLowerCase())
const label = computed(() => config[normalized.value]?.label ?? props.status)
const badgeClass = computed(() => config[normalized.value]?.cls ?? '')
</script>

<style scoped>
.badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
}
.badge--active {
  color: var(--color-success);
  background: var(--color-success-bg);
}
.badge--maintenance {
  color: var(--color-warning);
  background: var(--color-warning-bg);
}
.badge--offline {
  color: var(--color-offline);
  background: var(--color-offline-bg);
}
</style>
