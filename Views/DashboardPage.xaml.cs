using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PCOptimizer.Models;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class DashboardPage : Page
    {
        private readonly List<TweakItem> _allTweaks = new();
        private readonly OptimizerEngine _optimizerEngine;
        private readonly HardwareDetector _hardwareDetector;
        private readonly DispatcherTimer _monitorTimer;
        private TweakItem? _selectedTweak;

        // Grid columns DP for responsive layout
        public static readonly DependencyProperty GridColumnsProperty =
            DependencyProperty.Register("GridColumns", typeof(int), typeof(DashboardPage), new PropertyMetadata(2));

        public int GridColumns
        {
            get => (int)GetValue(GridColumnsProperty);
            set => SetValue(GridColumnsProperty, value);
        }

        public DashboardPage()
        {
            InitializeComponent();
            _optimizerEngine = new OptimizerEngine();
            _hardwareDetector = new HardwareDetector();

            // Set up 13 tweaks data
            InitializeTweaksData();

            // Set up monitor timer for CPU / RAM usage
            _monitorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            _monitorTimer.Tick += MonitorTimer_Tick;

            Loaded += DashboardPage_Loaded;
            Unloaded += DashboardPage_Unloaded;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguageTranslations();
            FilterAndBindTweaks("Recommended", string.Empty);
            SelectTweak("ram_cleaner"); // Default selected
            
            // Start monitor
            UpdateSystemStats();
            _monitorTimer.Start();
        }

        private void ApplyLanguageTranslations()
        {
            TranslationService.LoadLanguage();

            // Core Page Titles
            PageTitleText.Text = TranslationService.Get("Dash_Title");
            PageSubtitleText.Text = TranslationService.Get("Dash_Subtitle");
            BulkBtn.Content = TranslationService.Get("Dash_QuickBoost");

            // Live Monitors Labels
            CpuMonitorLabel.Text = TranslationService.Get("Dash_CpuLoad");
            RamMonitorLabel.Text = TranslationService.Get("Dash_RamUsage");
            ReclaimedLabel.Text = TranslationService.Get("Dash_Reclaimed");

            // Details Panel Labels
            CleanupTitleLabel.Text = TranslationService.Get("Dash_DetailsTitle");
            CleanupTypeLabel.Text = TranslationService.Get("Dash_DetailsTitle"); // Fallback
            SimpleRadioLabel.Text = TranslationService.Get("Opt_MidTitle"); // Custom fallback
            AdvancedRadioLabel.Text = TranslationService.Get("Opt_HighTitle"); // Custom fallback
            
            DnsServerLabel.Text = TranslationService.Get("Mouse_Dpi"); // Custom fallback
            ScheduleBtn.Content = TranslationService.Get("Dash_ApplyOpt"); // Fallback scheduler action

            SeeAllBtn1.Text = TranslationService.Get("Dash_SeeAll");
            SeeAllBtn2.Text = TranslationService.Get("Dash_SeeAll");
            WhatWillBeProcessedTitle.Text = TranslationService.Get("Dash_WhatProcessed");

            // Translate tabs
            TabRecommended.Content = TranslationService.Get("Dash_TabRecommended");
            TabSystem.Content = TranslationService.Get("Dash_TabSystem");
            TabGaming.Content = TranslationService.Get("Dash_TabGaming");
            TabHardware.Content = TranslationService.Get("Dash_TabHardware");

            SearchBox.PlaceholderText = TranslationService.Get("Dash_SearchPlaceholder");

            // Translate all tweak items dynamically
            foreach (var tweak in _allTweaks)
            {
                tweak.Name = TranslationService.Get($"Tweak_{tweak.Id}_name");
                tweak.Description = TranslationService.Get($"Tweak_{tweak.Id}_desc");
                tweak.SubCategory = tweak.Subtitle switch
                {
                    "System" => TranslationService.Get("Sub_System"),
                    "Gaming" => TranslationService.Get("Sub_Gaming"),
                    "Hardware" => TranslationService.Get("Sub_Hardware"),
                    _ => tweak.Subtitle
                };
            }
        }

        private void DashboardPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _monitorTimer.Stop();
        }

        private void MonitorTimer_Tick(object? sender, EventArgs e)
        {
            UpdateSystemStats();
        }

        private void UpdateSystemStats()
        {
            try
            {
                // CPU load
                float cpu = _hardwareDetector.GetCpuUsage();
                CpuUsageTxt.Text = $"{Math.Round(cpu, 0)}%";
                CpuProgressBar.Value = cpu;

                // RAM utility
                var ram = _hardwareDetector.GetRamUsage();
                double usedGb = ram.totalGb - ram.freeGb;
                RamUsageTxt.Text = $"{Math.Round(usedGb, 1)} GB / {ram.totalGb} GB";
                RamProgressBar.Value = ram.usedPercent;
            }
            catch {}
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Responsive threshold: if left panel width > 720, show 3 columns; else 2.
            double leftWidth = e.NewSize.Width * 1.5 / 2.5;
            if (leftWidth > 720)
            {
                GridColumns = 3;
            }
            else
            {
                GridColumns = 2;
            }
        }

        private void InitializeTweaksData()
        {
            // 1. RAM Cleaner
            _allTweaks.Add(new TweakItem
            {
                Id = "ram_cleaner",
                Name = "RAM Cache Cleaner",
                Subtitle = "System",
                Description = "Mengosongkan RAM standby list dan working set memory untuk memberi ruang bagi game.",
                SizeText = "STANDBY",
                Category = "Recommended",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = true,
                Icon = "DataHistogram24",
                PerformanceEstimate = "+12% RAM"
            });

            // 2. Temp Cleaner
            _allTweaks.Add(new TweakItem
            {
                Id = "temp_cleaner",
                Name = "Temp Files Cleaner",
                Subtitle = "System",
                Description = "Membersihkan folder Temp pengguna dan folder Temp sistem Windows secara aman.",
                SizeText = "1.2 GB",
                Category = "System",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = true,
                Icon = "Delete24",
                PerformanceEstimate = "+4 GB Disk"
            });

            // 3. DNS Optimizer
            _allTweaks.Add(new TweakItem
            {
                Id = "dns_optimizer",
                Name = "DNS Latency Optimizer",
                Subtitle = "Gaming",
                Description = "Flushing DNS cache dan menyetel DNS tercepat untuk rute game online stabil.",
                SizeText = "NET PING",
                Category = "Recommended",
                SubCategory = "Gaming",
                IsRecommended = true,
                Icon = "Globe24",
                PerformanceEstimate = "-15ms Ping"
            });

            // 4. Junk Cleaner (Deep)
            _allTweaks.Add(new TweakItem
            {
                Id = "junk_cleaner",
                Name = "Deep Junk Cleaner",
                Subtitle = "System",
                Description = "Pembersih berkas sampah mendalam termasuk Prefetch, Recycle Bin, dan Logs.",
                SizeText = "850 MB",
                Category = "System",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = false,
                Icon = "Delete24",
                PerformanceEstimate = "+1 GB Disk"
            });

            // 5. Shader Cache Cleaner
            _allTweaks.Add(new TweakItem
            {
                Id = "shader_cleaner",
                Name = "Shader Cache Purge",
                Subtitle = "Gaming",
                Description = "Membersihkan shader DirectX dan GPU cache lama untuk mereduksi stuttering game.",
                SizeText = "SHADERS",
                Category = "Gaming",
                SubCategory = "Gaming",
                IsRecommended = false,
                Icon = "Image24",
                PerformanceEstimate = "Fps Stable"
            });

            // 6. Network Boost
            _allTweaks.Add(new TweakItem
            {
                Id = "network_boost",
                Name = "Network Packet Optimizer",
                Subtitle = "Gaming",
                Description = "Menonaktifkan Network Throttling Index dan mengaktifkan TcpAckFrequency.",
                SizeText = "TCP STABIL",
                Category = "Gaming",
                SubCategory = "Gaming",
                IsRecommended = false,
                Icon = "WifiWarning24",
                PerformanceEstimate = "-10ms Latency"
            });

            // 7. GPU Priority Boost
            _allTweaks.Add(new TweakItem
            {
                Id = "gpu_boost",
                Name = "GPU Priority Scheduler",
                Subtitle = "Gaming",
                Description = "Mengaktifkan mode daya HAGS dan mengutamakan tasks GPU untuk foreground gaming.",
                SizeText = "HAGS MODE",
                Category = "Gaming",
                SubCategory = "Gaming",
                IsRecommended = false,
                Icon = "DeveloperBoard24",
                PerformanceEstimate = "+8% FPS"
            });

            // 8. CPU Priority Boost
            _allTweaks.Add(new TweakItem
            {
                Id = "cpu_boost",
                Name = "CPU Priority Separator",
                Subtitle = "Gaming",
                Description = "Menyetel prioritas alokasi thread CPU game aktif di atas background processes.",
                SizeText = "CPU ALLOC",
                Category = "Gaming",
                SubCategory = "Gaming",
                IsRecommended = false,
                Icon = "Desktop24",
                PerformanceEstimate = "Low Lag"
            });

            // 9. Service Optimizer
            _allTweaks.Add(new TweakItem
            {
                Id = "service_optimizer",
                Name = "Telemetry Service Opt",
                Subtitle = "System",
                Description = "Menghentikan layanan diagnostic telemetry Windows non-esensial secara aman.",
                SizeText = "4 SERVICES",
                Category = "System",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = false,
                Icon = "Settings24",
                PerformanceEstimate = "-2% CPU"
            });

            // 10. Startup Optimizer
            _allTweaks.Add(new TweakItem
            {
                Id = "startup_optimizer",
                Name = "Startup Apps Manager",
                Subtitle = "Hardware",
                Description = "Mengurangi startup delay dengan menonaktifkan aplikasi otomatis yang tidak perlu.",
                SizeText = "STARTUP",
                Category = "Hardware",
                SubCategory = "Hardware & Boot",
                IsRecommended = false,
                Icon = "Navigation24",
                PerformanceEstimate = "+35% Boot"
            });

            // 11. Prefetch Cleaner
            _allTweaks.Add(new TweakItem
            {
                Id = "prefetch_cleaner",
                Name = "Prefetch Cache Cleaner",
                Subtitle = "System",
                Description = "Menghapus berkas prefetch lama untuk menyegarkan database cache direktori Windows.",
                SizeText = "240 MB",
                Category = "System",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = false,
                Icon = "Folder24",
                PerformanceEstimate = "Fresh System"
            });

            // 12. Registry Cleaner
            _allTweaks.Add(new TweakItem
            {
                Id = "registry_cleaner",
                Name = "Safe Registry Purge",
                Subtitle = "System",
                Description = "Membersihkan TypedPaths Explorer dan log cache navigasi registry secara aman.",
                SizeText = "REGISTRY",
                Category = "System",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = false,
                Icon = "Storage24",
                PerformanceEstimate = "Fast Shell"
            });

            // 13. Discord Cache Cleaner
            _allTweaks.Add(new TweakItem
            {
                Id = "discord_cache",
                Name = "Discord Cache Clean",
                Subtitle = "System",
                Description = "Membersihkan cache gambar dan berkas sisa media terfragmentasi dari Discord.",
                SizeText = "340 MB",
                Category = "System",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = false,
                Icon = "Chat24",
                PerformanceEstimate = "+340 MB Disk"
            });

            // 14. Browser Cache Cleaner
            _allTweaks.Add(new TweakItem
            {
                Id = "browser_cache",
                Name = "Browser Cache Clean",
                Subtitle = "System",
                Description = "Membersihkan cache Chrome, Edge, dan Firefox tanpa menghapus cookies/logins.",
                SizeText = "1.5 GB",
                Category = "System",
                SubCategory = "Sistem & Pembersihan",
                IsRecommended = false,
                Icon = "Browser24",
                PerformanceEstimate = "+1.5 GB Disk"
            });
        }

        private void FilterAndBindTweaks(string category, string searchQuery)
        {
            var filtered = _allTweaks.AsEnumerable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                filtered = filtered.Where(t => t.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || 
                                               t.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }
            else if (category != "Recommended")
            {
                filtered = filtered.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase) ||
                                               t.Subtitle.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (category == "Recommended" && string.IsNullOrEmpty(searchQuery))
            {
                var recommendedList = _allTweaks.Where(t => t.IsRecommended).ToList();
                var otherList = _allTweaks.Where(t => !t.IsRecommended).ToList();

                RecommendedItemsCtrl.ItemsSource = recommendedList;
                SystemItemsCtrl.ItemsSource = otherList;

                RecSection.Visibility = Visibility.Visible;
                SysSection.Visibility = Visibility.Visible;

                RecHeaderTitle.Text = TranslationService.Get("Dash_RecTitle");
                SysHeaderTitle.Text = TranslationService.Get("Dash_SysTitle");
            }
            else
            {
                RecommendedItemsCtrl.ItemsSource = filtered.ToList();
                SystemItemsCtrl.ItemsSource = null;

                RecSection.Visibility = Visibility.Visible;
                SysSection.Visibility = Visibility.Collapsed;

                string localizedCat = TranslationService.Get("Dash_CatTitle");
                RecHeaderTitle.Text = localizedCat.Contains("{0}") ? string.Format(localizedCat, category) : $"{localizedCat} {category}";
            }
        }

        private void SelectTweak(string id)
        {
            _selectedTweak = _allTweaks.FirstOrDefault(t => t.Id == id);
            if (_selectedTweak == null) return;

            DetailTitle.Text = _selectedTweak.Name;
            DetailDesc.Text = _selectedTweak.Description;
            
            // Set details icon
            DetailIcon.Symbol = (Wpf.Ui.Controls.SymbolRegular)Enum.Parse(typeof(Wpf.Ui.Controls.SymbolRegular), _selectedTweak.Icon);
            
            // Toggle options panels dynamically
            if (id == "junk_cleaner")
            {
                CleanupOptionsPanel.Visibility = Visibility.Visible;
                DnsOptionsPanel.Visibility = Visibility.Collapsed;
                GeneralChangesPanel.Visibility = Visibility.Collapsed;
                RunTweakBtn.Content = TranslationService.Get("Dash_RunClean");
            }
            else if (id == "dns_optimizer")
            {
                CleanupOptionsPanel.Visibility = Visibility.Collapsed;
                DnsOptionsPanel.Visibility = Visibility.Visible;
                GeneralChangesPanel.Visibility = Visibility.Collapsed;
                RunTweakBtn.Content = TranslationService.Get("Dash_RunDns");
            }
            else
            {
                CleanupOptionsPanel.Visibility = Visibility.Collapsed;
                DnsOptionsPanel.Visibility = Visibility.Collapsed;
                GeneralChangesPanel.Visibility = Visibility.Visible;
                
                bool isClean = _selectedTweak.Id.Contains("clean") || _selectedTweak.Id.Contains("purge") || _selectedTweak.Id.Contains("cache");
                RunTweakBtn.Content = isClean ? TranslationService.Get("Dash_RunClean") : TranslationService.Get("Dash_ApplyOpt");

                // Setup specific description text
                DetailChangesTxt.Text = TranslationService.Get($"Tweak_{id}_detail");
            }
        }

        private void TweakCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string id)
            {
                SelectTweak(id);
            }
        }

        private void RunCard_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent card click trigger
            if (sender is Border border && border.Tag is string id)
            {
                SelectTweak(id);
                ExecuteSelectedTweak();
            }
        }

        private void RunTweakBtn_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSelectedTweak();
        }

        private async void ExecuteSelectedTweak()
        {
            if (_selectedTweak == null) return;

            string id = _selectedTweak.Id;
            
            // Special routing for startup manager redirect
            if (id == "startup_optimizer")
            {
                // Find parent Navigation and go to startup page
                var parentWindow = Application.Current.MainWindow as MainWindow;
                if (parentWindow != null)
                {
                    parentWindow.RootNavigation.Navigate(typeof(StartupPage));
                }
                return;
            }

            // Lock UI and show animated Progress
            RunTweakBtn.Visibility = Visibility.Collapsed;
            ProgressGrid.Visibility = Visibility.Visible;

            try
            {
                // Run progress ring animation from 0% to 100%
                for (int i = 0; i <= 100; i += 10)
                {
                    TweakProgressRing.Progress = i;
                    ProgressPercentTxt.Text = $"{i}%";
                    if (i < 30) ProgressStatusTxt.Text = TranslationService.Get("Status_Analyzing");
                    else if (i < 70) ProgressStatusTxt.Text = TranslationService.Get("Status_Applying");
                    else ProgressStatusTxt.Text = TranslationService.Get("Status_Finalizing");
                    await Task.Delay(120);
                }

                // Execute real program operations
                string successMsg = "Optimasi berhasil diselesaikan!";
                switch (id)
                {
                    case "ram_cleaner":
                        int ramProcCount = await _optimizerEngine.BoostRamAsync();
                        successMsg = string.Format(TranslationService.Get("Msg_ram_cleaner_success"), ramProcCount);
                        break;
                        
                    case "temp_cleaner":
                        long bytesTemp = await _optimizerEngine.CleanJunkFilesAsync(true, false, false, false);
                        double mbTemp = (double)bytesTemp / (1024 * 1024);
                        successMsg = string.Format(TranslationService.Get("Msg_temp_cleaner_success"), Math.Round(mbTemp, 1));
                        break;
                        
                    case "junk_cleaner":
                        bool deep = AdvancedCleanupRadio.IsChecked == true;
                        long bytesJ = await _optimizerEngine.CleanJunkFilesAsync(true, deep, true, deep);
                        double mbJ = (double)bytesJ / (1024 * 1024);
                        successMsg = string.Format(TranslationService.Get("Msg_junk_cleaner_success"), Math.Round(mbJ, 1));
                        break;

                    case "dns_optimizer":
                        string dnsProvider = "Cloudflare";
                        if (DnsComboBox.SelectedIndex == 1) dnsProvider = "Google";
                        else if (DnsComboBox.SelectedIndex == 2) dnsProvider = "Quad9";
                        else if (DnsComboBox.SelectedIndex == 3) dnsProvider = "Default";

                        await _optimizerEngine.OptimizeDnsAsync(dnsProvider);
                        successMsg = dnsProvider == "Default" ? TranslationService.Get("Msg_dns_default_success") : string.Format(TranslationService.Get("Msg_dns_optimizer_success"), dnsProvider);
                        break;

                    case "shader_cleaner":
                        await _optimizerEngine.CleanShaderCacheAsync();
                        successMsg = TranslationService.Get("Msg_shader_cleaner_success");
                        break;

                    case "network_boost":
                        await _optimizerEngine.OptimizeNetworkPingAsync();
                        successMsg = TranslationService.Get("Msg_network_boost_success");
                        break;

                    case "gpu_boost":
                        await _optimizerEngine.ApplyGpuPriorityBoostAsync();
                        successMsg = TranslationService.Get("Msg_gpu_boost_success");
                        break;

                    case "cpu_boost":
                        await _optimizerEngine.ApplyCpuPriorityBoostAsync();
                        successMsg = TranslationService.Get("Msg_cpu_boost_success");
                        break;

                    case "service_optimizer":
                        await _optimizerEngine.ApplyServiceOptimizerAsync();
                        successMsg = TranslationService.Get("Msg_service_optimizer_success");
                        break;

                    case "prefetch_cleaner":
                        long bytesP = await _optimizerEngine.CleanJunkFilesAsync(false, true, false, false);
                        double mbP = (double)bytesP / (1024 * 1024);
                        successMsg = string.Format(TranslationService.Get("Msg_prefetch_cleaner_success"), Math.Round(mbP, 1));
                        break;

                    case "registry_cleaner":
                        await _optimizerEngine.ApplyRegistryCleanerAsync();
                        successMsg = TranslationService.Get("Msg_registry_cleaner_success");
                        break;

                    case "discord_cache":
                        await CleanDiscordFolderAsync("Cache");
                        await CleanDiscordFolderAsync("Code Cache");
                        successMsg = TranslationService.Get("Msg_discord_cache_success");
                        break;

                    case "browser_cache":
                        await _optimizerEngine.CleanBrowserCacheAsync();
                        successMsg = TranslationService.Get("Msg_browser_cache_success");
                        break;
                }

                // Show success toast notification
                var mainWin = Application.Current.MainWindow as MainWindow;
                if (mainWin != null)
                {
                    mainWin.ShowToast(_selectedTweak.Name, successMsg, true);
                }
            }
            catch (Exception ex)
            {
                var mainWin = Application.Current.MainWindow as MainWindow;
                if (mainWin != null)
                {
                    mainWin.ShowToast("Optimasi Gagal", $"Gagal memproses: {ex.Message}", false);
                }
            }
            finally
            {
                ProgressGrid.Visibility = Visibility.Collapsed;
                RunTweakBtn.Visibility = Visibility.Visible;
            }
        }

        private async Task CleanDiscordFolderAsync(string subFolder)
        {
            await Task.Run(() =>
            {
                try
                {
                    var processes = System.Diagnostics.Process.GetProcessesByName("Discord");
                    foreach (var p in processes)
                    {
                        try { p.Kill(); p.WaitForExit(3000); } catch {}
                    }

                    string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string discordCachePath = Path.Combine(appData, "discord", subFolder);
                    if (Directory.Exists(discordCachePath))
                    {
                        var dir = new DirectoryInfo(discordCachePath);
                        foreach (var file in dir.GetFiles())
                        {
                            try { file.Delete(); } catch {}
                        }
                        foreach (var sub in dir.GetDirectories())
                        {
                            try { sub.Delete(true); } catch {}
                        }
                    }
                }
                catch {}
            });
        }

        private async void BulkBtn_Click(object sender, RoutedEventArgs e)
        {
            BulkBtn.IsEnabled = false;
            BulkBtn.Content = TranslationService.Get("Btn_Boosting");

            try
            {
                // Run ⚡ QUICK BOOST bulk actions
                await _optimizerEngine.BoostRamAsync();
                await _optimizerEngine.CleanJunkFilesAsync(true, false, true, false);
                await _optimizerEngine.OptimizeNetworkPingAsync();
                await _optimizerEngine.ApplyCpuPriorityBoostAsync();

                var mainWin = Application.Current.MainWindow as MainWindow;
                if (mainWin != null)
                {
                    mainWin.ShowToast(TranslationService.Get("Msg_QuickBoostTitle"), TranslationService.Get("Msg_QuickBoostSuccess"), true);
                }
            }
            catch (Exception ex)
            {
                var mainWin = Application.Current.MainWindow as MainWindow;
                if (mainWin != null)
                {
                    mainWin.ShowToast(TranslationService.Get("Msg_QuickBoostFail"), ex.Message, false);
                }
            }
            finally
            {
                BulkBtn.IsEnabled = true;
                BulkBtn.Content = TranslationService.Get("Dash_QuickBoost");
            }
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio)
            {
                string tag = radio.Tag?.ToString() ?? "Recommended";
                
                // Clear active color in other buttons
                foreach (var child in (radio.Parent as Panel)?.Children ?? new UIElementCollection(null, null))
                {
                    if (child is RadioButton otherRadio)
                    {
                        otherRadio.Foreground = otherRadio == radio ? Brushes.White : (Brush)new BrushConverter().ConvertFromString("#8F9CAE")!;
                    }
                }

                FilterAndBindTweaks(tag, SearchBox.Text);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAndBindTweaks("Recommended", SearchBox.Text);
        }

        private void ScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(TranslationService.Get("Msg_ScheduleBody"), TranslationService.Get("Msg_ScheduleTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
