using System.Collections.Generic;
using System.Threading.Tasks;
using DesignPatternMaster.Core.Entities;

namespace DesignPatternMaster.Core.Interfaces
{
    public interface IPatternRepository
    {
        Task<IEnumerable<DesignPattern>> GetAllPatternsAsync();
    Task<DesignPattern?> GetPatternByIdAsync(string id);
    }
}
