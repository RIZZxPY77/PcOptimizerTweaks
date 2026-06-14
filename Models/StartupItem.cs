namespace PCOptimizer.Models
{
    public class StartupItem
    {
        public string Name { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty; // e.g. Software\Microsoft\Windows\CurrentVersion\Run
        public bool IsEnabled { get; set; }
        public string RegistryHive { get; set; } = "HKCU"; // HKCU or HKLM
        public string Impact { get; set; } = "Medium"; // Low, Medium, High
        
        public string ImpactColor => Impact == "High" ? "#F44336" : (Impact == "Medium" ? "#FFC107" : "#4CAF50");
        public string EnabledText => IsEnabled ? PCOptimizer.Services.TranslationService.Get("Emu_Active") : PCOptimizer.Services.TranslationService.Get("Emu_Inactive");

        public string LocalizedImpact
        {
            get
            {
                string key = "Start_" + Impact; // Start_High, Start_Medium, Start_Low
                string translatedImpact = PCOptimizer.Services.TranslationService.Get(key);
                string format = PCOptimizer.Services.TranslationService.Get("Start_Impact"); // "Dampak: {0}" or "Impact: {0}"
                return string.Format(format, translatedImpact);
            }
        }
    }
}
