using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using PCOptimizer.Models;

namespace PCOptimizer.Services
{
    public class GameDetector
    {
        private List<GameItem> _detectedGames = new();
        private readonly HashSet<string> _knownGameExecutables = new(StringComparer.OrdinalIgnoreCase)
        {
            "cs2.exe", "gta5.exe", "VALORANT-Win64-Shipping.exe", "FortniteClient-Win64-Shipping.exe",
            "Overwatch.exe", "League of Legends.exe", "dota2.exe", "cyberpunk2077.exe",
            "Minecraft.exe", "r5apex.exe", "fifa.exe", "VALORANT.exe", "GenshinImpact.exe",
            "cod.exe", "pubg.exe", "TslGame.exe", "Rust.exe", "RobloxPlayerBeta.exe"
        };

        public GameDetector()
        {
            ScanForInstalledGames();
        }

        public List<GameItem> GetDetectedGames()
        {
            // Sync active process state
            var activeProcesses = Process.GetProcesses();
            var activeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in activeProcesses)
            {
                try
                {
                    activeNames.Add(p.ProcessName + ".exe");
                }
                catch {}
            }

            foreach (var game in _detectedGames)
            {
                bool wasRunning = game.IsRunning;
                game.IsRunning = activeNames.Contains(game.ExecutableName);
                if (game.IsRunning && !wasRunning && game.AutoBoostEnabled)
                {
                    game.LastBoosted = DateTime.Now;
                    ApplyProcessBoost(game.ExecutableName);
                }
            }

            return _detectedGames;
        }

        public void AddCustomGame(string name, string executablePath)
        {
            if (string.IsNullOrEmpty(executablePath)) return;

            string fileName = Path.GetFileName(executablePath);
            if (string.IsNullOrEmpty(fileName)) return;

            // Check if already exists
            if (_detectedGames.Exists(g => g.ExecutableName.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
                return;

            _detectedGames.Add(new GameItem
            {
                Name = name,
                ExecutableName = fileName,
                FilePath = executablePath,
                Launcher = "Kustom",
                AutoBoostEnabled = true,
                CoverImagePath = "https://images.unsplash.com/photo-1538481199705-c710c4e965fc?q=80&w=600&auto=format&fit=crop" // Default custom game cover (Unsplash gaming)
            });
        }

        private void ScanForInstalledGames()
        {
            // Add popular PC/Steam games with high‑quality cover URLs
            _detectedGames.Add(new GameItem
            {
                Name = "VALORANT",
                ExecutableName = "VALORANT.exe",
                FilePath = @"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe",
                Launcher = "Riot Games",
                AutoBoostEnabled = true,
                CoverImagePath = "https://upload.wikimedia.org/wikipedia/en/f/fc/Valorant_cover_art.jpg",
                PerformanceIndicator = "Ultra"
            });

            _detectedGames.Add(new GameItem
            {
                Name = "Apex Legends",
                ExecutableName = "r5apex.exe",
                FilePath = @"C:\Games\ApexLegends\r5apex.exe",
                Launcher = "Steam",
                AutoBoostEnabled = true,
                CoverImagePath = "https://cdn.cloudflare.steamstatic.com/steam/apps/1172470/library_600x900.jpg",
                PerformanceIndicator = "High"
            });

            _detectedGames.Add(new GameItem
            {
                Name = "Elden Ring",
                ExecutableName = "eldenring.exe",
                FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Elden Ring\eldenring.exe",
                Launcher = "Steam",
                AutoBoostEnabled = true,
                CoverImagePath = "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/library_600x900.jpg",
                PerformanceIndicator = "Ultra"
            });

            _detectedGames.Add(new GameItem
            {
                Name = "Grand Theft Auto V",
                ExecutableName = "GTA5.exe",
                FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V\GTA5.exe",
                Launcher = "Steam",
                AutoBoostEnabled = true,
                CoverImagePath = "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/library_600x900.jpg",
                PerformanceIndicator = "High"
            });

            _detectedGames.Add(new GameItem
            {
                Name = "Cyberpunk 2077",
                ExecutableName = "cyberpunk2077.exe",
                FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Cyberpunk 2077\cyberpunk2077.exe",
                Launcher = "Steam",
                AutoBoostEnabled = true,
                CoverImagePath = "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/library_600x900.jpg",
                PerformanceIndicator = "Ultra"
            });

            _detectedGames.Add(new GameItem
            {
                Name = "Minecraft",
                ExecutableName = "MinecraftLauncher.exe",
                FilePath = @"C:\Program Files (x86)\Minecraft\MinecraftLauncher.exe",
                Launcher = "Mojang",
                AutoBoostEnabled = true,
                CoverImagePath = "https://upload.wikimedia.org/wikipedia/en/5/51/Minecraft_cover_art.jpg",
                PerformanceIndicator = "High"
            });

            _detectedGames.Add(new GameItem
            {
                Name = "Counter-Strike 2",
                ExecutableName = "cs2.exe",
                FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\game\bin\win64\cs2.exe",
                Launcher = "Steam",
                AutoBoostEnabled = true,
                CoverImagePath = "https://cdn.cloudflare.steamstatic.com/steam/apps/730/library_600x900.jpg",
                PerformanceIndicator = "Ultra"
            });

            _detectedGames.Add(new GameItem
            {
                Name = "Dota 2",
                ExecutableName = "dota2.exe",
                FilePath = @"C:\Games\Dota 2\dota2.exe",
                Launcher = "Steam",
                AutoBoostEnabled = true,
                CoverImagePath = "https://cdn.cloudflare.steamstatic.com/steam/apps/570/library_600x900.jpg",
                PerformanceIndicator = "Ultra"
            });
        }

        private void ApplyProcessBoost(string exeName)
        {
            try
            {
                string procName = Path.GetFileNameWithoutExtension(exeName);
                var processes = Process.GetProcessesByName(procName);
                foreach (var p in processes)
                {
                    p.PriorityClass = ProcessPriorityClass.High;
                    Debug.WriteLine($"Successfully boosted process {p.ProcessName} to High Priority.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error boosting process priority: " + ex.Message);
            }
        }
    }
}
