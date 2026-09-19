using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Interfaces;
using DesignPatternMaster.UseCases.Exceptions;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UseCases.Queries;

public sealed class GetPatternDetailQuery : IGetPatternDetailQuery
{
    private readonly IPatternRepository _repository;
    private readonly ILogger<GetPatternDetailQuery> _logger;

    public GetPatternDetailQuery(IPatternRepository repository, ILogger<GetPatternDetailQuery> logger)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(logger);
        _repository = repository;
        _logger = logger;
    }

    public async Task<DesignPattern> ExecuteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id must not be empty.", nameof(id));

        var normalized = id.Trim();
        var pattern = await _repository.GetPatternByIdAsync(normalized, cancellationToken);
        if (pattern is null)
        {
            _logger.LogWarning("Design pattern not found: {PatternId}", normalized);
            throw new PatternNotFoundException(normalized);
        }

        return pattern;
    }
}
