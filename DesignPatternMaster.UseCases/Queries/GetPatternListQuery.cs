using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UseCases.Queries;

public sealed class GetPatternListQuery : IGetPatternListQuery
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
            _logger.LogInformation("No design patterns found.");

        return patterns;
    }
}
