using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.Entities;

namespace Tixora.Repository.Interfaces
{
    public interface IMovieRepository
    {
        Task<TbMovie> AddAsync(TbMovie movie);
        Task<TbMovie> GetByIdAsync(int id);
        Task<IEnumerable<TbMovie>> GetAllAsync();
        Task<IEnumerable<TbMovie>> GetAllActiveAsync();
        Task<TbMovie> UpdateAsync(TbMovie movie);
        Task<bool> DeleteAsync(int id);
    }
}
