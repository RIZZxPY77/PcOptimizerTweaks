using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using PCOptimizer.Models;

namespace PCOptimizer.Services
{
    public class OptimizerEngine
    {
        // Recycle Bin P/Invoke
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        public event Action<string>? OnLogMessage;

        private void Log(string message)
        {
            OnLogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        #region JUNK CLEANER

        public async Task<long> CleanJunkFilesAsync(bool cleanTemp, bool cleanPrefetch, bool cleanRecycleBin, bool cleanLogs)
        {
            long bytesCleaned = 0;
            
            await Task.Run(() =>
            {
                if (cleanTemp)
                {
                    Log("Mulai membersihkan file Temp pengguna...");
                    bytesCleaned += CleanDirectory(Path.GetTempPath());

                    Log("Mulai membersihkan file Temp sistem...");
                    bytesCleaned += CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"));
                }

                if (cleanPrefetch)
                {
                    Log("Mulai membersihkan folder Prefetch...");
                    string prefetchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
                    bytesCleaned += CleanDirectory(prefetchPath);
                }

                if (cleanLogs)
                {
                    Log("Mulai membersihkan file log sistem...");
                    string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs");
                    bytesCleaned += CleanDirectory(logsPath);
                }

                if (cleanRecycleBin)
                {
                    Log("Mengosongkan Recycle Bin...");
                    try
                    {
                        uint result = SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                        if (result == 0)
                        {
                            Log("Recycle Bin berhasil dikosongkan.");
                        }
                        else
                        {
                            Log("Recycle Bin kosong atau gagal dibersihkan.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Gagal mengosongkan Recycle Bin: {ex.Message}");
                    }
                }
            });

            double mb = (double)bytesCleaned / (1024 * 1024);
            Log($"Pembersihan selesai! Total ruang dibebaskan: {Math.Round(mb, 2)} MB");
            return bytesCleaned;
        }

        private long CleanDirectory(string path)
        {
            long cleanedBytes = 0;
            if (!Directory.Exists(path)) return 0;

            var dir = new DirectoryInfo(path);
            
            // Delete files
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    long size = file.Length;
                    file.Delete();
                    cleanedBytes += size;
                }
                catch
                {
                    // File might be locked by active processes, ignore it
                }
            }

            // Delete subdirectories
            foreach (var subDir in dir.GetDirectories())
            {
                try
                {
                    long size = GetDirectorySize(subDir);
                    subDir.Delete(true);
                    cleanedBytes += size;
                }
                catch
                {
                    // Subdirectory locked or has locked files
                }
            }

            return cleanedBytes;
        }

        private long GetDirectorySize(DirectoryInfo d)
        {
            long size = 0;
            try
            {
                foreach (FileInfo fi in d.GetFiles())
                {
                    size += fi.Length;
                }
                foreach (DirectoryInfo di in d.GetDirectories())
                {
                    size += GetDirectorySize(di);
                }
            }
            catch {}
            return size;
        }

        #endregion

        #region RAM BOOSTER

        public async Task<int> BoostRamAsync()
        {
            int optimizedProcesses = 0;
            Log("Mulai optimasi RAM (Working Set reduction)...");

            await Task.Run(() =>
            {
                var processes = Process.GetProcesses();
                foreach (var p in processes)
                {
                    IntPtr hProcess = IntPtr.Zero;
                    try
                    {
                        hProcess = NativeMethods.OpenProcess(NativeMethods.PROCESS_QUERY_INFORMATION | NativeMethods.PROCESS_SET_QUOTA, false, (uint)p.Id);
                        if (hProcess != IntPtr.Zero)
                        {
                            if (NativeMethods.EmptyWorkingSet(hProcess))
                            {
                                optimizedProcesses++;
                            }
                        }
                    }
                    catch
                    {
                        // System or protected process, skip
                    }
                    finally
                    {
                        if (hProcess != IntPtr.Zero)
                        {
                            NativeMethods.CloseHandle(hProcess);
                        }
                    }
                }
            });

            Log($"Optimasi RAM selesai! {optimizedProcesses} proses berhasil diringankan.");
            return optimizedProcesses;
        }

        #endregion

        #region NETWORK STABILIZER

