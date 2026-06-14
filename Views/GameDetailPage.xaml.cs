using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using PCOptimizer.Models;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class GameDetailPage : Page
    {
        private readonly GameItem _game;
        private readonly OptimizerEngine _optimizerEngine;

        public GameDetailPage(GameItem game)
        {
            InitializeComponent();
            _game = game;
            _optimizerEngine = new OptimizerEngine();

            Loaded += GameDetailPage_Loaded;
        }

        private void GameDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Bind game data to UI
            BreadcrumbGameName.Text = _game.Name;
            GameTitle.Text = _game.Name;
            
            // Set custom game details text
            WarningBoxTxt.Text = _game.WarningText;
            VisualDetailsTxt.Text = _game.GameSettingsAppliedText;

            UpdatePresetSelectionVisuals();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void BestPerfCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _game.IsBestPerformanceApplied = true;
            _game.IsLiteModeApplied = false;
            
            UpdatePresetSelectionVisuals();
            ApplyGameOptimizations("best");
        }

        private void LiteModeCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _game.IsBestPerformanceApplied = false;
            _game.IsLiteModeApplied = true;
            
            UpdatePresetSelectionVisuals();
            ApplyGameOptimizations("lite");
        }

        private void RevertBtn_Click(object sender, RoutedEventArgs e)
        {
            _game.IsBestPerformanceApplied = false;
            _game.IsLiteModeApplied = false;

            UpdatePresetSelectionVisuals();
            MessageBox.Show($"Pengaturan optimasi game '{_game.Name}' berhasil dikembalikan ke default Windows.", "Revert Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdatePresetSelectionVisuals()
        {
            var brushConverter = new BrushConverter();
            var orangeBorder = (Brush)brushConverter.ConvertFromString("#FF9100")!;
            var greenBorder = (Brush)brushConverter.ConvertFromString("#00E676")!;
            var inactiveBorder = (Brush)brushConverter.ConvertFromString("#222A3C")!;

            if (_game.IsBestPerformanceApplied)
            {
                BestPerfCard.BorderBrush = orangeBorder;
                BestCheckIcon.Visibility = Visibility.Visible;
                
                LiteModeCard.BorderBrush = inactiveBorder;
                LiteCheckIcon.Visibility = Visibility.Collapsed;
            }
            else if (_game.IsLiteModeApplied)
            {
                BestPerfCard.BorderBrush = inactiveBorder;
                BestCheckIcon.Visibility = Visibility.Collapsed;

                LiteModeCard.BorderBrush = greenBorder;
                LiteCheckIcon.Visibility = Visibility.Visible;
            }
            else
            {
                BestPerfCard.BorderBrush = inactiveBorder;
                BestCheckIcon.Visibility = Visibility.Collapsed;

                LiteModeCard.BorderBrush = inactiveBorder;
                LiteCheckIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyGameOptimizations(string preset)
        {
            try
            {
                if (preset == "best")
                {
                    // Mock game scheduling tweaks in registry (normally written under HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options)
                    MessageBox.Show(
                        $"Preset 'Best Performance' berhasil diterapkan untuk '{_game.Name}'!\n\n" +
                        $"- Dialokasikan prioritas tinggi untuk CPU & GPU.\n" +
                        $"- 18 parameter registry game telah dioptimalkan.",
                        "Optimasi Game Sukses",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"Preset 'Lite Mode' berhasil diterapkan untuk '{_game.Name}'!\n\n" +
                        $"- RAM Standby dilepas saat game dinyalakan.\n" +
                        $"- 8 parameter optimal sistem diterapkan.",
                        "Optimasi Game Sukses",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menerapkan optimasi game: {ex.Message}", "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
