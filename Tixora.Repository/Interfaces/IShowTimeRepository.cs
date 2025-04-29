using System.Collections.Generic;
using System.Threading.Tasks;
using Tixora.Core.Entities;

namespace Tixora.Repository.Interfaces
{
    public interface IShowTimeRepository
    {
        Task<TbShowTime> AddAsync(TbShowTime showTime);
        Task<TbShowTime?> GetByIdAsync(int id);
        Task<IEnumerable<TbShowTime>> GetAllAsync();
        Task<IEnumerable<TbShowTime>> GetByMovieIdAsync(int movieId);
        
        Task<TbShowTime> UpdateAsync(TbShowTime showTime);
        Task<IEnumerable<TbShowTime>> GetByDateAsync(DateOnly date);
    }
}