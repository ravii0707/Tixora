using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.Entities;

namespace Tixora.Repository.Interfaces
{
    public interface IShowTimeRepository
    {
        Task<TbShowTime> AddAsync(TbShowTime showTime);
        Task<TbShowTime> GetByIdAsync(int id);
        Task<IEnumerable<TbShowTime>> GetAllAsync();
        Task<IEnumerable<TbShowTime>> GetByMovieIdAsync(int movieId);
        Task<TbShowTime> UpdateAsync(TbShowTime showTime);
    }
}
