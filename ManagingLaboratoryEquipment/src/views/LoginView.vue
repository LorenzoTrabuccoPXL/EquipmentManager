<template>
  <div class="login-page">
    <div class="login-box">
      <div class="login-logo">
        <svg width="36" height="36" viewBox="0 0 36 36" fill="none">
          <rect width="36" height="36" rx="9" fill="#4361ee"/>
          <path d="M9 18h18M18 9v18" stroke="white" stroke-width="3" stroke-linecap="round"/>
          <circle cx="18" cy="18" r="5" stroke="white" stroke-width="2.5"/>
        </svg>
        <span class="login-logo__text">EquipManager</span>
      </div>

      <h1 class="login-title">Welcome back</h1>
      <p class="login-subtitle">Sign in to manage your equipment inventory.</p>

      <div v-if="errorMsg" class="login-error">
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>
        </svg>
        {{ errorMsg }}
      </div>

      <form class="login-form" @submit.prevent="handleLogin" novalidate>
        <div class="form-group">
          <label class="form-label">Email address</label>
          <input
            v-model="email"
            type="email"
            class="form-input"
            :class="{ error: emailError }"
            placeholder="admin@company.com"
            autocomplete="username"
          />
          <span v-if="emailError" class="form-error">{{ emailError }}</span>
        </div>

        <div class="form-group">
          <label class="form-label">Password</label>
          <div class="password-wrap">
            <input
              v-model="password"
              :type="showPassword ? 'text' : 'password'"
              class="form-input"
              :class="{ error: passwordError }"
              placeholder="••••••••"
              autocomplete="current-password"
            />
            <button type="button" class="toggle-pw" @click="showPassword = !showPassword" tabindex="-1">
              <svg v-if="!showPassword" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>
              </svg>
              <svg v-else width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94"/>
                <path d="M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19"/>
                <line x1="1" y1="1" x2="23" y2="23"/>
              </svg>
            </button>
          </div>
          <span v-if="passwordError" class="form-error">{{ passwordError }}</span>
        </div>

        <button type="submit" class="login-btn" :disabled="loading">
          <span v-if="loading" class="login-spinner" />
          {{ loading ? 'Signing in…' : 'Sign in' }}
        </button>
      </form>

    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore.js'

const router = useRouter()
const authStore = useAuthStore()

const email         = ref('')
const password      = ref('')
const showPassword  = ref(false)
const loading       = ref(false)
const errorMsg      = ref('')
const emailError    = ref('')
const passwordError = ref('')

function validate() {
  emailError.value    = ''
  passwordError.value = ''
  let valid = true
  if (!email.value.trim()) {
    emailError.value = 'Email is required.'
    valid = false
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
    emailError.value = 'Enter a valid email address.'
    valid = false
  }
  if (!password.value) {
    passwordError.value = 'Password is required.'
    valid = false
  }
  return valid
}

async function handleLogin() {
  errorMsg.value = ''
  if (!validate()) return

  loading.value = true
  try {
    await authStore.login({
      email: email.value.trim(),
      password: password.value,
    })
    await router.push('/dashboard')
  } catch (error) {
    errorMsg.value = error.message || 'Unable to sign in. Check your credentials and try again.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100%;
  background: linear-gradient(135deg, #1a2035 0%, #2d3561 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}

.login-box {
  background: white;
  border-radius: var(--radius-lg);
  padding: 36px 40px;
  width: 100%;
  max-width: 420px;
  box-shadow: var(--shadow-lg);
}

.login-logo {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 28px;
}
.login-logo__text {
  font-size: 18px;
  font-weight: 700;
  color: var(--color-text);
}

.login-title {
  font-size: 22px;
  font-weight: 700;
  margin-bottom: 6px;
}
.login-subtitle {
  color: var(--color-text-muted);
  font-size: 14px;
  margin-bottom: 24px;
}

.login-error {
  display: flex;
  align-items: center;
  gap: 8px;
  background: var(--color-danger-bg);
  color: var(--color-danger);
  border: 1px solid #fca5a5;
  border-radius: var(--radius-sm);
  padding: 10px 14px;
  font-size: 13.5px;
  margin-bottom: 16px;
}

.login-form { display: flex; flex-direction: column; gap: 16px; }

.form-group { display: flex; flex-direction: column; gap: 6px; }
.form-label { font-size: 13px; font-weight: 600; }

.form-input {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  padding: 10px 14px;
  font-size: 14px;
  color: var(--color-text);
  background: var(--color-bg);
  outline: none;
  width: 100%;
  transition: border-color .15s;
}
.form-input:focus { border-color: var(--color-primary); background: white; }
.form-input.error { border-color: var(--color-danger); }

.form-error { font-size: 12px; color: var(--color-danger); }

.password-wrap { position: relative; }
.password-wrap .form-input { padding-right: 40px; }
.toggle-pw {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: var(--color-text-muted);
  cursor: pointer;
  padding: 2px;
  display: flex;
}
.toggle-pw:hover { color: var(--color-text); }

.login-btn {
  width: 100%;
  background: var(--color-primary);
  color: white;
  border: none;
  border-radius: var(--radius-sm);
  padding: 11px;
  font-size: 14.5px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: background .15s;
  margin-top: 4px;
}
.login-btn:hover:not(:disabled) { background: var(--color-primary-hover); }
.login-btn:disabled { opacity: .7; cursor: not-allowed; }

.login-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255,255,255,.4);
  border-top-color: white;
  border-radius: 50%;
  animation: spin .6s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

.login-hint {
  text-align: center;
  font-size: 12.5px;
  color: var(--color-text-muted);
  margin-top: 20px;
  background: var(--color-bg);
  border-radius: var(--radius-sm);
  padding: 10px;
}
</style>
