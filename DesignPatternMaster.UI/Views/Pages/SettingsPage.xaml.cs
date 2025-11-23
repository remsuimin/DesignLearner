using DesignPatternMaster.UI.ViewModels;
using System.Windows.Controls;

namespace DesignPatternMaster.UI.Views.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
