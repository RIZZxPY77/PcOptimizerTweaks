using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PCOptimizer.Views;
using PCOptimizer.Services;

namespace PCOptimizer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                ((App)Application.Current).LogStartup("MainWindow InitializeComponent succeeded.");
            }
            catch (Exception ex)
            {
                ((App)Application.Current).LogCrash(ex, "MainWindow constructor InitializeComponent");
                MessageBox.Show($"Error initializing MainWindow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initial navigation to Dashboard Page with error handling
            try
            {
                ApplyLanguageTranslations();
                RootNavigation.Navigate(typeof(DashboardPage));
                // Log successful navigation
                ((App)Application.Current).LogStartup("Navigated to DashboardPage successfully.");
            }
            catch (Exception ex)
            {
                ((App)Application.Current).LogCrash(ex, "MainWindow_Loaded navigation");
                MessageBox.Show($"Failed to load dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ApplyLanguageTranslations()
        {
            TranslationService.LoadLanguage();
            
            // Translate Window titles and static badges
            Title = TranslationService.Get("Dash_Title");
            TitleBarText.Text = TranslationService.Get("Dash_Title");
            
            // Translate sidebar menu items
            foreach (var menuItem in RootNavigation.MenuItems)
            {
                if (menuItem is Wpf.Ui.Controls.NavigationViewItem item && item.Tag is string tag)
                {
                    item.Content = TranslationService.Get("Menu_" + tag);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Delete)
            {
                var focused = Keyboard.FocusedElement;
                if (focused is System.Windows.Controls.TextBox || focused is System.Windows.Controls.PasswordBox)
                {
                    return; // Ignore close hotkey if user is typing
                }

                ((App)Application.Current).LogStartup("Close hotkey (Delete) pressed. Closing MainWindow.");
                this.Close();
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void DoNothing(object sender, RoutedEventArgs e)
        {
            // Intentionally left blank to disable close action.
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(AboutPage));
        }

        public void ShowToast(string title, string message, bool isSuccess = true)
        {
            Dispatcher.Invoke(() =>
            {
                ToastTitle.Text = title;
                ToastMsg.Text = message;
                ToastIcon.Symbol = isSuccess ? Wpf.Ui.Controls.SymbolRegular.CheckmarkCircle24 : Wpf.Ui.Controls.SymbolRegular.ErrorCircle24;
                
                // Color configuration
                var greenBrush = (Brush)new BrushConverter().ConvertFromString("#00E676")!;
                var redBrush = (Brush)new BrushConverter().ConvertFromString("#FF1744")!;
                var blueBrush = (Brush)new BrushConverter().ConvertFromString("#00F2FE")!;
                
                ToastIcon.Foreground = isSuccess ? greenBrush : redBrush;
                NotificationToast.BorderBrush = isSuccess ? blueBrush : redBrush;
                
                NotificationToast.Visibility = Visibility.Visible;

                var sb = new Storyboard();

                // Opacity animation
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
                Storyboard.SetTarget(fadeIn, NotificationToast);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));

                // Translation Y animation
                var slideUp = new DoubleAnimation(50, 0, TimeSpan.FromMilliseconds(250));
                Storyboard.SetTarget(slideUp, ToastTranslate);
                Storyboard.SetTargetProperty(slideUp, new PropertyPath("Y"));

                sb.Children.Add(fadeIn);
                sb.Children.Add(slideUp);

                // Auto fade-out after 3 seconds
                sb.Completed += (s, ev) =>
                {
                    Task.Delay(3000).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var sbOut = new Storyboard();
                            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));
                            Storyboard.SetTarget(fadeOut, NotificationToast);
                            Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));

                            var slideDown = new DoubleAnimation(0, 50, TimeSpan.FromMilliseconds(250));
                            Storyboard.SetTarget(slideDown, ToastTranslate);
                            Storyboard.SetTargetProperty(slideDown, new PropertyPath("Y"));

                            sbOut.Children.Add(fadeOut);
                            sbOut.Children.Add(slideDown);
                            sbOut.Completed += (s2, e2) => { NotificationToast.Visibility = Visibility.Collapsed; };
                            sbOut.Begin();
                        });
                    });
                };

                sb.Begin();
            });
        }
    }
}