using DesignPatternMaster.UI.ViewModels;
using System.Windows.Controls;

namespace DesignPatternMaster.UI.Views.Pages
{
    public partial class PatternDetailPage : Page
    {
        public PatternDetailViewModel ViewModel { get; }

        public PatternDetailPage(PatternDetailViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel; // Bind directly to viewModel
            InitializeComponent();
        }
    }
}
