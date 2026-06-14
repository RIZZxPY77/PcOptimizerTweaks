using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using PCOptimizer.Models;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class GameModePage : Page
    {
        private readonly GameDetector _gameDetector;

        public GameModePage()
        {
            InitializeComponent();
            _gameDetector = new GameDetector();
            
            Loaded += GameModePage_Loaded;
        }

        private void GameModePage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguageTranslations();
            RefreshGamesGrid();
        }

        private void ApplyLanguageTranslations()
        {
            TranslationService.LoadLanguage();

            PageTitleText.Text = TranslationService.Get("Game_Title");
            PageSubtitleText.Text = TranslationService.Get("Game_Subtitle");
            LibraryTitleText.Text = TranslationService.Get("Game_LibTitle");
            AddGameBtn.Content = TranslationService.Get("Game_Add");
        }

        private void RefreshGamesGrid()
        {
            try
            {
                var list = _gameDetector.GetDetectedGames();
                GamesGridItemsControl.ItemsSource = null;
                GamesGridItemsControl.ItemsSource = list;
            }
            catch {}
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshGamesGrid();
        }

        private void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe",
                Title = "Pilih Executable Game Anda"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                string name = Path.GetFileNameWithoutExtension(path);
                
                _gameDetector.AddCustomGame(name, path);
                RefreshGamesGrid();
                MessageBox.Show($"Game '{name}' berhasil ditambahkan ke daftar library!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GamePoster_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is GameItem game)
            {
                NavigationService?.Navigate(new GameDetailPage(game));
            }
        }

        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is GameItem game)
            {
                try
                {
                    if (File.Exists(game.FilePath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = game.FilePath,
                            UseShellExecute = true,
                            WorkingDirectory = Path.GetDirectoryName(game.FilePath) ?? string.Empty
                        });
                        
                        var mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow?.ShowToast("Game Launched", $"Meluncurkan {game.Name}...", true);
                        
                        game.IsRunning = true;
                        GamesGridItemsControl.Items.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Game executable tidak ditemukan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gagal meluncurkan game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OptimizeGame_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is GameItem game)
            {
                game.IsOptimized = !game.IsOptimized;
                GamesGridItemsControl.Items.Refresh();
                
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (game.IsOptimized)
                {
                    mainWindow?.ShowToast("Game Optimized", $"Profil optimization diterapkan untuk {game.Name}", true);
                }
                else
                {
                    // Use a warning or neutral icon/color for restoring default, pass true for blue/green, false for red
                    mainWindow?.ShowToast("Optimization Removed", $"Profil default dikembalikan untuk {game.Name}", false);
                }
            }
        }
    }
}
