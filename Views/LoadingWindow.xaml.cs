using System;
using System.Windows;
using System.Threading.Tasks;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class LoadingWindow : Window
    {
        private string _selectedLanguage = "id";

        public LoadingWindow()
        {
            InitializeComponent();
        }

        private void IndonesianBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "id";
            TransitionToLoading();
        }

        private void EnglishBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "en";
            TransitionToLoading();
        }

        private void JapaneseBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "ja";
            TransitionToLoading();
        }

        private void ChineseBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "zh";
            TransitionToLoading();
        }

        private void ArabicBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "ar";
            TransitionToLoading();
        }

        private void PortugueseBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "pt";
            TransitionToLoading();
        }

        private void SpanishBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "es";
            TransitionToLoading();
        }

        private void RussianBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "ru";
            TransitionToLoading();
        }

        private void GermanBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "de";
            TransitionToLoading();
        }

        private void FrenchBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "fr";
            TransitionToLoading();
        }

        private void KoreanBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "ko";
            TransitionToLoading();
        }

        private void TurkishBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedLanguage = "tr";
            TransitionToLoading();
        }

        private void TransitionToLoading()
        {
            TranslationService.CurrentLanguage = _selectedLanguage;
            LanguageSelectionPanel.Visibility = Visibility.Collapsed;
            LoadingProgressPanel.Visibility = Visibility.Visible;
            StartLoadingSequence();
        }

        private async void StartLoadingSequence()
        {
            try
            {
                // Stage 1: Menyiapkan (Preparing)
                switch (_selectedLanguage)
                {
                    case "id":
                        StatusTitleText.Text = "MENYIAPKAN SISTEM";
                        StatusDescriptionText.Text = "Menyiapkan komponen dan antarmuka...";
                        break;
                    case "ja":
                        StatusTitleText.Text = "システムを準備中";
                        StatusDescriptionText.Text = "コンポーネントとインターフェースを準備しています...";
                        break;
                    case "zh":
                        StatusTitleText.Text = "正在准备系统";
                        StatusDescriptionText.Text = "正在准备组件和界面...";
                        break;
                    case "ar":
                        StatusTitleText.Text = "جاري تهيئة النظام";
                        StatusDescriptionText.Text = "جاري إعداد المكونات والواجهات...";
                        break;
                    case "pt":
                        StatusTitleText.Text = "PREPARANDO O SISTEMA";
                        StatusDescriptionText.Text = "Preparando componentes e interfaces...";
                        break;
                    case "es":
                        StatusTitleText.Text = "PREPARANDO EL SISTEMA";
                        StatusDescriptionText.Text = "Preparando componentes e interfaces...";
                        break;
                    case "ru":
                        StatusTitleText.Text = "ПОДГОТОВКА СИСТЕМЫ";
                        StatusDescriptionText.Text = "Подготовка компонентов и интерфейсов...";
                        break;
                    case "de":
                        StatusTitleText.Text = "SYSTEM WIRD VORBEREITET";
                        StatusDescriptionText.Text = "Komponenten und Schnittstellen werden vorbereitet...";
                        break;
                    case "fr":
                        StatusTitleText.Text = "PRÉPARATION DU SYSTÈME";
                        StatusDescriptionText.Text = "Préparation des composants et des interfaces...";
                        break;
                    case "ko":
                        StatusTitleText.Text = "시스템 준비 중";
                        StatusDescriptionText.Text = "구성 요소 및 인터페이스를 준비하는 중...";
                        break;
                    case "tr":
                        StatusTitleText.Text = "SİSTEM HAZIRLANIYOR";
                        StatusDescriptionText.Text = "Bileşenler ve arayüzler hazırlanıyor...";
                        break;
                    case "en":
                    default:
                        StatusTitleText.Text = "PREPARING SYSTEM";
                        StatusDescriptionText.Text = "Preparing components and interfaces...";
                        break;
                }
                await Task.Delay(1000);

                // Stage 2: Mengecek (Checking)
                switch (_selectedLanguage)
                {
                    case "id":
                        StatusTitleText.Text = "MENGECEK SERVER LISENSI";
                        StatusDescriptionText.Text = "Menghubungkan ke server lisensi VIP...";
                        break;
                    case "ja":
                        StatusTitleText.Text = "ライセンスサーバーを確認中";
                        StatusDescriptionText.Text = "VIPライセンスサーバーに接続しています...";
                        break;
                    case "zh":
                        StatusTitleText.Text = "正在检查授权服务器";
                        StatusDescriptionText.Text = "正在连接到VIP授权服务器...";
                        break;
                    case "ar":
                        StatusTitleText.Text = "التحقق من خادم الترخيص";
                        StatusDescriptionText.Text = "جاري الاتصال بخادم ترخيص VIP...";
                        break;
                    case "pt":
                        StatusTitleText.Text = "VERIFICANDO SERVIDOR DE LICENÇA";
                        StatusDescriptionText.Text = "Conectando ao servidor de licença VIP...";
                        break;
                    case "es":
                        StatusTitleText.Text = "VERIFICANDO EL SERVIDOR DE LICENCIA";
                        StatusDescriptionText.Text = "Conectando al servidor de licencia VIP...";
                        break;
                    case "ru":
                        StatusTitleText.Text = "ПРОВЕРКА СЕРВЕРА ЛИЦЕНЗИЙ";
                        StatusDescriptionText.Text = "Подключение к VIP-серверу лицензирования...";
                        break;
                    case "de":
                        StatusTitleText.Text = "LIZENZSERVER WIRD ÜBERPRÜFT";
                        StatusDescriptionText.Text = "Verbindung zum VIP-Lizenzserver wird hergestellt...";
                        break;
                    case "fr":
                        StatusTitleText.Text = "VÉRIFICATION DU SERVEUR DE LICENCE";
                        StatusDescriptionText.Text = "Connexion au serveur de licence VIP...";
                        break;
                    case "ko":
                        StatusTitleText.Text = "라이선스 서버 확인 중";
                        StatusDescriptionText.Text = "VIP 라이선스 서버에 연결하는 중...";
                        break;
                    case "tr":
                        StatusTitleText.Text = "LİSANS SUNUCUSU KONTROL EDİLİYOR";
                        StatusDescriptionText.Text = "VIP lisans sunucusuna bağlanılıyor...";
                        break;
                    case "en":
                    default:
                        StatusTitleText.Text = "CHECKING LICENSE SERVER";
                        StatusDescriptionText.Text = "Connecting to VIP license server...";
                        break;
                }

                // Run KeyAuth initialization and handle errors
                var initSuccess = await KeyAuthService.InitializeAsync();
                await Task.Delay(1200);
                if (!initSuccess)
                {
                    System.Windows.MessageBox.Show($"KeyAuth initialization failed: {KeyAuthService.LastErrorMessage}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    Close();
                    return;
                }
            }
            catch (Exception)
            {
                // Fallback catch (the exception is also logged in App domain)
            }
            finally
            {
                // Store language in Registry so keyauth service or profile page can read it if needed
                try
                {
                    using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\ClayxOptimize");
                    key?.SetValue("AppLanguage", _selectedLanguage);
                }
                catch {}

                Close();
            }
        }
    }
}
