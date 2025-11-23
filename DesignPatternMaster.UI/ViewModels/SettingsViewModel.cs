using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Reflection;

namespace DesignPatternMaster.UI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _selectedTheme = "Dark";

        [ObservableProperty]
        private double _codeFontSize = 14;

        [ObservableProperty]
        private string _version = "1.0.0";

        public SettingsViewModel()
        {
            // Get version from assembly
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            Version = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";
        }

        [RelayCommand]
        private void ResetData()
        {
            System.Diagnostics.Debug.WriteLine("Reset data requested");
            // Placeholder for reset functionality
            System.Windows.MessageBox.Show(
                "データのリセット機能は今後実装予定です。", 
                "情報", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Information);
        }

        [RelayCommand]
        private void OpenGitHub()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open URL: {ex.Message}");
            }
        }
    }
}
