using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class DriversPage : Page
    {
        private readonly DriverDetector _driverDetector;

        public DriversPage()
        {
            InitializeComponent();
            _driverDetector = new DriverDetector();

            Loaded += DriversPage_Loaded;
        }

        private void DriversPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDriversAsync();
        }

        private async void LoadDriversAsync()
        {
            // Query WMI in background to avoid any minor UI freezing
            await Task.Run(() =>
            {
                var drivers = _driverDetector.GetSystemDrivers();
                Dispatcher.Invoke(() =>
                {
                    DriversListView.ItemsSource = drivers;
                });
            });
        }

        private void ScanDriversBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadDriversAsync();
            MessageBox.Show("Pemindaian driver hardware selesai! Menampilkan versi saat ini.", "Pindai Selesai", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenNvidiaLink_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://www.nvidia.com/Download/index.aspx");
        }

        private void OpenAmdLink_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://www.amd.com/en/support");
        }

        private void OpenIntelLink_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://www.intel.com/content/www/us/en/support/intel-driver-support-assistant.html");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tidak dapat membuka browser: {ex.Message}", "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
