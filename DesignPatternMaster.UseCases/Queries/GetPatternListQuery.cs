using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UseCases.Queries;

public sealed partial class GetPatternListQuery : IGetPatternListQuery
{
    private readonly IPatternRepository _repository;
    private readonly ILogger<GetPatternListQuery> _logger;

    public GetPatternListQuery(IPatternRepository repository, ILogger<GetPatternListQuery> logger)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(logger);
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DesignPattern>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var patterns = await _repository.GetAllPatternsAsync(cancellationToken);
        if (patterns.Count == 0)
            LogNoPatternsFound(_logger);

        return patterns;
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "No design patterns found.")]
    private static partial void LogNoPatternsFound(ILogger logger);
}
