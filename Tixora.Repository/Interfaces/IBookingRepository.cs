using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Tixora.Core.Entities;

namespace Tixora.Repository.Interfaces
{
    public interface IBookingRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<TbBookingHistory> AddAsync(TbBookingHistory booking);
        Task<TbBookingHistory?> GetByIdAsync(int id);
        Task<IEnumerable<TbBookingHistory>> GetAllAsync();
        Task<IEnumerable<TbBookingHistory>> GetByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int userId, int showtimeId, int movieId);
    }
}