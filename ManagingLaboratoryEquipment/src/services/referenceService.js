import { createCrudService } from './apiClient.js'

export const categoriesService = createCrudService('Categories')
export const locationsService = createCrudService('Locations')
export const techniciansService = createCrudService('Technicians')
