using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.Entities;

namespace Tixora.Repository.Interfaces
{
    public interface IBookingRepository
    {
        Task<TbBookingHistory> AddAsync(TbBookingHistory booking);
        Task<TbBookingHistory?> GetByIdAsync(int id);
        Task<IEnumerable<TbBookingHistory>> GetAllAsync();
        Task<IEnumerable<TbBookingHistory>> GetByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int userId, int showtimeId, int movieId);
    }
}