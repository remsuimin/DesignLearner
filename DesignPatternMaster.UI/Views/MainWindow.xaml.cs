using System.Windows;
using DesignPatternMaster.UI.Services;
using DesignPatternMaster.UI.ViewModels;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UI.Views;

public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigation;
    private readonly ILogger<MainWindow> _logger;
    private bool _initialized;

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigation,
        ILogger<MainWindow> logger)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(logger);
        _navigation = navigation;
        _logger = logger;

        try
        {
            DataContext = viewModel;
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                if (_initialized)
                    return;

                _initialized = true;
                _navigation.Initialize(RootFrame);
                await _navigation.NavigateToDashboardAsync();
            };

            StateChanged += MainWindow_StateChanged;
        }
        catch (Exception ex)
        {
            LogFailedToInitializeMainWindow(_logger, ex);
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

    [LoggerMessage(EventId = 1, Level = LogLevel.Critical, Message = "Failed to initialize MainWindow.")]
    private static partial void LogFailedToInitializeMainWindow(ILogger logger, Exception ex);
}
