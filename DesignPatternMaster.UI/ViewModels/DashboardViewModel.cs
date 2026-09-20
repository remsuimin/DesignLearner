using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.UI.Services;
using DesignPatternMaster.UseCases.Queries;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UI.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly IGetPatternListQuery _query;
    private readonly INavigationService _navigation;
    private readonly ILogger<DashboardViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<DesignPattern> _patterns = new();

    public DashboardViewModel(
        IGetPatternListQuery query,
        INavigationService navigation,
        ILogger<DashboardViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(logger);
        _query = query;
        _navigation = navigation;
        _logger = logger;
    }

    public async Task LoadDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var patterns = await _query.ExecuteAsync(cancellationToken);
            Patterns.Clear();
            foreach (var pattern in patterns)
            {
                Patterns.Add(pattern);
            }

            LogDashboardPatternsLoaded(_logger, Patterns.Count);
        }
        catch (Exception ex)
        {
            LogFailedToLoadDashboardPatterns(_logger, ex);
            throw;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetail(DesignPattern? pattern)
    {
        if (pattern is null)
            return;

        await _navigation.NavigateToPatternDetailAsync(pattern.Id);
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Dashboard patterns loaded: {Count}")]
    private static partial void LogDashboardPatternsLoaded(ILogger logger, int count);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Failed to load dashboard patterns.")]
    private static partial void LogFailedToLoadDashboardPatterns(ILogger logger, Exception ex);
}