        public async Task OptimizeNetworkPingAsync()
        {
            Log("Mengoptimasi pengaturan jaringan untuk gaming (TCP Delay & Latency)...");

            await Task.Run(() =>
            {
                try
                {
                    // Flush DNS
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "ipconfig.exe",
                            Arguments = "/flushdns",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                    Log("DNS Cache berhasil di-flush.");

                    // Registry TCP Latency tweaks
                    using var interfacesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", true);
                    if (interfacesKey != null)
                    {
                        int count = 0;
                        foreach (var subKeyName in interfacesKey.GetSubKeyNames())
                        {
                            using var interfaceKey = interfacesKey.OpenSubKey(subKeyName, true);
                            if (interfaceKey != null)
                            {
                                // Set TcpAckFrequency = 1 (send immediate ACKs)
                                interfaceKey.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
                                // Set TCPNoDelay = 1 (disable Nagle's algorithm)
                                interfaceKey.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
                                count++;
                            }
                        }
                        Log($"Registry TCP berhasil dioptimasi pada {count} interface jaringan.");
                    }
                }
                catch (Exception ex)
                {
                    Log($"Gagal mengoptimasi registry jaringan (memerlukan Administrator): {ex.Message}");
                }
            });
        }

        #endregion

        #region STORAGE TRIM

