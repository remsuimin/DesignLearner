using CommunityToolkit.Mvvm.ComponentModel;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.UseCases.Queries;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UI.ViewModels;

public sealed partial class PatternDetailViewModel : ObservableObject
{
    private readonly IGetPatternDetailQuery _query;
    private readonly ILogger<PatternDetailViewModel> _logger;

    [ObservableProperty]
    private DesignPattern? _selectedPattern;

    public PatternDetailViewModel(IGetPatternDetailQuery query, ILogger<PatternDetailViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(logger);
        _query = query;
        _logger = logger;
    }

    public async Task LoadPatternAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            SelectedPattern = await _query.ExecuteAsync(id, cancellationToken);
            _logger.LogInformation("Pattern loaded: {PatternId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load pattern: {PatternId}", id);
            throw;
        }
    }
}
