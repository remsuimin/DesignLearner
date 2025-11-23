using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.UseCases.Queries;
using System.Collections.ObjectModel;
using System.Threading.Tasks;



using CommunityToolkit.Mvvm.Messaging;

namespace DesignPatternMaster.UI.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly GetPatternListQuery _getPatternListQuery;

        [ObservableProperty]
        private ObservableCollection<DesignPattern> _patterns;

        public DashboardViewModel(GetPatternListQuery getPatternListQuery)
        {
            _getPatternListQuery = getPatternListQuery;
            Patterns = new ObservableCollection<DesignPattern>();
        }

        public async Task LoadDataAsync()
        {
            var patterns = await _getPatternListQuery.ExecuteAsync();
            Patterns.Clear();
            foreach (var pattern in patterns)
            {
                Patterns.Add(pattern);
            }
            try
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard patterns loaded: {Patterns.Count}");
                System.IO.File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DesignPatternMaster.log"), $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Dashboard patterns loaded: {Patterns.Count}\r\n");
            }
            catch { }
        }

        [RelayCommand]
        private void NavigateToDetail(DesignPattern pattern)
        {
            if (pattern != null)
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as Views.MainWindow;
                if (mainWindow != null)
                {
                    // Navigate with parameter
                    mainWindow.Navigate(typeof(Views.Pages.PatternDetailPage), pattern.Id);
                }
            }
        }
    }

}
