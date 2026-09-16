namespace EquipmentManager.Domain
{
    public class MaintenanceRecords
    {
        public int MaintenanceRecordId { get; set; }
        public int EquipmentId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public int TechnicianId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
