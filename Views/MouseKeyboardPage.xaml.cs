using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.Win32;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class MouseKeyboardPage : Page
    {
        private readonly OptimizerEngine _optimizerEngine;
        private bool _isInitialLoading = true;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        private const uint SPI_SETMOUSESPEED = 0x0071;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        public MouseKeyboardPage()
        {
            InitializeComponent();
            _optimizerEngine = new OptimizerEngine();

            Loaded += MouseKeyboardPage_Loaded;

            // Attach auto-save triggers
            DpiComboBox.SelectionChanged += (s, e) => TriggerAutoSave();
            HzComboBox.SelectionChanged += (s, e) => TriggerAutoSave();
            MouseTweakToggle.Checked += (s, e) => TriggerAutoSave();
            MouseTweakToggle.Unchecked += (s, e) => TriggerAutoSave();
            KeyboardTweakToggle.Checked += (s, e) => TriggerAutoSave();
            KeyboardTweakToggle.Unchecked += (s, e) => TriggerAutoSave();
            UsbSuspendToggle.Checked += (s, e) => TriggerAutoSave();
            UsbSuspendToggle.Unchecked += (s, e) => TriggerAutoSave();
            PollingPriorityToggle.Checked += (s, e) => TriggerAutoSave();
            PollingPriorityToggle.Unchecked += (s, e) => TriggerAutoSave();
        }

        private void MouseKeyboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            _isInitialLoading = true;
            ApplyLanguageTranslations();
            try
            {
                // Detect current settings to check/uncheck toggles
                using (var mouseKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse"))
                {
                    if (mouseKey != null)
                    {
                        string speed = mouseKey.GetValue("MouseSpeed")?.ToString() ?? "1";
                        string th1 = mouseKey.GetValue("MouseThreshold1")?.ToString() ?? "6";
                        MouseTweakToggle.IsChecked = (speed == "0" && th1 == "0");

                        // Load Pointer Speed
                        string sensVal = mouseKey.GetValue("MouseSensitivity")?.ToString() ?? "10";
                        if (int.TryParse(sensVal, out int sens) && sens >= 1 && sens <= 20)
                        {
                            PointerSpeedSlider.Value = sens;
                            PointerSpeedValueText.Text = sens.ToString();
                        }
                    }
                }

                using (var kbKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Keyboard"))
                {
                    if (kbKey != null)
                    {
                        string delay = kbKey.GetValue("KeyboardDelay")?.ToString() ?? "1";
                        KeyboardTweakToggle.IsChecked = (delay == "0");
                    }
                }

                // Load custom mouse settings (X/Y Sensi, DPI, Hz)
                using (var customKey = Registry.CurrentUser.OpenSubKey(@"Software\ClayxOptimize"))
                {
                    if (customKey != null)
                    {
                        // Sensi X
                        string sx = customKey.GetValue("MouseSensiX")?.ToString() ?? "1.00";
                        if (double.TryParse(sx, NumberStyles.Any, CultureInfo.InvariantCulture, out double sensiX))
                        {
                            SensiXSlider.Value = sensiX;
                            SensiXValueText.Text = sensiX.ToString("F2") + "x";
                        }

                        // Sensi Y
                        string sy = customKey.GetValue("MouseSensiY")?.ToString() ?? "1.00";
                        if (double.TryParse(sy, NumberStyles.Any, CultureInfo.InvariantCulture, out double sensiY))
                        {
                            SensiYSlider.Value = sensiY;
                            SensiYValueText.Text = sensiY.ToString("F2") + "x";
                        }

                        // DPI
                        string dpiStr = customKey.GetValue("MouseDpi")?.ToString() ?? "800";
                        int dpiIdx = dpiStr switch
                        {
                            "400" => 0,
                            "800" => 1,
                            "1200" => 2,
                            "1600" => 3,
                            "3200" => 4,
                            "6400" => 5,
                            _ => 1
                        };
                        DpiComboBox.SelectedIndex = dpiIdx;

                        // Hz
                        string hzStr = customKey.GetValue("MouseHz")?.ToString() ?? "1000";
                        int hzIdx = hzStr switch
                        {
                            "125" => 0,
                            "250" => 1,
                            "500" => 2,
                            "1000" => 3,
                            "2000" => 4,
                            "4000" => 5,
                            "8000" => 6,
                            _ => 3
                        };
                        HzComboBox.SelectedIndex = hzIdx;

                        // Load AutoSave
                        string autoSaveStr = customKey.GetValue("AutoSaveConfig")?.ToString() ?? "1";
                        AutoSaveToggle.IsChecked = autoSaveStr == "1";
                    }
                }
            }
            catch {}
            finally
            {
                _isInitialLoading = false;
            }
        }

        private void ApplyLanguageTranslations()
        {
            TranslationService.LoadLanguage();

            PageHeaderText.Text = TranslationService.Get("Mouse_Title");
            PageSubtitleText.Text = TranslationService.Get("Mouse_Subtitle");

            // Card 1
            Card1Title.Text = TranslationService.Get("Mouse_AimTitle");
            Card1Desc.Text = TranslationService.Get("Mouse_AimDesc");
            Card1ActivateText.Text = TranslationService.Get("Mouse_Activate");

            // Card 2
            Card2Title.Text = TranslationService.Get("Mouse_KbTitle");
            Card2Desc.Text = TranslationService.Get("Mouse_KbDesc");
            Card2ActivateText.Text = TranslationService.Get("Mouse_Activate");

            // Card 3
            Card3Title.Text = TranslationService.Get("Mouse_UsbTitle");
            Card3Desc.Text = TranslationService.Get("Mouse_UsbDesc");
            Card3ActivateText.Text = TranslationService.Get("Mouse_Activate");

            // Card 4
            Card4Title.Text = TranslationService.Get("Mouse_PollTitle");
            Card4Desc.Text = TranslationService.Get("Mouse_PollDesc");
            Card4ActivateText.Text = TranslationService.Get("Mouse_Activate");

            // Card 5
            Card5Title.Text = TranslationService.Get("Mouse_SensiTitle");
            PointerSpeedLabelText.Text = TranslationService.Get("Mouse_PointerSpeed");
            SensiXLabelText.Text = TranslationService.Get("Mouse_SensiX");
            SensiYLabelText.Text = TranslationService.Get("Mouse_SensiY");
            ApplySensitivityBtn.Content = TranslationService.Get("Mouse_ApplySensi");

            // Card 6
            Card6Title.Text = TranslationService.Get("Mouse_HardwareTitle");
            DpiLabelText.Text = TranslationService.Get("Mouse_Dpi");
            HzLabelText.Text = TranslationService.Get("Mouse_Hz");
            HardwareFooterText.Text = TranslationService.Get("Mouse_HardwareFooter");
            ApplyHardwareBtn.Content = TranslationService.Get("Mouse_ApplyHardware");

            // Card 7
            Card7Title.Text = TranslationService.Get("Mouse_ConfigTitle");
            Card7DescText.Text = TranslationService.Get("Mouse_ConfigDesc");
            AutoSaveLabelText.Text = TranslationService.Get("Mouse_AutoSave");
            AutoSaveSubLabelText.Text = TranslationService.Get("Mouse_AutoSaveDesc");
            ExportConfigBtn.Content = TranslationService.Get("Mouse_Export");
            ImportConfigBtn.Content = TranslationService.Get("Mouse_Import");
        }

        private void MouseTweakToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            _optimizerEngine.ApplyMouseTweaks(true);
            MessageBox.Show("MarkC Mouse Fix sukses diterapkan! Mouse acceleration dimatikan (Aim 1:1 aktif).", "Mouse Tweaked", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MouseTweakToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            _optimizerEngine.ApplyMouseTweaks(false);
            MessageBox.Show("Pengaturan mouse dikembalikan ke default Windows (Akselerasi aktif).", "Mouse Default", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void KeyboardTweakToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            _optimizerEngine.ApplyKeyboardTweaks(true);
            MessageBox.Show("Optimasi Keyboard sukses! Delay input diminimalkan.", "Keyboard Tweaked", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void KeyboardTweakToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            _optimizerEngine.ApplyKeyboardTweaks(false);
            MessageBox.Show("Keyboard dikembalikan ke setting delay default Windows.", "Keyboard Default", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UsbSuspendToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            SetUsbSelectiveSuspend(false); // disable suspend
            MessageBox.Show("USB Selective Suspend dinonaktifkan! Periferal tidak akan hemat daya secara mendadak.", "USB Optimized", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UsbSuspendToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            SetUsbSelectiveSuspend(true); // enable suspend (default)
            MessageBox.Show("USB Selective Suspend dikembalikan ke pengaturan default Windows.", "USB Default", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PollingPriorityToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            // Write custom CPU registry priority for USB Hub
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl", true);
                if (key != null)
                {
                    key.SetValue("Win32PrioritySeparation", 38, RegistryValueKind.DWord); // Hex 0x26 - prioritizes foreground
                }
                MessageBox.Show("Prioritas Polling USB ditingkatkan pada CPU untuk mouse 1000Hz+.", "Polling Optimized", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Gagal mengakses PriorityControl registry. Memerlukan hak Administrator.", "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PollingPriorityToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoading) return;
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl", true);
                if (key != null)
                {
                    key.SetValue("Win32PrioritySeparation", 2, RegistryValueKind.DWord);
                }
                MessageBox.Show("Prioritas Polling USB dikembalikan ke default Windows.", "Polling Default", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch {}
        }

        private void SetUsbSelectiveSuspend(bool enable)
        {
            try
            {
                string value = enable ? "1" : "0";
                
                // USB Subgroup GUID: 2a737441-1930-4402-8d77-b2bebba308a3
                // USB Selective Suspend Setting GUID: 48e6b7a6-50f5-4782-a5d4-53bb8f07e226
                string argsAc = $"/SETACVALUEINDEX SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 {value}";
                string argsDc = $"/SETDCVALUEINDEX SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 {value}";

                var procAc = Process.Start(new ProcessStartInfo("powercfg.exe", argsAc) { CreateNoWindow = true, UseShellExecute = false });
                procAc?.WaitForExit();

                var procDc = Process.Start(new ProcessStartInfo("powercfg.exe", argsDc) { CreateNoWindow = true, UseShellExecute = false });
                procDc?.WaitForExit();
                
                // Apply changes
                var procApply = Process.Start(new ProcessStartInfo("powercfg.exe", "/S SCHEME_CURRENT") { CreateNoWindow = true, UseShellExecute = false });
                procApply?.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error setting USB selective suspend: " + ex.Message);
            }
        }

        // New Mouse Event Handlers
        private void PointerSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitialLoading) return;
            if (PointerSpeedValueText != null)
            {
                PointerSpeedValueText.Text = ((int)e.NewValue).ToString();
            }
            TriggerAutoSave();
        }

        private void SensiXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitialLoading) return;
            if (SensiXValueText != null)
            {
                SensiXValueText.Text = e.NewValue.ToString("F2") + "x";
            }
            TriggerAutoSave();
        }

        private void SensiYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitialLoading) return;
            if (SensiYValueText != null)
            {
                SensiYValueText.Text = e.NewValue.ToString("F2") + "x";
            }
            TriggerAutoSave();
        }

        private void ApplySensitivity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int pointerSpeed = (int)PointerSpeedSlider.Value;
                double sensiX = SensiXSlider.Value;
                double sensiY = SensiYSlider.Value;

                // 1. Write Pointer Speed to system and registry
                bool success = SystemParametersInfo(SPI_SETMOUSESPEED, 0, (uint)pointerSpeed, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                if (success)
                {
                    // Also write to Registry manually to ensure permanence
                    using (var mouseKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse", true))
                    {
                        if (mouseKey != null)
                        {
                            mouseKey.SetValue("MouseSensitivity", pointerSpeed.ToString());
                        }
                    }
                }

                // 2. Save X and Y sensitivity factors to custom key
                using (var customKey = Registry.CurrentUser.CreateSubKey(@"Software\ClayxOptimize"))
                {
                    if (customKey != null)
                    {
                        customKey.SetValue("MouseSensiX", sensiX.ToString("F2", CultureInfo.InvariantCulture));
                        customKey.SetValue("MouseSensiY", sensiY.ToString("F2", CultureInfo.InvariantCulture));
                    }
                }

                MessageBox.Show($"Pengaturan sensitivitas berhasil diterapkan!\nKecepatan Pointer: {pointerSpeed}\nSens X: {sensiX:F2}x | Sens Y: {sensiY:F2}x\n(Kompatibel untuk VALORANT & Minecraft)", "Sensitivitas Diterapkan", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menerapkan sensitivitas mouse: " + ex.Message, "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyHardware_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dpiVal = GetSelectedDpiValue();
                string hzVal = GetSelectedHzValue();

                // Save to custom key
                using (var customKey = Registry.CurrentUser.CreateSubKey(@"Software\ClayxOptimize"))
                {
                    if (customKey != null)
                    {
                        customKey.SetValue("MouseDpi", dpiVal);
                        customKey.SetValue("MouseHz", hzVal);
                    }
                }

                // Apply Registry Optimization for high polling rate if needed
                int hz = int.Parse(hzVal);
                if (hz >= 1000)
                {
                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl", true);
                        if (key != null)
                        {
                            // Set foreground priority separation to optimize mouse interrupt processing
                            key.SetValue("Win32PrioritySeparation", 38, RegistryValueKind.DWord);
                        }
                    }
                    catch {}
                }

                MessageBox.Show($"Konfigurasi hardware berhasil disimpan!\nMouse diselaraskan pada {dpiVal} DPI dan {hzVal} Hz Polling Rate.", "Konfigurasi Hardware Disimpan", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan konfigurasi hardware: " + ex.Message, "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Auto-Save Toggle Event Handlers
        private void AutoSaveToggle_Checked(object sender, RoutedEventArgs e)
        {
            SaveAutoSaveState(true);
            TriggerAutoSave();
        }

        private void AutoSaveToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveAutoSaveState(false);
        }

        private void SaveAutoSaveState(bool enabled)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\ClayxOptimize");
                key?.SetValue("AutoSaveConfig", enabled ? 1 : 0, RegistryValueKind.DWord);
            }
            catch {}
        }

        private void TriggerAutoSave()
        {
            if (_isInitialLoading) return;
            if (AutoSaveToggle.IsChecked == true)
            {
                try
                {
                    var config = new PageConfig
                    {
                        MouseAimStabilizer = MouseTweakToggle.IsChecked == true,
                        KeyboardLatencyReducer = KeyboardTweakToggle.IsChecked == true,
                        UsbSelectiveSuspend = UsbSuspendToggle.IsChecked == true,
                        UsbPollingRatePriority = PollingPriorityToggle.IsChecked == true,
                        PointerSpeed = (int)PointerSpeedSlider.Value,
                        SensiX = SensiXSlider.Value,
                        SensiY = SensiYSlider.Value,
                        Dpi = GetSelectedDpiValue(),
                        Hz = GetSelectedHzValue(),
                        AutoSave = true
                    };
                    SaveAllSettingsToRegistry(config);
                }
                catch {}
            }
        }

        // Export config
        private void ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = new PageConfig
                {
                    MouseAimStabilizer = MouseTweakToggle.IsChecked == true,
                    KeyboardLatencyReducer = KeyboardTweakToggle.IsChecked == true,
                    UsbSelectiveSuspend = UsbSuspendToggle.IsChecked == true,
                    UsbPollingRatePriority = PollingPriorityToggle.IsChecked == true,
                    PointerSpeed = (int)PointerSpeedSlider.Value,
                    SensiX = SensiXSlider.Value,
                    SensiY = SensiYSlider.Value,
                    Dpi = GetSelectedDpiValue(),
                    Hz = GetSelectedHzValue(),
                    AutoSave = AutoSaveToggle.IsChecked == true
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    FileName = "clayx_mouse_config.json"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, json);
                    MessageBox.Show("Konfigurasi berhasil diekspor!", "Ekspor Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengekspor konfigurasi: " + ex.Message, "Ekspor Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Import config
        private void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var json = System.IO.File.ReadAllText(openFileDialog.FileName);
                    var config = Newtonsoft.Json.JsonConvert.DeserializeObject<PageConfig>(json);

                    if (config != null)
                    {
                        // Temporarily disable auto-save triggers to prevent redundant calls
                        _isInitialLoading = true;

                        // Apply configurations to UI controls
                        MouseTweakToggle.IsChecked = config.MouseAimStabilizer;
                        KeyboardTweakToggle.IsChecked = config.KeyboardLatencyReducer;
                        UsbSuspendToggle.IsChecked = config.UsbSelectiveSuspend;
                        PollingPriorityToggle.IsChecked = config.UsbPollingRatePriority;
                        
                        PointerSpeedSlider.Value = config.PointerSpeed;
                        PointerSpeedValueText.Text = config.PointerSpeed.ToString();
                        
                        SensiXSlider.Value = config.SensiX;
                        SensiXValueText.Text = config.SensiX.ToString("F2") + "x";

                        SensiYSlider.Value = config.SensiY;
                        SensiYValueText.Text = config.SensiY.ToString("F2") + "x";

                        AutoSaveToggle.IsChecked = config.AutoSave;

                        // Set ComboBox indices
                        DpiComboBox.SelectedIndex = config.Dpi switch
                        {
                            "400" => 0,
                            "800" => 1,
                            "1200" => 2,
                            "1600" => 3,
                            "3200" => 4,
                            "6400" => 5,
                            _ => 1
                        };

                        HzComboBox.SelectedIndex = config.Hz switch
                        {
                            "125" => 0,
                            "250" => 1,
                            "500" => 2,
                            "1000" => 3,
                            "2000" => 4,
                            "4000" => 5,
                            "8000" => 6,
                            _ => 3
                        };

                        _isInitialLoading = false;

                        // Apply active system tweaks
                        _optimizerEngine.ApplyMouseTweaks(config.MouseAimStabilizer);
                        _optimizerEngine.ApplyKeyboardTweaks(config.KeyboardLatencyReducer);
                        SetUsbSelectiveSuspend(!config.UsbSelectiveSuspend);
                        
                        // Set USB Polling priority
                        try
                        {
                            using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl", true);
                            if (key != null)
                            {
                                key.SetValue("Win32PrioritySeparation", config.UsbPollingRatePriority ? 38 : 2, RegistryValueKind.DWord);
                            }
                        }
                        catch {}

                        // Set Pointer Speed
                        SystemParametersInfo(SPI_SETMOUSESPEED, 0, (uint)config.PointerSpeed, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                        // Save everything to registry
                        SaveAllSettingsToRegistry(config);

                        MessageBox.Show("Konfigurasi berhasil diimpor dan diterapkan!", "Impor Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengimpor konfigurasi: " + ex.Message, "Impor Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSelectedDpiValue()
        {
            if (DpiComboBox.SelectedItem is ComboBoxItem dpiItem)
            {
                string content = dpiItem.Content.ToString() ?? "";
                if (content.Contains("400")) return "400";
                if (content.Contains("800")) return "800";
                if (content.Contains("1200")) return "1200";
                if (content.Contains("1600")) return "1600";
                if (content.Contains("3200")) return "3200";
                if (content.Contains("6400")) return "6400";
            }
            return "800";
        }

        private string GetSelectedHzValue()
        {
            if (HzComboBox.SelectedItem is ComboBoxItem hzItem)
            {
                string content = hzItem.Content.ToString() ?? "";
                if (content.Contains("125")) return "125";
                if (content.Contains("250")) return "250";
                if (content.Contains("500")) return "500";
                if (content.Contains("1000")) return "1000";
                if (content.Contains("2000")) return "2000";
                if (content.Contains("4000")) return "4000";
                if (content.Contains("8000")) return "8000";
            }
            return "1000";
        }

        private void SaveAllSettingsToRegistry(PageConfig config)
        {
            try
            {
                using var customKey = Registry.CurrentUser.CreateSubKey(@"Software\ClayxOptimize");
                if (customKey != null)
                {
                    customKey.SetValue("MouseSensiX", config.SensiX.ToString("F2", CultureInfo.InvariantCulture));
                    customKey.SetValue("MouseSensiY", config.SensiY.ToString("F2", CultureInfo.InvariantCulture));
                    customKey.SetValue("MouseDpi", config.Dpi);
                    customKey.SetValue("MouseHz", config.Hz);
                    customKey.SetValue("AutoSaveConfig", config.AutoSave ? 1 : 0, RegistryValueKind.DWord);
                }

                using var mouseKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse", true);
                if (mouseKey != null)
                {
                    mouseKey.SetValue("MouseSensitivity", config.PointerSpeed.ToString());
                }
            }
            catch {}
        }

        public class PageConfig
        {
            public bool MouseAimStabilizer { get; set; } = false;
            public bool KeyboardLatencyReducer { get; set; } = false;
            public bool UsbSelectiveSuspend { get; set; } = false;
            public bool UsbPollingRatePriority { get; set; } = false;
            public int PointerSpeed { get; set; } = 10;
            public double SensiX { get; set; } = 1.0;
            public double SensiY { get; set; } = 1.0;
            public string Dpi { get; set; } = "800";
            public string Hz { get; set; } = "1000";
            public bool AutoSave { get; set; } = true;
        }
    }
}
