using Microsoft.EntityFrameworkCore;
using Tixora.Core.Context;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Tixora.Repository.Implementations
{
    public class ShowTimeRepository : IShowTimeRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ShowTimeRepository> _logger;

        public ShowTimeRepository(AppDbContext context, ILogger<ShowTimeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TbShowTime> AddAsync(TbShowTime showTime)
        {
            try
            {
                // Validate the time format
                if (!TimeOnly.TryParse(showTime.ShowTime, out _))
                {
                    _logger.LogWarning("Invalid time format provided: {ShowTime}", showTime.ShowTime);
                    throw new ArgumentException("Invalid time format. Use HH:mm");
                }

                await _context.TbShowTimes.AddAsync(showTime);
                await _context.SaveChangesAsync();
                return showTime;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error adding showtime");
                throw;
            }
        }

        public async Task<TbShowTime> UpdateAsync(TbShowTime showTime)
        {
            try
            {
                // Validate the time format
                if (!TimeOnly.TryParse(showTime.ShowTime, out _))
                {
                    _logger.LogWarning("Invalid time format during update: {ShowTime}", showTime.ShowTime);
                    throw new ArgumentException("Invalid time format. Use HH:mm");
                }

                _context.TbShowTimes.Update(showTime);
                await _context.SaveChangesAsync();
                return showTime;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating showtime with ID: {ShowtimeId}", showTime.ShowtimeId);
                throw;
            }
        }

        public async Task<TbShowTime?> GetByIdAsync(int id)
        {
            return await _context.TbShowTimes
                .Include(st => st.Movie)
                .FirstOrDefaultAsync(st => st.ShowtimeId == id);
        }

        public async Task<IEnumerable<TbShowTime>> GetAllAsync()
        {
            return await _context.TbShowTimes
                .Include(st => st.Movie)
                .ToListAsync();
        }

        public async Task<IEnumerable<TbShowTime>> GetByMovieIdAsync(int movieId)
        {
            return await _context.TbShowTimes
                .Include(st => st.Movie)
                .Where(st => st.MovieId == movieId && st.IsActive == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<TbShowTime>> GetByDateAsync(DateOnly date)
        {
            return await _context.TbShowTimes
               .Where(st => st.ShowDate == date && st.IsActive == true)
               .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var showTime = await _context.TbShowTimes.FindAsync(id);
            if (showTime == null)
            {
                return false;
            }

            //_context.TbShowTimes.Remove(showTime);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}