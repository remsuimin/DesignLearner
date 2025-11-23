using DesignPatternMaster.UI.ViewModels;
using System.Windows.Controls;

namespace DesignPatternMaster.UI.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;  // Fixed: bind to ViewModel
            InitializeComponent();
            
            // Data is loaded by MainWindow.Navigate when the DashboardPage is created.
        }
    }
}
