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
    public class ShowTimeRepository : IShowTimeRepository
    {
        private readonly AppDbContext _context;

        public ShowTimeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TbShowTime> AddAsync(TbShowTime showTime)
        {
            await _context.TbShowTimes.AddAsync(showTime);
            await _context.SaveChangesAsync();
            return showTime;
        }

        public async Task<TbShowTime> GetByIdAsync(int id)
        {
            return await _context.TbShowTimes.FindAsync(id);
        }

        public async Task<IEnumerable<TbShowTime>> GetAllAsync()
        {
            return await _context.TbShowTimes.Include(st => st.Movie).ToListAsync();
        }

        public async Task<IEnumerable<TbShowTime>> GetByMovieIdAsync(int movieId)
        {
            return await _context.TbShowTimes
                .Include(st => st.Movie)
                .Where(st => st.MovieId == movieId && st.IsActive == true)
                .ToListAsync();
        }

        public async Task<TbShowTime> UpdateAsync(TbShowTime showTime)
        {
            _context.TbShowTimes.Update(showTime);
            await _context.SaveChangesAsync();
            return showTime;
        }
    }
}
