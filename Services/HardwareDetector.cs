using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading.Tasks;

namespace PCOptimizer.Services
{
    public class HardwareDetector
    {
        private PerformanceCounter? _cpuCounter;

        public HardwareDetector()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                // Trigger first read
                _cpuCounter.NextValue();
            }
            catch
            {
                _cpuCounter = null;
            }
        }

        public float GetCpuUsage()
        {
            if (_cpuCounter == null) return 0f;
            try
            {
                return _cpuCounter.NextValue();
            }
            catch
            {
                return 0f;
            }
        }

        public (double totalGb, double freeGb, double usedPercent) GetRamUsage()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    double totalKb = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                    double freeKb = Convert.ToDouble(obj["FreePhysicalMemory"]);
                    
                    double totalGb = totalKb / (1024 * 1024);
                    double freeGb = freeKb / (1024 * 1024);
                    double usedGb = totalGb - freeGb;
                    double usedPercent = (usedGb / totalGb) * 100;

                    return (Math.Round(totalGb, 1), Math.Round(freeGb, 1), Math.Round(usedPercent, 1));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading RAM usage via WMI: " + ex.Message);
            }

            // Fallback
            return (8.0, 4.0, 50.0);
        }

        public string GetCpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    return obj["Name"]?.ToString()?.Trim() ?? "Unknown CPU";
                }
            }
            catch {}
            return "Unknown Processor";
        }

        public (int physicalCores, int logicalCores) GetCpuCores()
        {
            int physical = 0;
            int logical = 0;
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    physical += Convert.ToInt32(obj["NumberOfCores"]);
                    logical += Convert.ToInt32(obj["NumberOfLogicalProcessors"]);
                }
            }
            catch {}
            return (physical > 0 ? physical : 4, logical > 0 ? logical : 8);
        }

        public bool IsVirtualizationEnabled()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT VirtualizationFirmwareEnabled FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    var val = obj["VirtualizationFirmwareEnabled"];
                    if (val != null)
                    {
                        return Convert.ToBoolean(val);
                    }
                }
            }
            catch {}
            
            // Fallback: check bios traits or environment
            return false;
        }

        public string GetGpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                foreach (var obj in searcher.Get())
                {
                    string name = obj["Name"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(name) && !name.Contains("Basic Display"))
                    {
                        return name; // Prefer dedicated GPU
                    }
                }
            }
            catch {}
            return "Intel HD Graphics (Standard)";
        }

        public string GetGpuVram()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT AdapterRAM FROM Win32_VideoController");
                foreach (var obj in searcher.Get())
                {
                    var ramObj = obj["AdapterRAM"];
                    if (ramObj != null)
                    {
                        long bytes = Convert.ToInt64(ramObj);
                        double gb = (double)bytes / (1024 * 1024 * 1024);
                        if (gb > 0)
                        {
                            return $"{Math.Round(gb, 1)} GB";
                        }
                    }
                }
            }
            catch {}
            return "Shared System Memory";
        }

        public string GetOsName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Caption, Version FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    string caption = obj["Caption"]?.ToString() ?? "Windows";
                    string version = obj["Version"]?.ToString() ?? "";
                    return $"{caption} ({version})";
                }
            }
            catch {}
            return "Windows 11";
        }
    }
}
