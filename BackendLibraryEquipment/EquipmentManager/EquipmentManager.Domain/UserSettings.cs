namespace EquipmentManager.Domain
{
    public class UserSettings
    {
        public int UserSettingsId { get; set; }
        public int ItemsPerPage { get; set; }
        public string DateFormat { get; set; } = string.Empty;
        public bool OverdueAlerts { get; set; }
        public bool UpcomingAlerts { get; set; }
        public bool OfflineAlerts { get; set; }
        public bool EmailDigest { get; set; }
        public int ReminderDays { get; set; }
        public int DefaultTechnicianId { get; set; }
    }
}
