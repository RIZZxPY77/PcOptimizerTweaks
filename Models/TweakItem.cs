using System;

namespace PCOptimizer.Models
{
    public class TweakItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty; // e.g. "Gaming", "System"
        public string Description { get; set; } = string.Empty;
        public string SizeText { get; set; } = string.Empty; // e.g. "882 MB" or "11 KB"
        public string Category { get; set; } = string.Empty; // System, Gaming, Hardware, Recommended
        public string SubCategory { get; set; } = string.Empty; // e.g. "FPS & Latency", "Network & Ping"
        public bool IsActive { get; set; }
        public bool IsRecommended { get; set; }
        public bool HasDetails { get; set; } = true;
        
        // Redesign properties
        public string Icon { get; set; } = "Flash24";
        public string Status { get; set; } = "Siap Ditingkatkan";
        public string PerformanceEstimate { get; set; } = "+15% FPS";
    }
}
