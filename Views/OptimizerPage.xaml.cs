using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class OptimizerPage : Page
    {
        private readonly OptimizerEngine _optimizerEngine;

        public OptimizerPage()
        {
            InitializeComponent();
            _optimizerEngine = new OptimizerEngine();
            _optimizerEngine.OnLogMessage += AppendLog;
            
            UpdateCardSelectionVisuals();
            Loaded += OptimizerPage_Loaded;
        }

        private void OptimizerPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguageTranslations();
        }

        private void ApplyLanguageTranslations()
        {
            TranslationService.LoadLanguage();

            PageHeader.Text = TranslationService.Get("Opt_Title");
            PageSubtitle.Text = TranslationService.Get("Opt_Subtitle");
            ProfileTitle.Text = TranslationService.Get("Opt_SelectProfile");

            // Profiles
            LowTitleText.Text = TranslationService.Get("Opt_LowTitle");
            LowDescText.Text = TranslationService.Get("Opt_LowDesc");
            MidTitleText.Text = TranslationService.Get("Opt_MidTitle");
            MidDescText.Text = TranslationService.Get("Opt_MidDesc");
            HighTitleText.Text = TranslationService.Get("Opt_HighTitle");
            HighDescText.Text = TranslationService.Get("Opt_HighDesc");

            // Extra
            ExtraTitleText.Text = TranslationService.Get("Opt_ExtraTitle");
            NetTitleText.Text = TranslationService.Get("Opt_ExtraNetTitle");
            NetDescText.Text = TranslationService.Get("Opt_ExtraNetDesc");
            TrimTitleText.Text = TranslationService.Get("Opt_ExtraTrimTitle");
            TrimDescText.Text = TranslationService.Get("Opt_ExtraTrimDesc");
            JunkTitleText.Text = TranslationService.Get("Opt_ExtraJunkTitle");
            JunkDescText.Text = TranslationService.Get("Opt_ExtraJunkDesc");

            ApplyBoostBtn.Content = TranslationService.Get("Opt_ApplyBtn");
            LogTitleText.Text = TranslationService.Get("Opt_LogTitle");
            LogBox.Text = TranslationService.Get("Opt_ReadyLog");
        }

        private void AppendLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                if (LogBox.Text == "Siap melakukan optimasi...")
                {
                    LogBox.Text = message;
                }
                else
                {
                    LogBox.AppendText("\n" + message);
                }
                LogScroll.ScrollToEnd();
            });
        }

        #region PROFILE SELECTIONS

        private void LowCard_Click(object sender, MouseButtonEventArgs e)
        {
            LowRadio.IsChecked = true;
            MidRadio.IsChecked = false;
            HighRadio.IsChecked = false;
            UpdateCardSelectionVisuals();
        }

        private void MidCard_Click(object sender, MouseButtonEventArgs e)
        {
            LowRadio.IsChecked = false;
            MidRadio.IsChecked = true;
            HighRadio.IsChecked = false;
            UpdateCardSelectionVisuals();
        }

        private void HighCard_Click(object sender, MouseButtonEventArgs e)
        {
            LowRadio.IsChecked = false;
            MidRadio.IsChecked = false;
            HighRadio.IsChecked = true;
            UpdateCardSelectionVisuals();
        }

        private void LowRadio_Click(object sender, RoutedEventArgs e)
        {
            MidRadio.IsChecked = false;
            HighRadio.IsChecked = false;
            UpdateCardSelectionVisuals();
        }

        private void MidRadio_Click(object sender, RoutedEventArgs e)
        {
            LowRadio.IsChecked = false;
            HighRadio.IsChecked = false;
            UpdateCardSelectionVisuals();
        }

        private void HighRadio_Click(object sender, RoutedEventArgs e)
        {
            LowRadio.IsChecked = false;
            MidRadio.IsChecked = false;
            UpdateCardSelectionVisuals();
        }

        private void UpdateCardSelectionVisuals()
        {
            var brushConverter = new BrushConverter();
            var activeBorder = (Brush)brushConverter.ConvertFromString("#00F2FE")!;
            var inactiveBorder = (Brush)brushConverter.ConvertFromString("#323C52")!;

            LowCard.BorderBrush = LowRadio.IsChecked == true ? activeBorder : inactiveBorder;
            MidCard.BorderBrush = MidRadio.IsChecked == true ? activeBorder : inactiveBorder;
            HighCard.BorderBrush = HighRadio.IsChecked == true ? activeBorder : inactiveBorder;
        }

        #endregion

        private async void ApplyBoostBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyBoostBtn.IsEnabled = false;
            ApplyBoostBtn.Content = "MEMPROSES OPTIMASI...";
            LogBox.Text = string.Empty;

            string selectedProfile = "mid";
            if (LowRadio.IsChecked == true) selectedProfile = "low";
            else if (HighRadio.IsChecked == true) selectedProfile = "high";

            AppendLog($"*** MEMULAI PROSES OPTIMASI PC (Profil: {selectedProfile.ToUpper()}) ***");

            try
            {
                // 1. Apply Selected System Profile (Registry, Services, Power Scheme)
                await _optimizerEngine.ApplyOptimizationProfileAsync(selectedProfile);

                // 2. RAM Booster
                await _optimizerEngine.BoostRamAsync();

                // 3. Optional: Network Tweak
                if (NetworkChk.IsChecked == true)
                {
                    await _optimizerEngine.OptimizeNetworkPingAsync();
                }

                // 4. Optional: SSD TRIM reindexing
                if (TrimChk.IsChecked == true)
                {
                    await _optimizerEngine.OptimizeStorageTrimAsync();
                }

                // 5. Optional: Deep Junk Cleaner
                if (DeepJunkChk.IsChecked == true)
                {
                    await _optimizerEngine.CleanJunkFilesAsync(
                        cleanTemp: true,
                        cleanPrefetch: true,
                        cleanRecycleBin: true,
                        cleanLogs: true
                    );
                }

                AppendLog("*** SEMUA PROSES OPTIMASI SELESAI DENGAN SUKSES! ***");
                MessageBox.Show("Sistem Anda berhasil dioptimalkan sepenuhnya!", "Optimasi Berhasil", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"[ERROR] Terjadi kesalahan: {ex.Message}");
                MessageBox.Show($"Gagal melakukan beberapa langkah optimasi: {ex.Message}", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                ApplyBoostBtn.IsEnabled = true;
                ApplyBoostBtn.Content = "TERAPKAN PENINGKATAN PC";
            }
        }
    }
}
