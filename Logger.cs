using System;
using System.IO;
using System.Text;

namespace PCOptimizer
{
    public static class Logger
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string StartupLog = Path.Combine(LogDirectory, "startup.log");
        private static readonly string CrashLog = Path.Combine(LogDirectory, "crash.log");

        static Logger()
        {
            try
            {
                if (!Directory.Exists(LogDirectory)) Directory.CreateDirectory(LogDirectory);
            }
            catch { /* ignore */ }
        }

        public static void Startup(string message)
        {
            Write(StartupLog, message);
        }

        public static void Crash(Exception ex, string source)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Source: {source}");
            sb.AppendLine(ex.ToString());
            sb.AppendLine(new string('-', 80));
            Write(CrashLog, sb.ToString());
        }

        private static void Write(string path, string content)
        {
            try
            {
                File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {content}{Environment.NewLine}");
            }
            catch { /* ignore */ }
        }
    }
}
