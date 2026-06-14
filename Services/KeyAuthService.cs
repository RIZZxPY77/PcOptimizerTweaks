using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;




namespace PCOptimizer.Services
{
    // Simple file logger used for debugging KeyAuth interactions
    internal static class Logger
    {
        internal static void Log(string message)
        {
            try
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keyauth_debug.log");
                File.AppendAllText(logPath, $"{DateTime.Now:u} {message}\n");
            }
            catch { /* ignore logging failures */ }
        }
    }

    // The official KeyAuth API communication class
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class api
    {
        public string name { get; }
        public string ownerid { get; }
        public string version { get; }

        private string? _sessionId;
        public string? sessionid => _sessionId;
        private static readonly HttpClient _client = new HttpClient();

        public api(string name, string ownerid, string version, string? path = null)
        {
            this.name = name;
            this.ownerid = ownerid;
            this.version = version;
        }

        public async Task<KeyAuthResponse> init()
        {
            try
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "type", "init" },
                    { "ver", version },
                    { "name", name },
                    { "ownerid", ownerid }
                });
                // Optional: set a User-Agent header
                if (!_client.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    _client.DefaultRequestHeaders.Add("User-Agent", "PCOptimizer/1.0");
                }
                var response = await _client.PostAsync("https://keyauth.win/api/1.2/", content);
                var json = await response.Content.ReadAsStringAsync();
                // Log raw response for debugging
                Logger.Log($"[Init] HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {json}");
                var result = JsonConvert.DeserializeObject<KeyAuthResponse>(json);
                if (result != null && result.success)
                {
                    _sessionId = result.sessionid;
                }
                else if (result != null)
                {
                    Logger.Log($"[Init] Failure message: {result.message}");
                }
                return result ?? new KeyAuthResponse { success = false, message = "Gagal memproses data inisialisasi." };
            }
            catch (Exception ex)
            {
                Logger.Log($"[Init] Exception: {ex.Message}");
                return new KeyAuthResponse { success = false, message = "Koneksi KeyAuth gagal: " + ex.Message };
            }
        }

        public async Task<KeyAuthResponse> license(string key, string hwid)
        {
            try
            {
                if (string.IsNullOrEmpty(_sessionId))
                {
                    Logger.Log("[License] No session ID – init not done.");
                    return new KeyAuthResponse { success = false, message = "Aplikasi belum terinisialisasi. Silakan restart." };
                }
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "type", "license" },
                    { "key", key },
                    { "hwid", hwid },
                    { "sessionid", _sessionId },
                    { "name", name },
                    { "ownerid", ownerid },
                    { "ver", version }
                });
                if (!_client.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    _client.DefaultRequestHeaders.Add("User-Agent", "PCOptimizer/1.0");
                }
                var response = await _client.PostAsync("https://keyauth.win/api/1.2/", content);
                var json = await response.Content.ReadAsStringAsync();
                Logger.Log($"[License] HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {json}");
                var result = JsonConvert.DeserializeObject<KeyAuthResponse>(json);
                return result ?? new KeyAuthResponse { success = false, message = "Gagal memproses data login." };
            }
            catch (Exception ex)
            {
                Logger.Log($"[License] Exception: {ex.Message}");
                return new KeyAuthResponse { success = false, message = "Koneksi login gagal: " + ex.Message };
            }
        }
    }

    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class KeyAuthResponse
    {
        public bool success { get; set; }
        public string? sessionid { get; set; }
        public string? message { get; set; }
        public UserData? info { get; set; }
    }

    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class UserData
    {
        public string? username { get; set; }
        public string? hwid { get; set; }
        public List<SubscriptionData>? subscriptions { get; set; }
        public string? ip { get; set; }
    }

    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class SubscriptionData
    {
        public string? subscription { get; set; }
        public string? expiry { get; set; }
        public string? timeleft { get; set; }
    }

    // License information holder
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class LicenseInfo
    {
        public required string Key { get; set; }
        public required DateTime Expiry { get; set; }
        public required string HWID { get; set; }
        public required string IP { get; set; }
        public required string PCName { get; set; }
        public TimeSpan Duration => Expiry - DateTime.Now;
    }

    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public static class KeyAuthService
    {
        public static api KeyAuthApp;

        private static string Decrypt(byte[] data, byte key = 0x5A)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key);
            }
            return System.Text.Encoding.ASCII.GetString(result);
        }

        static KeyAuthService()
        {
            // Direct initialization using provided credentials (no obfuscation)
            KeyAuthApp = new api(
                name: "optimizepc",
                ownerid: "GLCBimsCfU",
                version: "1.0"
            );
            // Ensure log file exists
            try
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keyauth_error.log");
                if (!File.Exists(logPath)) File.Create(logPath).Dispose();
            }
            catch { }
        }

        // Holds the logged‑in license info
        public static LicenseInfo? CurrentLicense { get; private set; }

        // Stores the error message from the last operation
        public static string LastErrorMessage { get; private set; } = string.Empty;

        public static async Task<bool> InitializeAsync()
        {
            var result = await KeyAuthApp.init();
            if (!result.success)
            {
                LastErrorMessage = result.message ?? "Gagal inisialisasi KeyAuth.";
                // Write error to log file for diagnostics
                try
                {
                    var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keyauth_error.log");
                    File.AppendAllText(logPath, $"[Init Failed] {DateTime.Now}: {LastErrorMessage}\n");
                }
                catch { }
                return false;
            }
            return true;
        }

        public static async Task<bool> LoginAsync(string licenseKey)
        {
            LastErrorMessage = string.Empty;
            var hwid = GetMachineGuid();
            var result = await KeyAuthApp.license(licenseKey, hwid);

            if (result.success && result.info != null)
            {
                long expirySeconds = 0;
                if (result.info.subscriptions != null && result.info.subscriptions.Count > 0)
                {
                    long.TryParse(result.info.subscriptions[0].expiry, out expirySeconds);
                }

                // Populate license info from server response
                CurrentLicense = new LicenseInfo
                {
                    Key = licenseKey,
                    Expiry = expirySeconds > 0 ? DateTimeOffset.FromUnixTimeSeconds(expirySeconds).DateTime.ToLocalTime() : DateTime.Now.AddDays(30),
                    HWID = result.info.hwid ?? hwid,
                    IP = result.info.ip ?? GetLocalIP(),
                    PCName = Environment.MachineName
                };
                return true;
            }
            else
            {
                LastErrorMessage = result.message ?? "Lisensi tidak valid.";
                return false;
            }
        }

        // Helper to read the MachineGuid from registry (used as HWID)
        private static string GetMachineGuid()
        {
            try
            {
                using var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");
                var guid = regKey?.GetValue("MachineGuid")?.ToString();
                return guid ?? "unknown-hwid";
            }
            catch { return "unknown-hwid"; }
        }

        // Public accessor for current HWID
        public static string GetCurrentHWID() => GetMachineGuid();

        // Verify license status (expiry and HWID)
        public static bool VerifyLicense(out string error)
        {
            error = string.Empty;
            if (CurrentLicense == null)
            {
                error = "License not loaded.";
                return false;
            }
            if (CurrentLicense.Expiry < DateTime.Now)
            {
                error = "License expired.";
                return false;
            }
            // HWID check (since server handles it, we can keep it as is or verify)
            if (CurrentLicense.HWID != GetCurrentHWID())
            {
                error = "HWID mismatch.";
                return false;
            }
            return true;
        }

        private static string GetLocalIP()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                return ip?.ToString() ?? "unknown";
            }
            catch { return "unknown"; }
        }
    }
}
