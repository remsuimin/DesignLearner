using DesignPatternMaster.Core.Entities;

namespace DesignPatternMaster.UseCases.Queries;

public interface IGetPatternListQuery
{
    Task<IReadOnlyList<DesignPattern>> ExecuteAsync(CancellationToken cancellationToken = default);
}
