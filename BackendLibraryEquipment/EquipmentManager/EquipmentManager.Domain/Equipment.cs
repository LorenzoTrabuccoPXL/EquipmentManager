namespace EquipmentManager.Domain
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int LocationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public DateTime WarrantyExpiry { get; set; }
        public DateTime LastMaintenance { get; set; }
        public DateTime NextMaintenance { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
