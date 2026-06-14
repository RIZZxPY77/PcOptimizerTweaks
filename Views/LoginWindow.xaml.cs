using System;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Load and apply translations
            TranslationService.LoadLanguage();
            
            Title = TranslationService.Get("Login_Title");
            TitleBarText.Text = TranslationService.Get("Login_DragBar");
            BrandTitleText.Text = TranslationService.Get("Login_Brand1");
            BrandSubText.Text = TranslationService.Get("Login_Brand2");
            WarningTitleText.Text = TranslationService.Get("Login_WarningTitle");
            WarningBodyText.Text = TranslationService.Get("Login_WarningBody");
            InputLabelText.Text = TranslationService.Get("Login_InputLabel");
            KeyTextBox.PlaceholderText = TranslationService.Get("Login_Placeholder");
            LoginBtn.Content = TranslationService.Get("Login_Btn");
            FooterText.Text = TranslationService.Get("Login_Footer");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var key = KeyTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show(TranslationService.Get("Login_EmptyKey"), TranslationService.Get("Menu_dashboard"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Attempt login via KeyAuth
            bool loginSuccess = await PCOptimizer.Services.KeyAuthService.LoginAsync(key);
            if (!loginSuccess)
            {
                MessageBox.Show(PCOptimizer.Services.KeyAuthService.LastErrorMessage, "Gagal Masuk", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verify license (expiry, HWID)
            if (!PCOptimizer.Services.KeyAuthService.VerifyLicense(out var error))
            {
                MessageBox.Show(error, "Verifikasi Lisensi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Welcome popup
            MessageBox.Show("Welcome! dev By RizzX dev of CLAYX-TEAM", "CLAYX-TEAM VIP", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
