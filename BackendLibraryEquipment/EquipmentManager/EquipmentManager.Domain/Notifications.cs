namespace EquipmentManager.Domain
{
    public class Notifications
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int EquipmentId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
