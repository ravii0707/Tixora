using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.Context;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;

namespace Tixora.Repository.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TbBookingHistory> AddAsync(TbBookingHistory booking)
        {
            await _context.TbBookingHistories.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
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