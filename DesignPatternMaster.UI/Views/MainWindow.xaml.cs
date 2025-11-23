using DesignPatternMaster.UI.ViewModels;
using DesignPatternMaster.UI.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace DesignPatternMaster.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Views.MainWindow constructor start");
                DataContext = viewModel;
                InitializeComponent();
                
                System.Diagnostics.Debug.WriteLine("MainWindow initialized");
                System.Diagnostics.Debug.WriteLine("Views.MainWindow initialized");
            
                // Initial navigation to Dashboard - moved to Loaded event
                Loaded += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("Views.MainWindow Loaded");
                    NavigateToDashboard();
                };

                Closing += (s, e) => System.Diagnostics.Debug.WriteLine("Views.MainWindow Closing");

                Closed += (s, e) => System.Diagnostics.Debug.WriteLine("Views.MainWindow Closed");

                StateChanged += MainWindow_StateChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in MainWindow constructor: {ex.Message}");
                MessageBox.Show($"Error initializing MainWindow: {ex.Message}\n\n{ex.StackTrace}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MaximizeButton.Content = "❐"; // Restore icon
            }
            else
            {
                MaximizeButton.Content = "⬜"; // Maximize icon
            }
        }

        public void Navigate(Type pageType, object? parameter = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Navigating to: {pageType.Name}");
                Console.WriteLine($"Navigating to: {pageType.Name}");
                
                var page = App.ServiceProvider.GetRequiredService(pageType) as Page;
                if (page != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Page created: {page.GetType().Name}");
                    Console.WriteLine($"Page created: {page.GetType().Name}");
                    RootFrame.Navigate(page);
                    
                    // Load data if it's DashboardPage
                    if (page is DashboardPage dashboardPage)
                    {
                        var vm = App.ServiceProvider.GetRequiredService<DashboardViewModel>();
                        dashboardPage.DataContext = vm;
                        System.Diagnostics.Debug.WriteLine("Loading dashboard data...");
                        Console.WriteLine("Loading dashboard data...");
                        _ = vm.LoadDataAsync();
                    }
                    else if (page is PatternDetailPage detailPage)
                    {
                        var vm = App.ServiceProvider.GetRequiredService<PatternDetailViewModel>();
                        detailPage.DataContext = vm;
                        
                        if (parameter is string patternId)
                        {
                            System.Diagnostics.Debug.WriteLine($"Loading pattern detail for: {patternId}");
                            _ = vm.LoadPatternAsync(patternId);
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Page is null!");
                    Console.WriteLine("ERROR: Page is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in Navigate: {ex.Message}");
                Console.WriteLine($"ERROR in Navigate: {ex.Message}");
                MessageBox.Show($"Navigation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToDashboard()
        {
            System.Diagnostics.Debug.WriteLine("NavigateToDashboard called");
            Console.WriteLine("NavigateToDashboard called");
            Navigate(typeof(DashboardPage));
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
