using System.Collections.Generic;
using System.Threading.Tasks;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Interfaces;

namespace DesignPatternMaster.UseCases.Queries
{
    public class GetPatternListQuery
    {
        private readonly IPatternRepository _repository;

        public GetPatternListQuery(IPatternRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DesignPattern>> ExecuteAsync()
        {
            return await _repository.GetAllPatternsAsync();
        }
    }
}
