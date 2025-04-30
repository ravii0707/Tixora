using System.Collections.Generic;
using System.Threading.Tasks;
using Tixora.Core.Entities;

namespace Tixora.Repository.Interfaces
{
    public interface IMovieRepository
    {
        Task<TbMovie> AddAsync(TbMovie movie);
        Task<TbMovie?> GetByIdAsync(int id);
        Task<IEnumerable<TbMovie>> GetAllAsync();
        Task<IEnumerable<TbMovie>> GetAllActiveAsync();
        Task<TbMovie> UpdateAsync(TbMovie movie);
        Task<bool> DeleteAsync(int id);
        Task ToggleActiveStatusAsync(int movieId, bool isActive);
        //Task<TbMovie> GetByTitleAsync(string title);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}