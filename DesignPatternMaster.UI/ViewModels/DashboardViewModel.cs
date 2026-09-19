using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.UI.Services;
using DesignPatternMaster.UseCases.Queries;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

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

            _logger.LogInformation("Dashboard patterns loaded: {Count}", Patterns.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard patterns.");
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
}
