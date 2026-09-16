Update the existing Vue 3 application. Do not redesign, replace or recreate the application. Preserve the current visual style, layout, colours, icons, components and all existing functionality.

The sidebar items Analytics, Maintenance Log and Settings are currently not functional because their views do not exist. Create complete view files for these pages, add the necessary routes and fill every view with realistic, working content.

The notification button must also become fully functional. Its badge must react automatically to the actual number of unread notifications. Never hard-code the notification count.

## Technical requirements

- Continue using Vue 3 with Vite.
- Use JavaScript, not TypeScript.
- Use the Composition API with `<script setup>`.
- Use Vue Router for navigation.
- Use Pinia for shared application state.
- Use computed properties for derived values.
- Do not use React, ReactDOM, JSX, TSX or React hooks.
- Do not use placeholder links such as `href="#"`.
- Preserve all existing working functionality.
- Do not create empty views or pages containing only placeholder text.

## Required file structure

Create or update at least these files:

```text
src/
  main.js
  App.vue
  router/
    index.js
  views/
    LoginView.vue
    DashboardView.vue
    EquipmentView.vue
    EquipmentDetailView.vue
    AnalyticsView.vue
    MaintenanceLogView.vue
    SettingsView.vue
  stores/
    equipmentStore.js
    maintenanceStore.js
    notificationStore.js
    settingsStore.js
  components/
    AppSidebar.vue
    TopNavigation.vue
    SummaryCard.vue
    StatusBadge.vue
    EquipmentTable.vue
    MaintenanceTable.vue
    MaintenanceFormModal.vue
    NotificationDropdown.vue
```

Reuse existing files when they already provide the same functionality. Do not create duplicate components or stores.

## Vue Router

Configure these routes in `src/router/index.js`:

- `/login` → `LoginView.vue`
- `/dashboard` → `DashboardView.vue`
- `/equipment` → `EquipmentView.vue`
- `/equipment/:id` → `EquipmentDetailView.vue`
- `/analytics` → `AnalyticsView.vue`
- `/maintenance` → `MaintenanceLogView.vue`
- `/settings` → `SettingsView.vue`

Use `<RouterLink>` for every sidebar navigation item.

Requirements:

- Analytics must open `AnalyticsView.vue`.
- Maintenance Log must open `MaintenanceLogView.vue`.
- Settings must open `SettingsView.vue`.
- The selected navigation item must have an active visual state.
- Browser back and forward navigation must work.
- Directly opening or refreshing a route must work.
- Keep the existing sidebar design and icons.

## Pinia stores

### Equipment store

Create or update `src/stores/equipmentStore.js`.

It must manage:

- Equipment records
- Equipment statuses
- Selected equipment
- Searching and filtering
- Equipment summary statistics

Derived statistics must use Pinia getters or Vue computed properties.

### Maintenance store

Create `src/stores/maintenanceStore.js`.

It must manage:

- Maintenance records
- Upcoming maintenance
- Overdue maintenance
- Completed maintenance
- Creating and editing maintenance records
- Searching and filtering maintenance records

Every maintenance record must contain:

- Unique ID
- Equipment ID
- Maintenance type
- Scheduled date
- Technician
- Status
- Notes
- Created date

Maintenance records must be connected to actual equipment through the equipment ID.

### Notification store

Create `src/stores/notificationStore.js`.

Each notification must contain:

- Unique ID
- Title
- Message
- Type
- Related equipment or maintenance ID
- Target route
- Creation date and time
- `isRead` boolean

The unread notification count must be derived from the notification array.

Use a reactive Pinia getter or computed property equivalent to:

```js
const unreadCount = computed(
  () => notifications.value.filter(notification => !notification.isRead).length
)
```

The badge must display `unreadCount` directly. Never store a separate manually maintained notification counter and never hard-code the number 3.

When a notification is marked as read, removed or added, the displayed badge must update immediately without refreshing the page.

Create notifications from meaningful application data and events, including:

- Equipment with overdue maintenance
- Equipment with maintenance due within seven days
- Equipment whose status changes to unavailable
- A newly registered maintenance record when appropriate

Do not create duplicate notifications for the same equipment and maintenance event.

Provide actions for:

- `addNotification`
- `markAsRead`
- `markAllAsRead`
- `removeNotification`
- `synchronizeMaintenanceNotifications`

Run `synchronizeMaintenanceNotifications` after the equipment and maintenance data have loaded and after relevant maintenance data change.

