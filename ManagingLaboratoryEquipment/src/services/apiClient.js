const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7051'
const API_BASE_URL = configuredBaseUrl.replace(/\/$/, '')

export class ApiError extends Error {
  constructor(message, status = 0, details = null) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.details = details
  }
}

function parseBody(text) {
  if (!text) return null
  try {
    return JSON.parse(text)
  } catch {
    return text
  }
}

function errorMessage(body, status) {
  if (typeof body === 'string' && body.trim()) return body
  return body?.detail || body?.title || body?.message || `Request failed with status ${status}.`
}

export async function apiRequest(path, options = {}) {
  const { method = 'GET', body, signal } = options
  const headers = { Accept: 'application/json' }
  const token = sessionStorage.getItem('ems_token')

  if (body !== undefined) headers['Content-Type'] = 'application/json'
  if (token) headers.Authorization = `Bearer ${token}`

  let response
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
      signal,
    })
  } catch (error) {
    throw new ApiError(
      'The API cannot be reached. Start the backend and check the API URL or HTTPS certificate.',
      0,
      error,
    )
  }

  const parsedBody = parseBody(await response.text())
  if (!response.ok) {
    throw new ApiError(errorMessage(parsedBody, response.status), response.status, parsedBody)
  }

  return parsedBody
}

export function createCrudService(resource) {
  const basePath = `/api/${resource}`
  return {
    getAll: () => apiRequest(`${basePath}/GetAll`),
    getById: id => apiRequest(`${basePath}/GetById/${id}`),
    add: payload => apiRequest(`${basePath}/Add`, { method: 'POST', body: payload }),
    update: payload => apiRequest(`${basePath}/Update`, { method: 'PUT', body: payload }),
    remove: id => apiRequest(`${basePath}/Delete/${id}`, { method: 'DELETE' }),
  }
}
