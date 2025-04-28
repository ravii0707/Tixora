using Microsoft.EntityFrameworkCore;
using Tixora.Core.Context;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tixora.Repository.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(AppDbContext context, ILogger<BookingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<TbBookingHistory> AddAsync(TbBookingHistory booking)
        {
            try
            {
                await _context.TbBookingHistories.AddAsync(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating booking");
                throw new Exception("Failed to create booking. Please try again.", ex);
            }
        }

        public async Task<TbBookingHistory?> GetByIdAsync(int id)
        {
            return await _context.TbBookingHistories
                .Include(b => b.User)
                .Include(b => b.Movie)
                .Include(b => b.Showtime)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<IEnumerable<TbBookingHistory>> GetAllAsync()
        {
            return await _context.TbBookingHistories
                .Include(b => b.User)
                .Include(b => b.Movie)
                .Include(b => b.Showtime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TbBookingHistory>> GetByUserIdAsync(int userId)
        {
            return await _context.TbBookingHistories
                .Include(b => b.Movie)
                .Include(b => b.Showtime)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int userId, int showtimeId, int movieId)
        {
            return await _context.TbBookingHistories
                .AnyAsync(b => b.UserId == userId && b.ShowtimeId == showtimeId && b.MovieId == movieId);
        }
    }
}