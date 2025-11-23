using DesignPatternMaster.Core.Interfaces;
using DesignPatternMaster.Infrastructure.Repositories;
using DesignPatternMaster.UseCases.Queries;
using DesignPatternMaster.UI.ViewModels;
using DesignPatternMaster.UI.Views;
using DesignPatternMaster.UI.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.IO;

namespace DesignPatternMaster.UI
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        public App()
        {
            Log("App constructor called");
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                Log("OnStartup called");

                // Prevent automatic shutdown when Splash Screen closes
                ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // Show Splash Screen
                var splashScreen = new DesignPatternMaster.UI.Views.SplashScreen();
                splashScreen.Show();

                // Wait for 1 second
                await Task.Delay(1000);

                splashScreen.Close();
                
                // Build DI container
                var services = new ServiceCollection();
                
                // Core Services
                services.AddSingleton<IPatternRepository, JsonPatternRepository>();
                
                // Use Cases
                services.AddSingleton<GetPatternListQuery>();
                services.AddSingleton<GetPatternDetailQuery>();
                
                // ViewModels
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<PatternDetailViewModel>();
                services.AddSingleton<SettingsViewModel>();
                
                // Views
                // Register the MainWindow from the Views namespace (full UI layout)
                services.AddSingleton<Views.MainWindow>();
                services.AddTransient<DashboardPage>();
                services.AddTransient<PatternDetailPage>();
                services.AddTransient<SettingsPage>();
                
                _serviceProvider = services.BuildServiceProvider();
                Log("Service provider built");
                
                // Create and show main window (use the Views.MainWindow)
                var mainWindow = _serviceProvider.GetRequiredService<Views.MainWindow>();
                Log("MainWindow retrieved from DI");
                
                // ensure any UI thread unhandled exceptions are surfaced
                DispatcherUnhandledException += (sender, args) =>
                {
                    System.Diagnostics.Debug.WriteLine($"Unhandled UI exception: {args.Exception.Message}");
                    MessageBox.Show($"Unhandled exception: {args.Exception.Message}\n\n{args.Exception.StackTrace}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    args.Handled = true;
                };

                // Set as main window and restore shutdown mode
                Application.Current.MainWindow = mainWindow;
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                
                mainWindow.Show();
                Log("MainWindow.Show() called");
                
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Log($"ERROR in OnStartup: {ex.Message}");
                MessageBox.Show($"Error during startup: {ex.Message}\n\n{ex.StackTrace}", 
                    "Startup Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
                Console.WriteLine("OnExit called");
                Log("OnExit called");
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        public static IServiceProvider ServiceProvider => ((App)Current)._serviceProvider 
            ?? throw new InvalidOperationException("Service provider not initialized");

        private static void Log(string message)
        {
            try
            {
                var logPath = Path.Combine(Path.GetTempPath(), "DesignPatternMaster.log");
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}\r\n");
            }
            catch
            {
                // ignore logging errors
            }
        }
    }
}

