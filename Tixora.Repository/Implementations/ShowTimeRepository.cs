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
            // Validate the string format before saving
            if (!TimeOnly.TryParse(showTime.ShowTime, out _))
            {
                throw new ArgumentException("Invalid time format. Use HH:mm");
            }

            await _context.TbShowTimes.AddAsync(showTime);
            await _context.SaveChangesAsync();
            return showTime;
        }
        public async Task<TbShowTime> UpdateAsync(TbShowTime showTime)
        {
            // Same validation for updates
            //if (string.IsNullOrWhiteSpace(showTime.ShowTime) ||
            //    !IsValidShowTimeString(showTime.ShowTime))
            //{
            //    throw new ArgumentException("Invalid ShowTime format");
            //}

            _context.TbShowTimes.Update(showTime);
            await _context.SaveChangesAsync();
            return showTime;
        }

        //private bool IsValidShowTimeString(string showTime)
        //{
        //    if (string.IsNullOrWhiteSpace(showTime))
        //        return false;

        //    var times = showTime.Split('|');
        //    if (times.Length != 4)
        //        return false;

        //    foreach (var timeStr in times)
        //    {
        //        if (!TimeOnly.TryParse(timeStr, out _))
        //            return false;
        //    }

        //    return true;
        //}

        //private bool IsValidShowTimeFormat(string showTime)
        //{
        //    if (string.IsNullOrWhiteSpace(showTime))
        //        return false;

        //    var times = showTime.Split('|');
        //    if (times.Length != 4)
        //        return false;

        //    foreach (var timeStr in times)
        //    {
        //        if (!TimeOnly.TryParse(timeStr, out _))
        //            return false;
        //    }

        //    return true;
        //}


        public async Task<TbShowTime> GetByIdAsync(int id)
        {
            return await _context.TbShowTimes
                .Include(st => st.Movie)
                .FirstOrDefaultAsync(st => st.ShowtimeId == id);
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


    }
}