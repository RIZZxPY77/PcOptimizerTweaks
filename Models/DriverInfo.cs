namespace PCOptimizer.Models
{
    public class DriverInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string DeviceClass { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string Status { get; set; } = "OK"; // OK, Outdated, Warning
        public string DriverDate { get; set; } = string.Empty;
        
        public string StatusColor => Status == "OK" ? "#4CAF50" : (Status == "Outdated" ? "#FFC107" : "#F44336");
        public string StatusText => Status == "OK" ? "Aktif & Optimal" : (Status == "Outdated" ? "Update Tersedia" : "Peringatan");
    }
}
