using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DesignPatternMaster.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "Design Pattern Master";

        public MainWindowViewModel()
        {
        }

        [RelayCommand]
        private void Navigate(string pageName)
        {
            // Simple navigation logic for now
            var mainWindow = System.Windows.Application.Current.MainWindow as Views.MainWindow;
            if (mainWindow != null)
            {
                if (pageName == "Dashboard")
                {
                    mainWindow.Navigate(typeof(Views.Pages.DashboardPage));
                }
                else if (pageName == "Settings")
                {
                    mainWindow.Navigate(typeof(Views.Pages.SettingsPage));
                }
            }
        }
    }
}
