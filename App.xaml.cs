using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;

namespace PCOptimizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MediaPlayer _startupPlayer; // MediaPlayer for startup sound
        private const string CrashLogPath = "logs/crash.log";
        private const string StartupLogPath = "logs/startup.log";

        protected override void OnStartup(StartupEventArgs e)
        {
            // Log entry at start of OnStartup
            LogStartup("OnStartup begin");
            base.OnStartup(e);
            EnsureLogDirectory();
            LogStartup("Application starting...");

            // Global exception handlers (retain)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Initialize Security Guard Protection: anti-debug, anti-dump, background anti-tamper threads
            try
            {
                PCOptimizer.Services.SecurityGuard.Initialize();
                LogStartup("Security Guard initialized.");
            }
            catch (Exception ex)
            {
                LogCrash(ex, "SecurityGuard.Initialize");
            }

            // Set shutdown mode to explicit during loading and login dialogs
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            // Play startup sound (if file exists)
            try
            {
                PlayStartupSound();
                LogStartup("Startup sound played.");
            }
            catch (Exception ex)
            {
                LogCrash(ex, "PlayStartupSound");
            }

            // 1. Show LoadingWindow
            try
            {
                LogStartup("Opening LoadingWindow...");
                var loading = new Views.LoadingWindow();
                loading.ShowDialog();
                LogStartup("LoadingWindow completed.");
            }
            catch (Exception ex)
            {
                LogCrash(ex, "LoadingWindow ShowDialog");
            }

            // Check KeyAuth client initialization status (which was run inside LoadingWindow)
            if (string.IsNullOrEmpty(Services.KeyAuthService.KeyAuthApp.sessionid))
            {
                var errMsg = string.IsNullOrEmpty(Services.KeyAuthService.LastErrorMessage) 
                    ? "Gagal terhubung ke server lisensi. Silakan periksa koneksi internet Anda." 
                    : Services.KeyAuthService.LastErrorMessage;
                
                MessageBox.Show(errMsg, "Inisialisasi Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // 2. Show LoginWindow
            bool loginSuccessful = false;
            try
            {
                LogStartup("Opening LoginWindow...");
                var login = new Views.LoginWindow();
                var dialogResult = login.ShowDialog();
                loginSuccessful = dialogResult == true;
                LogStartup($"LoginWindow result: {loginSuccessful}");
            }
            catch (Exception ex)
            {
                LogCrash(ex, "LoginWindow ShowDialog");
            }

            if (!loginSuccessful)
            {
                LogStartup("Login was not successful or cancelled. Shutting down.");
                Shutdown();
                return;
            }

            // 3. Open MainWindow
            try
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                LogStartup("Opening MainWindow...");
                var main = new MainWindow();
                MainWindow = main;
                main.Show();
                LogStartup("MainWindow displayed successfully.");
            }
            catch (Exception ex)
            {
                LogCrash(ex, "App.OnStartup (MainWindow Show)");
                MessageBox.Show(
                    $"Critical error during startup:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Startup Failure",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void EnsureLogDirectory()
        {
            try
            {
                var dirs = new[] {
                    Path.GetDirectoryName(CrashLogPath),
                    Path.GetDirectoryName(StartupLogPath)
                };
                foreach (var d in dirs)
                {
                    if (!string.IsNullOrEmpty(d) && !Directory.Exists(d))
                        Directory.CreateDirectory(d);
                }
            }
            catch { /* ignore */ }
        }

        public void LogStartup(string message)
        {
            try
            {
                File.AppendAllText(StartupLogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch { /* ignore */ }
        }

        public void LogCrash(Exception ex, string source)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Source: {source}");
                sb.AppendLine(ex.ToString());
                sb.AppendLine(new string('-', 80));
                File.AppendAllText(CrashLogPath, sb.ToString());
            }
            catch { /* ignore */ }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogCrash(ex, "AppDomain.UnhandledException");
                ShowErrorAndContinue("An unexpected error occurred. The application will continue running.");
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogCrash(e.Exception, "DispatcherUnhandledException");
            e.Handled = true;
            ShowErrorAndContinue("An error occurred in the UI thread. The application will continue running.");
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            // Handle background task errors

            LogCrash(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
            ShowErrorAndContinue("A background task failed. The application will continue running.");
        }

        private void ShowErrorAndContinue(string message)
        {
            // Show a simple MessageBox; avoid throwing further exceptions.
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Play the startup sound using MediaPlayer (lightweight, non-UI)
        private void PlayStartupSound()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Assets/startup.wav", UriKind.Absolute);
                _startupPlayer = new MediaPlayer();
                _startupPlayer.Open(uri);
                _startupPlayer.Volume = 0.6;
                _startupPlayer.Play();
            }
            catch
            {
                // Ignore if sound file missing or playback fails
            }
        }
    }
}
