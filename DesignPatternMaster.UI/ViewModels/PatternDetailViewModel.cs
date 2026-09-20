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
            LogPatternLoaded(_logger, id);
        }
        catch (Exception ex)
        {
            LogFailedToLoadPattern(_logger, ex, id);
            throw;
        }
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Pattern loaded: {PatternId}")]
    private static partial void LogPatternLoaded(ILogger logger, string patternId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Failed to load pattern: {PatternId}")]
    private static partial void LogFailedToLoadPattern(ILogger logger, Exception ex, string patternId);
}
