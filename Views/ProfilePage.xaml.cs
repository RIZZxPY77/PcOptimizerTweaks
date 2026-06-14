using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class ProfilePage : Page
    {
        private DispatcherTimer? _countdownTimer;

        public ProfilePage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => ApplyLanguageTranslations();
            LoadProfileData();
            StartCountdown();
            this.Unloaded += ProfilePage_Unloaded;
        }

        private void ApplyLanguageTranslations()
        {
            TranslationService.LoadLanguage();

            PageTitleText.Text = TranslationService.Get("Prof_Title");
            PageSubtitleText.Text = TranslationService.Get("Prof_Subtitle");
            AccountTypeLabel.Text = TranslationService.Get("Prof_AccountType");
            AccountTypeValue.Text = TranslationService.Get("Prof_PremiumAccess");
            ServerStatusLabel.Text = TranslationService.Get("Prof_ServerStatus");
            ServerStatusValue.Text = TranslationService.Get("Prof_Connected");
            ActiveDurationLabel.Text = TranslationService.Get("Prof_ActiveDuration");
            SubSysInfoTitle.Text = TranslationService.Get("Prof_SubSysInfo");
            LicenseDetailsHeader.Text = TranslationService.Get("Prof_DetailsHeader");
            LicenseCodeLabel.Text = TranslationService.Get("Prof_LicenseCode");
            ExpiryDateLabel.Text = TranslationService.Get("Prof_Expiry");
            VerificationStatusLabel.Text = TranslationService.Get("Prof_Verify");
            VerificationStatusValue.Text = TranslationService.Get("Prof_Verified");
            DeviceDetailsHeader.Text = TranslationService.Get("Prof_DeviceHeader");
            CopyHwidBtn.Content = TranslationService.Get("Prof_CopyBtn");
            ComputerNameLabel.Text = TranslationService.Get("Prof_CompName");
            LocalIpLabel.Text = TranslationService.Get("Prof_LocalIp");
            OsInfoHeader.Text = TranslationService.Get("Prof_OsInfo");
            OsVersionLabel.Text = TranslationService.Get("Prof_OsVersionLabel");
        }

        private void LoadProfileData()
        {
            try
            {
                var license = KeyAuthService.CurrentLicense;
                if (license != null)
                {
                    LicenseKeyHeaderTxt.Text = FormatKeyHeader(license.Key);
                    LicenseKeyTxt.Text = license.Key;
                    HwidTxt.Text = license.HWID;
                    PcNameTxt.Text = license.PCName;
                    IpAddressTxt.Text = license.IP;

                    // Display friendly formatted expiry date
                    ExpiryTxt.Text = license.Expiry.ToString("dd MMMM yyyy, HH:mm:ss");
                }
                else
                {
                    // Fallback if license data isn't loaded
                    var hwid = KeyAuthService.GetCurrentHWID();
                    LicenseKeyHeaderTxt.Text = TranslationService.Get("Prof_Verified");
                    LicenseKeyTxt.Text = "CLAYX-VIP-PREMIUM";
                    HwidTxt.Text = hwid;
                    PcNameTxt.Text = Environment.MachineName;
                    IpAddressTxt.Text = "127.0.0.1";
                    ExpiryTxt.Text = TranslationService.Get("Prof_Lifetime");
                }

                // Friendly OS Description (e.g. Microsoft Windows 11 Pro)
                OsVersionTxt.Text = RuntimeInformation.OSDescription;
            }
            catch (Exception)
            {
                // Soft fallback
                LicenseKeyHeaderTxt.Text = "ERROR LOAD";
                LicenseKeyTxt.Text = "CLAYX-ERR-01";
                HwidTxt.Text = "unknown-hwid";
                PcNameTxt.Text = Environment.MachineName;
                IpAddressTxt.Text = "unknown";
                OsVersionTxt.Text = Environment.OSVersion.ToString();
                ExpiryTxt.Text = "unknown";
            }
        }

        private string FormatKeyHeader(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "GUEST USER";
            if (key.Length > 12)
            {
                return key.Substring(0, 4) + "-XXXX-" + key.Substring(key.Length - 4);
            }
            return key;
        }

        private void StartCountdown()
        {
            _countdownTimer = new DispatcherTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();
            // Call once immediately to set initial text
            CountdownTimer_Tick(this, EventArgs.Empty);
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                var license = KeyAuthService.CurrentLicense;
                if (license != null)
                {
                    var remaining = license.Expiry - DateTime.Now;
                    if (remaining.TotalSeconds <= 0)
                    {
                        ActiveDurationTxt.Text = TranslationService.Get("Prof_Expired");
                        ActiveDurationTxt.Foreground = System.Windows.Media.Brushes.Red;
                    }
                    else if (remaining.TotalDays > 365)
                    {
                        ActiveDurationTxt.Text = TranslationService.Get("Prof_Lifetime");
                    }
                    else
                    {
                        ActiveDurationTxt.Text = $"{remaining.Days} {TranslationService.Get("Prof_Days")}, " +
                                                 $"{remaining.Hours} {TranslationService.Get("Prof_Hours")}, " +
                                                 $"{remaining.Minutes} {TranslationService.Get("Prof_Minutes")}, " +
                                                 $"{remaining.Seconds} {TranslationService.Get("Prof_Seconds")}";
                    }
                }
                else
                {
                    ActiveDurationTxt.Text = TranslationService.Get("Prof_Lifetime");
                }
            }
            catch
            {
                ActiveDurationTxt.Text = TranslationService.Get("Prof_Lifetime");
            }
        }

        private void ProfilePage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Tick -= CountdownTimer_Tick;
            }
        }

        private void CopyHwid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(HwidTxt.Text);
                
                string title = TranslationService.CurrentLanguage == "id" ? "Salin Berhasil" 
                             : TranslationService.CurrentLanguage == "ja" ? "コピー成功" 
                             : TranslationService.CurrentLanguage == "zh" ? "复制成功" 
                             : TranslationService.CurrentLanguage == "ar" ? "تم النسخ بنجاح" 
                             : TranslationService.CurrentLanguage == "pt" ? "Copiado com Sucesso" 
                             : TranslationService.CurrentLanguage == "es" ? "Copia Exitosa" 
                             : TranslationService.CurrentLanguage == "ru" ? "Копирование успешно" 
                             : TranslationService.CurrentLanguage == "de" ? "Kopieren erfolgreich" 
                             : TranslationService.CurrentLanguage == "fr" ? "Copie réussie" 
                             : TranslationService.CurrentLanguage == "ko" ? "복사 성공" 
                             : TranslationService.CurrentLanguage == "tr" ? "Kopyalama Başarılı" 
                             : "Copy Success";
                             
                string body = TranslationService.CurrentLanguage == "id" ? "Hardware ID (HWID) disalin ke clipboard!" 
                            : TranslationService.CurrentLanguage == "ja" ? "Hardware ID (HWID) がクリップボードにコピーされました！" 
                            : TranslationService.CurrentLanguage == "zh" ? "硬件ID (HWID) 已复制到剪贴板！" 
                            : TranslationService.CurrentLanguage == "ar" ? "تم نسخ معرف الأجهزة (HWID) إلى الحافظة!" 
                            : TranslationService.CurrentLanguage == "pt" ? "Hardware ID (HWID) copiado para a área de transferência!" 
                            : TranslationService.CurrentLanguage == "es" ? "¡El ID de hardware (HWID) se ha copiado al portapapeles!" 
                            : TranslationService.CurrentLanguage == "ru" ? "Аппаратный ID (HWID) скопирован в буфер обмена!" 
                            : TranslationService.CurrentLanguage == "de" ? "Hardware-ID (HWID) wurde in die Zwischenablage kopiert!" 
                            : TranslationService.CurrentLanguage == "fr" ? "L'ID matériel (HWID) a été copié dans le presse-papiers !" 
                            : TranslationService.CurrentLanguage == "ko" ? "하드웨어 ID(HWID)가 클립보드에 복사되었습니다!" 
                            : TranslationService.CurrentLanguage == "tr" ? "Donanım Kimliği (HWID) panoya kopyalandı!" 
                            : "Hardware ID (HWID) copied to clipboard!";

                var mainWin = Application.Current.MainWindow as MainWindow;
                if (mainWin != null)
                {
                    mainWin.ShowToast(title, body, true);
                }
                else
                {
                    MessageBox.Show(body, title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyalin HWID: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
