<template>
  <div class="card" :class="`card--${variant}`">
    <div class="card__icon">
      <slot name="icon" />
    </div>
    <div class="card__body">
      <span class="card__value">{{ value }}</span>
      <span class="card__label">{{ label }}</span>
    </div>
    <div class="card__trend" v-if="trend !== undefined">
      <svg v-if="trend >= 0" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <polyline points="18 15 12 9 6 15" />
      </svg>
      <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <polyline points="6 9 12 15 18 9" />
      </svg>
      <span>{{ Math.abs(trend) }}%</span>
    </div>
  </div>
</template>

<script setup>
defineProps({
  value:   { type: [Number, String], required: true },
  label:   { type: String, required: true },
  variant: { type: String, default: 'default' },
  trend:   { type: Number, default: undefined },
})
</script>

<style scoped>
.card {
  background: var(--color-card);
  border-radius: var(--radius-md);
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--color-border);
  transition: box-shadow .2s;
}
.card:hover { box-shadow: var(--shadow-md); }

.card__icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-sm);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.card--default .card__icon  { background: #eef1ff; color: var(--color-primary); }
.card--success .card__icon  { background: var(--color-success-bg); color: var(--color-success); }
.card--warning .card__icon  { background: var(--color-warning-bg); color: var(--color-warning); }
.card--danger  .card__icon  { background: var(--color-danger-bg);  color: var(--color-danger); }
.card--offline .card__icon  { background: var(--color-offline-bg); color: var(--color-offline); }

.card__body {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.card__value {
  font-size: 26px;
  font-weight: 700;
  line-height: 1;
  color: var(--color-text);
}
.card__label {
  font-size: 13px;
  color: var(--color-text-muted);
}

.card__trend {
  display: flex;
  align-items: center;
  gap: 2px;
  font-size: 12px;
  font-weight: 600;
  color: var(--color-success);
}
</style>
