using System.Threading.Tasks;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Interfaces;

namespace DesignPatternMaster.UseCases.Queries
{
    public class GetPatternDetailQuery
    {
        private readonly IPatternRepository _repository;

        public GetPatternDetailQuery(IPatternRepository repository)
        {
            _repository = repository;
        }

        public async Task<DesignPattern?> ExecuteAsync(string id)
        {
            return await _repository.GetPatternByIdAsync(id);
        }
    }
}
