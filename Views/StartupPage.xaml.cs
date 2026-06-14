using System;
using System.Windows;
using System.Windows.Controls;
using PCOptimizer.Services;
using PCOptimizer.Models;
using Wpf.Ui.Controls;

namespace PCOptimizer.Views
{
    public partial class StartupPage : Page
    {
        private readonly OptimizerEngine _optimizerEngine;
        private bool _isUpdatingList;

        public StartupPage()
        {
            InitializeComponent();
            _optimizerEngine = new OptimizerEngine();
            
            Loaded += StartupPage_Loaded;
        }

        private void StartupPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguageTranslations();
            RefreshStartupList();
        }

        private void ApplyLanguageTranslations()
        {
            PageTitleText.Text = TranslationService.Get("Start_Title");
            PageSubtitleText.Text = TranslationService.Get("Start_Subtitle");
            RefreshBtn.Content = TranslationService.Get("Start_ScanBtn");
        }

        private void RefreshStartupList()
        {
            _isUpdatingList = true;
            try
            {
                var list = _optimizerEngine.GetStartupItems();
                StartupListView.ItemsSource = list;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Gagal memuat daftar startup: {ex.Message}", "Kesalahan", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                _isUpdatingList = false;
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshStartupList();
        }

        private void StartupToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingList) return;

            if (sender is FrameworkElement element && element.DataContext is StartupItem item)
            {
                bool success = _optimizerEngine.ToggleStartupItem(item, true);
                if (!success)
                {
                    // Revert UI toggle
                    _isUpdatingList = true;
                    if (sender is ToggleSwitch toggle) toggle.IsChecked = false;
                    _isUpdatingList = false;
                    System.Windows.MessageBox.Show("Gagal mengaktifkan item startup. Memerlukan hak Administrator.", "Kesalahan", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                else
                {
                    item.IsEnabled = true;
                }
            }
        }

        private void StartupToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingList) return;

            if (sender is FrameworkElement element && element.DataContext is StartupItem item)
            {
                bool success = _optimizerEngine.ToggleStartupItem(item, false);
                if (!success)
                {
                    // Revert UI toggle
                    _isUpdatingList = true;
                    if (sender is ToggleSwitch toggle) toggle.IsChecked = true;
                    _isUpdatingList = false;
                    System.Windows.MessageBox.Show("Gagal menonaktifkan item startup. Memerlukan hak Administrator.", "Kesalahan", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                else
                {
                    item.IsEnabled = false;
                }
            }
        }
    }
}
