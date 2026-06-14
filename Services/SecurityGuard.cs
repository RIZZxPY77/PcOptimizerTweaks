using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Text;
using Microsoft.Win32;

namespace PCOptimizer.Services
{
    [System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true)]
    public static class SecurityGuard
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isPresent);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("ntdll.dll")]
        private static extern int NtSetInformationThread(IntPtr threadHandle, int threadInformationClass, IntPtr threadInformation, int threadInformationLength);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static Thread? _securityThread;
        private static bool _isRunning = false;

        // Comprehensive list of banned reverse engineering and dumping tools
        private static readonly string[] BannedProcesses = new string[]
        {
            "dnspy", "ilspy", "de4dot", "cheatengine", "ollydbg", "x64dbg", "x32dbg",
            "processhacker", "ida", "ida64", "megadumper", "dumpdumper", "hxd", 
            "wireshark", "fiddler", "httpdebugger", "scylla", "petools", "simpleassemblyexplorer",
            "systeminformer", "folderchangesview", "regshot", "cheat engine"
        };

        private static readonly string[] BannedWindowTitles = new string[]
        {
            "dnspy", "ilspy", "cheat engine", "ollydbg", "x64dbg", "x32dbg", "process hacker",
            "megadumper", "scylla", "wireshark", "fiddler", "simpleassemblyexplorer", "de4dot",
            "system informer", "reclass"
        };

        public static void Initialize()
        {
            // 1. Thread Hide From Debugger
            HideCurrentThread();

            // 2. Run Anti-Debug Checks
            CheckDebuggers();

            // 3. Start background monitoring thread
            StartSecurityMonitor();
        }

        private static void HideCurrentThread()
        {
            try
            {
                IntPtr hThread = GetCurrentThread();
                // ThreadHideFromDebugger = 0x11
                NtSetInformationThread(hThread, 0x11, IntPtr.Zero, 0);
            }
            catch {}
        }

        private static void CheckDebuggers()
        {
            // .NET Check
            if (Debugger.IsAttached)
            {
                ExitForSecurity("Debugger detected (CLR)");
            }

            // Win32 API Check
            if (IsDebuggerPresent())
            {
                ExitForSecurity("Debugger detected (Win32)");
            }

            // Remote Debugger Check
            bool isRemoteDebuggerPresent = false;
            try
            {
                Process process = Process.GetCurrentProcess();
                CheckRemoteDebuggerPresent(process.Handle, ref isRemoteDebuggerPresent);
            }
            catch {}

            if (isRemoteDebuggerPresent)
            {
                ExitForSecurity("Remote Debugger detected");
            }

            // COR Profiler Check (Detects tracer/patchers attaching to CLR)
            try
            {
                if (Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING") == "1")
                {
                    ExitForSecurity("Profiler detected (COR_PROFILER)");
                }
            }
            catch {}
        }

        private static void StartSecurityMonitor()
        {
            if (_isRunning) return;
            _isRunning = true;

            _securityThread = new Thread(SecurityLoop)
            {
                IsBackground = true,
                Name = "ClayxSecurityMonitor"
            };
            _securityThread.Start();
        }

        private static void SecurityLoop()
        {
            while (_isRunning)
            {
                try
                {
                    // 1. Re-check debuggers
                    CheckDebuggers();

                    // 2. Scan loaded modules (anti-injection)
                    ScanLoadedModules();

                    // 3. Scan window titles (anti-crack application)
                    ScanWindowTitles();

                    // 4. Scan running processes
                    var processes = Process.GetProcesses();
                    foreach (var proc in processes)
                    {
                        try
                        {
                            string procName = proc.ProcessName.ToLower();
                            foreach (var banned in BannedProcesses)
                            {
                                if (procName.Contains(banned))
                                {
                                    proc.Kill(); // Kill the banned process
                                    ExitForSecurity($"Unauthorized software detected: {proc.ProcessName}");
                                }
                            }
                        }
                        catch {}
                    }
                }
                catch {}

                Thread.Sleep(2000); // Check every 2 seconds
            }
        }

        private static void ScanLoadedModules()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                foreach (ProcessModule module in currentProcess.Modules)
                {
                    string moduleName = module.ModuleName?.ToLower() ?? "";
                    string modulePath = module.FileName?.ToLower() ?? "";

                    if (moduleName.Contains("cheatengine") || 
                        moduleName.Contains("easyhook") || 
                        moduleName.Contains("minhook") || 
                        moduleName.Contains("scylla") ||
                        modulePath.Contains("cheatengine"))
                    {
                        ExitForSecurity($"Suspicious module loaded: {module.ModuleName}");
                    }
                }
            }
            catch {}
        }

        private static void ScanWindowTitles()
        {
            EnumWindows((hWnd, lParam) =>
            {
                int length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    StringBuilder builder = new StringBuilder(length + 1);
                    GetWindowText(hWnd, builder, builder.Capacity);
                    string title = builder.ToString().ToLower();

                    foreach (var banned in BannedWindowTitles)
                    {
                        if (title.Contains(banned))
                        {
                            ExitForSecurity($"Banned window detected: {builder}");
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);
        }

        private static void ExitForSecurity(string reason)
        {
            MessageBox.Show($"Security Protection Alert!\n\nApplication terminated due to:\n{reason}", 
                "CLAYX SECURITY GUARD", MessageBoxButton.OK, MessageBoxImage.Stop);
            
            // Kill own process instantly
            Process.GetCurrentProcess().Kill();
            Environment.Exit(0);
        }
    }
}
