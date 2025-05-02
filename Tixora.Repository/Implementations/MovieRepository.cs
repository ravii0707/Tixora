using Microsoft.EntityFrameworkCore;
using Tixora.Core.Context;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Tixora.Repository.Implementations
{
    public class MovieRepository : IMovieRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MovieRepository> _logger;

        public MovieRepository(AppDbContext context, ILogger<MovieRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TbMovie> AddAsync(TbMovie movie)
        {
            await _context.TbMovies.AddAsync(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<TbMovie?> GetByIdAsync(int id)
        {
            return await _context.TbMovies.FindAsync(id);
        }

        public async Task<IEnumerable<TbMovie>> GetAllAsync()
        {
            return await _context.TbMovies.ToListAsync();
        }

        public async Task<IEnumerable<TbMovie>> GetAllActiveAsync()
        {
            return await _context.TbMovies
                .Where(m => m.IsActive == true)
                .ToListAsync();
        }

        public async Task<TbMovie> UpdateAsync(TbMovie movie)
        {
            _context.TbMovies.Update(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _context.TbMovies.FindAsync(id);
            if (movie == null) return false;

            _context.TbMovies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ToggleActiveStatusAsync(int movieId, bool isActive)
        {
            var movie = await GetByIdAsync(movieId);
            if (movie != null)
            {
                movie.IsActive = isActive;
                await _context.SaveChangesAsync();
            }
        }
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
       
    }
}