Persist notification read states in `localStorage`, so notifications that were marked as read do not become unread again after refreshing the page.

### Settings store

Create `src/stores/settingsStore.js`.

It must manage:

- Profile preferences
- Display preferences
- Notification preferences
- Saved settings

Persist settings in `localStorage` without installing an unnecessary persistence package.

Keep temporary component state, such as whether a modal is visible, inside the relevant component instead of Pinia.

## Fill `AnalyticsView.vue`

Create a complete Analytics view using real data from the Pinia stores.

The page must contain:

- Page title and description
- Date-range filter
- Equipment-status filter
- Summary card for total equipment
- Summary card for active equipment
- Summary card for unavailable equipment
- Summary card for overdue maintenance
- Equipment distribution by status
- Monthly maintenance activity
- Recent activity section
- A simple bar or line chart
- Loading state
- Empty state
- Error state

All statistics and chart values must be calculated from the equipment and maintenance stores. Do not hard-code values that can be derived from application data.

Avoid installing a large chart library. Prefer a lightweight chart built with CSS or SVG.

## Fill `MaintenanceLogView.vue`

Create a complete and interactive Maintenance Log view.

The page must contain:

- Page title and description
- Search field
- Equipment filter
- Maintenance-status filter
- Date filter
- Maintenance table
- Button for registering maintenance
- Modal for adding maintenance
- Modal or form for editing maintenance
- Form validation
- Loading state
- Empty state
- Error state

The maintenance table must display:

- Equipment name
- Maintenance type
- Scheduled date
- Technician
- Status
- Notes
- Available actions

Adding or editing maintenance must update the maintenance Pinia store. The dashboard, analytics page and relevant notifications must then update automatically.

## Fill `SettingsView.vue`

Create a complete Settings view.

The page must contain:

- Profile settings
- Display preferences
- Notification preferences
- Email-notification toggle
- Maintenance-reminder toggle
- Number of days before a maintenance reminder
- Save button
- Reset button
- Form validation
- Success confirmation after saving

Bind the form fields to the settings store. Saved values must remain available after refreshing the application.

The maintenance notification synchronization must respect the saved notification preferences.

## Notification button and dropdown

Convert the notification bell into a real accessible button.

Create and use `NotificationDropdown.vue`.

When the bell is clicked:

- Open a dropdown underneath the button.
- Display the actual notifications from `notificationStore`.
- Sort notifications from newest to oldest.
- Clearly distinguish read and unread notifications.
- Display the creation date or relative time.
- Allow one notification to be marked as read.
- Provide a “Mark all as read” action.
- Allow notifications to be removed.
- Navigate to the notification’s target route when selected.
- Mark a selected notification as read.
- Show a clear empty state when there are no notifications.

The red badge must:

- Use the reactive `unreadCount` getter.
- Update immediately when notifications change.
- Be hidden when `unreadCount` equals zero.
- Display `9+` when more than nine notifications are unread.
- Never display a hard-coded value.

Also implement:

- Close the dropdown when clicking outside.
- Close it when pressing Escape.
- Add an appropriate `aria-label`.
- Support keyboard interaction.
- Keep the current bell design.

## Reactive synchronization

The views must remain synchronized through Pinia.

Examples:

- Adding a maintenance record updates the Maintenance Log immediately.
- Changing maintenance data updates Analytics automatically.
- Overdue maintenance creates an appropriate notification.
- Marking a notification as read immediately decreases the badge count.
- Marking all notifications as read immediately hides the badge.
- Changing notification settings affects which notifications are generated.
- Updating equipment information is reflected everywhere without refreshing.

Do not manually copy the same data into different components.

## Final verification

After implementing the changes, verify all of the following:

1. `AnalyticsView.vue`, `MaintenanceLogView.vue` and `SettingsView.vue` exist.
2. The three views contain complete and usable content.
3. The three sidebar items are clickable.
4. Every sidebar item opens the correct route.
5. The active sidebar item is highlighted.
6. The notification dropdown opens and closes correctly.
7. The notification list comes from Pinia.
8. The badge count equals the actual number of unread notifications.
9. The badge updates immediately after reading, adding or removing a notification.
10. Maintenance changes update the dashboard and Analytics automatically.
11. Settings and notification read states remain saved after refreshing.
12. There are no broken imports or duplicate stores.
13. There are no React dependencies, React imports, `.tsx` files or JSX syntax.
14. There are no console errors.
15. The application works with `npm install` and `npm run dev`.

After completing the update, provide a short summary of every file that was created or changed.