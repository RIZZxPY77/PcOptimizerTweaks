using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class EmulatorPage : Page
    {
        private readonly EmulatorOptimizer _emuOptimizer;
        private readonly HardwareDetector _hardwareDetector;
        private readonly DispatcherTimer _emuTimer;

        public EmulatorPage()
        {
            InitializeComponent();
            _emuOptimizer = new EmulatorOptimizer();
            _hardwareDetector = new HardwareDetector();

            // Setup timer to refresh active stats and list
            _emuTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            _emuTimer.Tick += EmuTimer_Tick;

            Loaded += EmulatorPage_Loaded;
            Unloaded += EmulatorPage_Unloaded;
        }

        private void EmulatorPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguageTranslations();
            RefreshEmulatorList();
            _emuTimer.Start();
        }

        private void ApplyLanguageTranslations()
        {
            TranslationService.LoadLanguage();

            PageTitleText.Text = TranslationService.Get("Emu_Title");
            PageSubtitleText.Text = TranslationService.Get("Emu_Subtitle");
            LeftColumnTitleText.Text = TranslationService.Get("Emu_Detected");

            GpuTitleText.Text = TranslationService.Get("Emu_GpuTitle");
            GpuDescText.Text = TranslationService.Get("Emu_GpuDesc");
            GpuPriorityTitleText.Text = TranslationService.Get("Emu_GpuAc");
            GpuPriorityDescText.Text = TranslationService.Get("Emu_GpuAcDesc");
            CpuPolicyTitleText.Text = TranslationService.Get("Emu_CpuPolicy");
            CpuPolicyDescText.Text = TranslationService.Get("Emu_CpuPolicyDesc");

            NoActiveTitleText.Text = TranslationService.Get("Emu_NoActiveTitle");
            NoActiveDescText.Text = TranslationService.Get("Emu_NoActiveDesc");

            CheckVirtualizationStatus();
        }

        private void EmulatorPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _emuTimer.Stop();
        }

        private void EmuTimer_Tick(object? sender, EventArgs e)
        {
            RefreshEmulatorList();
        }

        private void CheckVirtualizationStatus()
        {
            try
            {
                bool virtEnabled = _hardwareDetector.IsVirtualizationEnabled();
                if (virtEnabled)
                {
                    VirtIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.ShieldCheckmark24;
                    VirtIcon.Foreground = System.Windows.Media.Brushes.Green;
                    VirtStatusTitle.Text = TranslationService.Get("Emu_VirtTitle");
                    VirtStatusTitle.Foreground = System.Windows.Media.Brushes.White;
                    VirtStatusDesc.Text = TranslationService.Get("Emu_VirtDesc");
                }
                else
                {
                    VirtIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.Warning24;
                    VirtIcon.Foreground = System.Windows.Media.Brushes.Red;
                    VirtStatusTitle.Text = TranslationService.Get("Emu_VirtTitleOff");
                    VirtStatusTitle.Foreground = System.Windows.Media.Brushes.Red;
                    VirtStatusDesc.Text = TranslationService.Get("Emu_VirtDescOff");
                }
            }
            catch {}
        }

        private void RefreshEmulatorList()
        {
            try
            {
                var list = _emuOptimizer.GetEmulatorsStatus();
                
                // Track scroll offset by avoiding nulling items source
                var selectedIndex = EmuListView.SelectedIndex;
                
                // Just force update
                EmuListView.ItemsSource = null;
                EmuListView.ItemsSource = list;
                EmuListView.SelectedIndex = selectedIndex;

                // Check if any emulator is running
                bool anyRunning = false;
                if (list != null)
                {
                    foreach (var emu in list)
                    {
                        if (emu.IsRunning)
                        {
                            anyRunning = true;
                            break;
                        }
                    }
                }

                if (anyRunning)
                {
                    EmuListView.Visibility = Visibility.Visible;
                    NoActiveEmulatorCard.Visibility = Visibility.Collapsed;
                }
                else
                {
                    EmuListView.Visibility = Visibility.Collapsed;
                    NoActiveEmulatorCard.Visibility = Visibility.Visible;
                }
            }
            catch {}
        }

        private void OptimizeEmuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is EmulatorOptimizer.EmulatorInfo emu)
            {
                bool success = _emuOptimizer.OptimizeEmulator(emu.ExecutableName, true, true);
                
                var mainWin = Application.Current.MainWindow as MainWindow;
                if (mainWin != null)
                {
                    if (success)
                    {
                        mainWin.ShowToast(emu.Name, "Optimasi sukses! Prioritas disetel ke High & Core Fisik dikunci.", true);
                    }
                    else
                    {
                        mainWin.ShowToast(emu.Name, "Gagal mengoptimasi emulator. Jalankan sebagai Administrator.", false);
                    }
                }
                
                RefreshEmulatorList();
            }
        }
    }
}
