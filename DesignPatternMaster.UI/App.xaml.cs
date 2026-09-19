using DesignPatternMaster.Core.Interfaces;
using DesignPatternMaster.Infrastructure.Repositories;
using DesignPatternMaster.UI.Services;
using DesignPatternMaster.UI.ViewModels;
using DesignPatternMaster.UI.Views.Pages;
using DesignPatternMaster.UseCases.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace DesignPatternMaster.UI;

public partial class App : Application
{
    private const int SplashScreenDurationMs = 1000;

    private IServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        RegisterExceptionHandlers();
        _ = RunStartupAsync();
    }

    private async Task RunStartupAsync()
    {
        try
        {
            // Prevent automatic shutdown when Splash Screen closes
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Show Splash Screen
            var splashScreen = new Views.SplashScreen();
            splashScreen.Show();

            var services = BuildServices();
#if DEBUG
            _services = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
#else
            _services = services.BuildServiceProvider();
#endif
            var logger = _services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("Service provider built.");

            await Task.Delay(SplashScreenDurationMs);
            splashScreen.Close();

            // Create and show main window (use the Views.MainWindow)
            var mainWindow = _services.GetRequiredService<Views.MainWindow>();

            // Set as main window and restore shutdown mode
            MainWindow = mainWindow;
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            mainWindow.Show();
            logger.LogInformation("MainWindow shown.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during startup: {ex.Message}\n\n{ex.StackTrace}",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    internal static ServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddDebug());

        // Core Services
        services.AddSingleton<IPatternRepository, JsonPatternRepository>();

        // Use Cases
        services.AddTransient<IGetPatternListQuery, GetPatternListQuery>();
        services.AddTransient<IGetPatternDetailQuery, GetPatternDetailQuery>();

        // UI Services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IUrlLauncher, UrlLauncher>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<PatternDetailViewModel>();
        services.AddSingleton<SettingsViewModel>();

        // Views
        // Register the MainWindow from the Views namespace (full UI layout)
        services.AddSingleton<Views.MainWindow>();
        services.AddTransient<DashboardPage>();
        services.AddTransient<PatternDetailPage>();
        services.AddTransient<SettingsPage>();

        return services;
    }

    private void RegisterExceptionHandlers()
    {
        // Registered before DI construction so that startup failures are also observed.
        // Continuing after Handled=true may resume in a corrupted state; recorded as policy.
        DispatcherUnhandledException += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled UI exception: {args.Exception.Message}");
            MessageBox.Show($"Unhandled exception: {args.Exception.Message}\n\n{args.Exception.StackTrace}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"Unhandled domain exception: {ex?.Message}");
            MessageBox.Show($"Fatal error: {ex?.Message}", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        };
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine($"Unobserved task exception: {args.Exception.Message}");
            args.SetObserved();
        };
    }

    protected override void OnExit(ExitEventArgs e)
    {
        (_services as IDisposable)?.Dispose();
        base.OnExit(e);
    }
}