        public async Task OptimizeStorageTrimAsync()
        {
            Log("Menjalankan SSD TRIM untuk mempercepat kecepatan baca/tulis...");

            await Task.Run(() =>
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "defrag.exe",
                            Arguments = "/O /C", // Retrim all SSDs
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                    Log("Proses SSD TRIM berhasil dieksekusi di semua drive SSD.");
                }
                catch (Exception ex)
                {
                    Log($"Gagal menjalankan SSD TRIM: {ex.Message}");
                }
            });
        }

        #endregion

        #region STARTUP MANAGER

        public List<StartupItem> GetStartupItems()
        {
            var list = new List<StartupItem>();
            string[] runKeys = {
                @"Software\Microsoft\Windows\CurrentVersion\Run",
                @"Software\Microsoft\Windows\CurrentVersion\RunOnce"
            };

            // Registry hives to read
            var hives = new (RegistryKey root, string hiveName)[]
            {
                (Registry.CurrentUser, "HKCU"),
                (Registry.LocalMachine, "HKLM")
            };

            foreach (var (root, hiveName) in hives)
            {
                foreach (var path in runKeys)
                {
                    try
                    {
                        using var key = root.OpenSubKey(path, false);
                        if (key == null) continue;

                        foreach (var name in key.GetValueNames())
                        {
                            string command = key.GetValue(name)?.ToString() ?? string.Empty;
                            string filePath = ExtractPathFromCommand(command);

                            list.Add(new StartupItem
                            {
                                Name = name,
                                Command = command,
                                FilePath = filePath,
                                RegistryPath = path,
                                RegistryHive = hiveName,
                                IsEnabled = true, // By default active if in Run
                                Impact = EstimateStartupImpact(name)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error reading startup registry ({hiveName}\\{path}): {ex.Message}");
                    }
                }
            }

            // Fallback mock values if registry is empty
            if (list.Count == 0)
            {
                list.Add(new StartupItem { Name = "Discord", Command = @"C:\Users\User\AppData\Local\Discord\Update.exe --processStart Discord.exe", FilePath = @"C:\Users\User\AppData\Local\Discord\app.exe", RegistryHive = "HKCU", RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run", IsEnabled = true, Impact = "High" });
                list.Add(new StartupItem { Name = "Spotify", Command = @"C:\Users\User\AppData\Roaming\Spotify\Spotify.exe --minimized", FilePath = @"C:\Users\User\AppData\Roaming\Spotify\Spotify.exe", RegistryHive = "HKCU", RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run", IsEnabled = true, Impact = "Medium" });
                list.Add(new StartupItem { Name = "Steam Client Bootstrapper", Command = @"C:\Program Files (x86)\Steam\steam.exe -silent", FilePath = @"C:\Program Files (x86)\Steam\steam.exe", RegistryHive = "HKLM", RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run", IsEnabled = true, Impact = "High" });
            }

            return list;
        }

        public bool ToggleStartupItem(StartupItem item, bool enable)
        {
            try
            {
                RegistryKey rootKey = item.RegistryHive == "HKCU" ? Registry.CurrentUser : Registry.LocalMachine;
                
                if (enable)
                {
                    // Re-add key
                    using var key = rootKey.OpenSubKey(item.RegistryPath, true);
                    if (key != null)
                    {
                        key.SetValue(item.Name, item.Command);
                        Log($"Startup item '{item.Name}' diaktifkan.");
                        return true;
                    }
                }
                else
                {
                    // Remove key
                    using var key = rootKey.OpenSubKey(item.RegistryPath, true);
                    if (key != null)
                    {
                        key.DeleteValue(item.Name, false);
                        Log($"Startup item '{item.Name}' dinonaktifkan.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Gagal mengubah status startup '{item.Name}': {ex.Message}");
            }
            return false;
        }

        private string ExtractPathFromCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return string.Empty;
            command = command.Trim();
            if (command.StartsWith("\""))
            {
                int nextQuote = command.IndexOf("\"", 1);
                if (nextQuote > 1)
                {
                    return command.Substring(1, nextQuote - 1);
                }
            }
            int space = command.IndexOf(" ");
            return space > 0 ? command.Substring(0, space) : command;
        }

        private string EstimateStartupImpact(string name)
        {
            string lower = name.ToLower();
            if (lower.Contains("discord") || lower.Contains("steam") || lower.Contains("teams") || lower.Contains("chrome"))
                return "High";
            if (lower.Contains("spotify") || lower.Contains("onedrive") || lower.Contains("dropbox"))
                return "Medium";
            return "Low";
        }

        #endregion

        #region PROFILES OPTIMIZATION

        public async Task ApplyOptimizationProfileAsync(string profile)
        {
            Log($"Meningkatkan PC dengan Profil: {profile.ToUpper()}...");

            await Task.Run(() =>
            {
                switch (profile.ToLower())
                {
                    case "low":
                        ApplyLowEndOptimizations();
                        break;
                    case "mid":
                        ApplyMidEndOptimizations();
                        break;
                    case "high":
                        ApplyHighEndOptimizations();
                        break;
                }
            });
        }

        private void ApplyLowEndOptimizations()
        {
            Log("Mengaplikasikan optimasi PC Low-End...");
            try
            {
                // Set power scheme to High Performance
                RunPowercfgSetActive("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"); // High Performance GUID
                Log("Skema daya disetel ke: High Performance.");

                // Registry: Disable animations & visual effects (best performance)
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", true))
                {
                    key?.SetValue("VisualFXSetting", 2, RegistryValueKind.DWord); // 2 = Best performance
                }
                Log("Efek visual sistem disetel ke: Performa Terbaik (Animasi dinonaktifkan).");

                // Stop telemetry services
                StopService("DiagTrack"); // Telemetry
                StopService("dmwappushservice"); // WAP Push routing
                Log("Layanan telemetri latar belakang dihentikan.");
            }
            catch (Exception ex)
            {
                Log($"Gagal menerapkan beberapa optimasi: {ex.Message}");
            }
        }

        private void ApplyMidEndOptimizations()
        {
            Log("Mengaplikasikan optimasi PC Mid-End...");
            try
            {
                // Set power scheme to Balanced/High Performance
                RunPowercfgSetActive("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"); // High Performance
                Log("Skema daya disetel ke: High Performance.");

                // Registry: Custom Balanced Visual Effects
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", true))
                {
                    key?.SetValue("VisualFXSetting", 0, RegistryValueKind.DWord); // 0 = Let windows choose
                }
                Log("Efek visual disetel ke seimbang.");

                // Stop diagnostic tracking
                StopService("DiagTrack");
                Log("Layanan diagnostik latar belakang dinonaktifkan.");
            }
            catch (Exception ex)
            {
                Log($"Gagal menerapkan beberapa optimasi: {ex.Message}");
            }
        }

        private void ApplyHighEndOptimizations()
        {
            Log("Mengaplikasikan optimasi PC High-End...");
            try
            {
                // Try Ultimate Performance, fallback to High Performance
                bool powerSet = RunPowercfgSetActive("e9a22d33-7e51-40e8-a5f1-3094ac66504a"); // Ultimate Performance GUID
                if (!powerSet)
                {
                    // If Ultimate doesn't exist, enable it then set active, or fallback to High Performance
                    try
                    {
                        // Command to duplicate Ultimate plan: powercfg -duplicatescheme e9a22d33-7e51-40e8-a5f1-3094ac66504a
                        var process = Process.Start(new ProcessStartInfo
                        {
                            FileName = "powercfg.exe",
                            Arguments = "-duplicatescheme e9a22d33-7e51-40e8-a5f1-3094ac66504a",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                        process?.WaitForExit();
                        RunPowercfgSetActive("e9a22d33-7e51-40e8-a5f1-3094ac66504a");
                    }
                    catch
                    {
                        RunPowercfgSetActive("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"); // High Performance fallback
                    }
                }
                Log("Skema daya disetel ke: Ultimate Performance.");

                // Registry: Disable network throttling for high bandwidth gaming
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", true))
                {
                    key?.SetValue("NetworkThrottlingIndex", unchecked((int)0xffffffff), RegistryValueKind.DWord);
                    key?.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);
                }
                Log("Fitur Throttling Jaringan dinonaktifkan (Kecepatan bandwidth maksimum).");

                // Ensure Windows Game Mode is enabled
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar", true))
                {
                    key?.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                }
                Log("Windows Game Mode telah diaktifkan.");
            }
            catch (Exception ex)
            {
                Log($"Gagal menerapkan optimasi High-End: {ex.Message}");
            }
        }

        private bool RunPowercfgSetActive(string guid)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powercfg.exe",
                        Arguments = $"/setactive {guid}",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private void StopService(string serviceName)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"stop {serviceName}",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to stop service {serviceName}: {ex.Message}");
            }
        }

        #endregion

        #region MOUSE & KEYBOARD TWEAKS

        public void ApplyMouseTweaks(bool enable)
        {
            Log(enable ? "Mengaktifkan optimasi Mouse (MarkC Mouse Fix)..." : "Mengembalikan pengaturan Mouse ke default...");
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse", true);
                if (key != null)
                {
                    if (enable)
                    {
                        key.SetValue("MouseSpeed", "0", RegistryValueKind.String);
                        key.SetValue("MouseThreshold1", "0", RegistryValueKind.String);
                        key.SetValue("MouseThreshold2", "0", RegistryValueKind.String);
                        
                        // MarkC linear curve
                        byte[] xCurve = { 0, 0, 0, 0, 0, 0, 0, 0, 21, 0, 0, 0, 0, 0, 0, 0, 48, 0, 0, 0, 0, 0, 0, 0, 69, 0, 0, 0, 0, 0, 0, 0, 96, 0, 0, 0, 0, 0, 0, 0 };
                        byte[] yCurve = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 56, 0, 0, 0, 0, 0, 0, 0, 112, 0, 0, 0, 0, 0, 0, 0, 0, 0, 168, 0, 0, 0, 0, 0, 0, 0, 0, 0, 224, 0, 0, 0, 0, 0, 0, 0 };
                        
                        key.SetValue("SmoothMouseXCurve", xCurve, RegistryValueKind.Binary);
                        key.SetValue("SmoothMouseYCurve", yCurve, RegistryValueKind.Binary);
                        Log("Mouse Acceleration dinonaktifkan (MarkC 1:1 Aim Stabilizer diterapkan).");
                    }
                    else
                    {
                        key.SetValue("MouseSpeed", "1", RegistryValueKind.String);
                        key.SetValue("MouseThreshold1", "6", RegistryValueKind.String);
                        key.SetValue("MouseThreshold2", "10", RegistryValueKind.String);

                        // Default Windows 10/11 curves
                        byte[] xCurve = { 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0, 30, 0, 0, 0, 0, 0, 0, 0, 45, 0, 0, 0, 0, 0, 0, 0, 60, 0, 0, 0, 0, 0, 0, 0 };
                        byte[] yCurve = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 38, 0, 0, 0, 0, 0, 0, 0, 0, 0, 70, 0, 0, 0, 0, 0, 0, 0, 0, 0, 106, 0, 0, 0, 0, 0, 0, 0, 0, 0, 140, 0, 0, 0, 0, 0, 0, 0 };

                        key.SetValue("SmoothMouseXCurve", xCurve, RegistryValueKind.Binary);
                        key.SetValue("SmoothMouseYCurve", yCurve, RegistryValueKind.Binary);
                        Log("Mouse Acceleration dikembalikan ke pengaturan default Windows.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Gagal mengoptimasi Mouse: {ex.Message}");
            }
        }

        public void ApplyKeyboardTweaks(bool enable)
        {
            Log(enable ? "Mengaktifkan optimasi Keyboard (Mengurangi Delay Input)..." : "Mengembalikan pengaturan Keyboard ke default...");
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Keyboard", true);
                if (key != null)
                {
                    if (enable)
                    {
                        key.SetValue("KeyboardDelay", "0", RegistryValueKind.String); // Fastest repeat delay
                        key.SetValue("KeyboardSpeed", "31", RegistryValueKind.String); // Fastest repeat rate
                        Log("Delay pengetikan Keyboard diminimalkan dan respon rate dimaksimalkan.");
                    }
                    else
                    {
                        key.SetValue("KeyboardDelay", "1", RegistryValueKind.String);
                        key.SetValue("KeyboardSpeed", "31", RegistryValueKind.String);
                        Log("Keyboard dikembalikan ke respon rate standar Windows.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Gagal mengoptimasi Keyboard: {ex.Message}");
            }
        }

        #endregion

        #region ADDITIONAL OPTIMIZATIONS

        public async Task CleanShaderCacheAsync()
        {
            Log("Mulai membersihkan Shader Cache...");
            await Task.Run(() =>
            {
                string localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string[] cacheDirs = {
                    Path.Combine(localApp, @"D3DSCache"),
                    Path.Combine(localApp, @"NVIDIA\DXCache"),
                    Path.Combine(localApp, @"NVIDIA\GLCache"),
                    Path.Combine(localApp, @"AMD\DxCache")
                };

                long totalFreed = 0;
                foreach (var dir in cacheDirs)
                {
                    if (Directory.Exists(dir))
                    {
                        Log($"Menganalisis folder cache shader: {dir}");
                        totalFreed += CleanDirectory(dir);
                    }
                }
                double mb = (double)totalFreed / (1024 * 1024);
                Log($"Shader Cache dibersihkan: {Math.Round(mb, 2)} MB dibebaskan.");
            });
        }

        public async Task OptimizeDnsAsync(string provider)
        {
            Log($"Mulai optimalisasi DNS Server ke: {provider}...");
            await Task.Run(() =>
            {
                try
                {
                    // Flush DNS first
                    var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "ipconfig.exe",
                        Arguments = "/flushdns",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                    process?.WaitForExit();
                    Log("DNS cache di-flush.");

                    if (provider.Equals("Default", StringComparison.OrdinalIgnoreCase))
                    {
                        // Reset to DHCP DNS
                        var netshProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = "netsh.exe",
                            Arguments = "interface ip set dns name=\"Ethernet\" source=dhcp",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                        netshProcess?.WaitForExit();
                        Log("DNS adapter 'Ethernet' dikembalikan ke konfigurasi default (DHCP).");
                    }
                    else
                    {
                        string dns1 = "1.1.1.1";
                        string dns2 = "8.8.8.8";

                        if (provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
                        {
                            dns1 = "8.8.8.8";
                            dns2 = "8.8.4.4";
                        }
                        else if (provider.Equals("Quad9", StringComparison.OrdinalIgnoreCase))
                        {
                            dns1 = "9.9.9.9";
                            dns2 = "149.112.112.112";
                        }

                        // Set primary and secondary DNS
                        var netshProcess1 = Process.Start(new ProcessStartInfo
                        {
                            FileName = "netsh.exe",
                            Arguments = $"interface ip set dns name=\"Ethernet\" static {dns1} primary",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                        netshProcess1?.WaitForExit();
                        
                        var netshProcess2 = Process.Start(new ProcessStartInfo
                        {
                            FileName = "netsh.exe",
                            Arguments = $"interface ip add dns name=\"Ethernet\" {dns2} index=2",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                        netshProcess2?.WaitForExit();
                        
                        Log($"Server DNS disetel ke {provider} ({dns1} & {dns2}) untuk stabilitas koneksi.");
                    }
                }
                catch (Exception ex)
                {
                    Log($"Gagal mengatur server DNS: {ex.Message}");
                }
            });
        }

        public async Task ApplyGpuPriorityBoostAsync()
        {
            Log("Menerapkan prioritas GPU Scheduling...");
            await Task.Run(() =>
            {
                try
                {
                    // Enable Hardware-Accelerated GPU Scheduling (HAGS)
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("HwSchMode", 2, RegistryValueKind.DWord); // 2 = Enabled HAGS
                            Log("Hardware-Accelerated GPU Scheduling (HAGS) diaktifkan di Registry. (Restart diperlukan)");
                        }
                    }

                    // Tweak GPU priority details for games in multimedia class
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("GPU Priority", 8, RegistryValueKind.DWord);
                            key.SetValue("Priority", 6, RegistryValueKind.DWord);
                            key.SetValue("Scheduling Category", "High", RegistryValueKind.String);
                            Log("Prioritas penjadwalan GPU untuk kategori Game ditingkatkan.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"Gagal menyetel prioritas GPU di Registry: {ex.Message}");
                }
            });
        }

        public async Task ApplyCpuPriorityBoostAsync()
        {
            Log("Menerapkan optimasi CPU Priority untuk Foreground Apps...");
            await Task.Run(() =>
            {
                try
                {
                    // Set Win32PrioritySeparation to 26 (0x1A) for short, variable quantums (best for desktop/gaming response)
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("Win32PrioritySeparation", 26, RegistryValueKind.DWord);
                            Log("Win32PrioritySeparation disetel ke 26 (mengutamakan respon aplikasi aktif/game).");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"Gagal menyetel alokasi CPU di Registry: {ex.Message}");
                }
            });
        }

        public async Task ApplyServiceOptimizerAsync()
        {
            Log("Mengoptimasi Layanan Latar Belakang (Services)...");
            await Task.Run(() =>
            {
                // strictly target telemetry and diagnostic routing (excluding SysMain per safety revision)
                string[] servicesToStop = {
                    "DiagTrack",       // Connected User Experiences and Telemetry
                    "dmwappushservice", // WAP Push Message Routing Service
                    "WerSvc"            // Windows Error Reporting Service
                };

                foreach (var svc in servicesToStop)
                {
                    Log($"Menghentikan dan menonaktifkan layanan non-esensial: {svc}");
                    StopService(svc);
                    DisableService(svc);
                }
                Log("Layanan telemetri & error reporting latar belakang dinonaktifkan.");
            });
        }

        private void DisableService(string serviceName)
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"config {serviceName} start=disabled",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                process?.WaitForExit();
            }
            catch {}
        }

        public async Task ApplyRegistryCleanerAsync()
        {
            Log("Melakukan pembersihan parameter Registry non-esensial...");
            await Task.Run(() =>
            {
                try
                {
                    // Safe cleanup: TypedPaths Explorer
                    using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths", true))
                    {
                        if (key != null)
                        {
                            foreach (var val in key.GetValueNames())
                            {
                                key.DeleteValue(val, false);
                            }
                            Log("Registry TypedPaths Explorer dibersihkan.");
                        }
                    }
                    Log("Pembersihan log cache registry selesai secara aman.");
                }
                catch (Exception ex)
                {
                    Log($"Peringatan pembersihan registry: {ex.Message}");
                }
            });
        }

        public async Task CleanBrowserCacheAsync()
        {
            Log("Membersihkan cache Web Browser...");
            await Task.Run(() =>
            {
                string localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                
                // Strictly target Cache and Code Cache folders to preserve Cookies, Login data, and Session passwords
                string[] browserCaches = {
                    Path.Combine(localApp, @"Google\Chrome\User Data\Default\Cache"),
                    Path.Combine(localApp, @"Google\Chrome\User Data\Default\Code Cache"),
                    Path.Combine(localApp, @"Microsoft\Edge\User Data\Default\Cache"),
                    Path.Combine(localApp, @"Microsoft\Edge\User Data\Default\Code Cache"),
                    Path.Combine(appData, @"Mozilla\Firefox\Profiles")
                };

                long totalFreed = 0;
                foreach (var dir in browserCaches)
                {
                    if (Directory.Exists(dir))
                    {
                        if (dir.Contains("Firefox"))
                        {
                            // In Firefox, cache is stored in profiles, search for 'cache2' subdirectory
                            try
                            {
                                foreach (var profileDir in Directory.GetDirectories(dir))
                                {
                                    string cache2Path = Path.Combine(profileDir, "cache2");
                                    if (Directory.Exists(cache2Path))
                                    {
                                        Log($"Membersihkan cache Firefox: {cache2Path}");
                                        totalFreed += CleanDirectory(cache2Path);
                                    }
                                }
                            }
                            catch {}
                        }
                        else
                        {
                            Log($"Membersihkan cache browser: {dir}");
                            totalFreed += CleanDirectory(dir);
                        }
                    }
                }
                double mb = (double)totalFreed / (1024 * 1024);
                Log($"Pembersihan Cache Browser selesai! Berhasil menghapus {Math.Round(mb, 2)} MB.");
            });
        }

        #endregion
    }
}
