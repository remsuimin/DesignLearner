using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternMaster.UI.Services;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UI.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private string _applicationTitle = "Design Pattern Master";

    public MainWindowViewModel(INavigationService navigation, ILogger<MainWindowViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(logger);
        _navigation = navigation;
        _logger = logger;
    }

    [RelayCommand]
    private async Task Navigate(string? pageName)
    {
        switch (pageName)
        {
            case "Dashboard":
                await _navigation.NavigateToDashboardAsync();
                break;
            case "Settings":
                await _navigation.NavigateToSettingsAsync();
                break;
            default:
                LogUnknownPageRequested(_logger, pageName);
                break;
        }
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Unknown page requested: {PageName}")]
    private static partial void LogUnknownPageRequested(ILogger logger, string? pageName);
}
