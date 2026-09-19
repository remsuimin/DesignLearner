using DesignPatternMaster.Core.Entities;

namespace DesignPatternMaster.UseCases.Queries;

public interface IGetPatternDetailQuery
{
    Task<DesignPattern> ExecuteAsync(string id, CancellationToken cancellationToken = default);
}
