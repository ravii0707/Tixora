using System.Collections.Generic;
using System.Threading.Tasks;
using Tixora.Core.DTOs;

namespace Tixora.Service.Interfaces
{
    public interface IMovieService
    {
        // Basic Movie CRUD
        Task<MovieResponseDTO> CreateAsync(MovieCreateDTO movieDto);
        Task<MovieResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<MovieResponseDTO>> GetAllAsync();
        Task<IEnumerable<MovieResponseDTO>> GetAllActiveAsync();
        Task<MovieResponseDTO> UpdateAsync(int id, MovieCreateDTO movieDto);
        Task<bool> DeleteAsync(int id);
        Task ToggleMovieStatusAsync(int id, bool isActive);

        // Movie with Showtimes operations
        Task<MovieWithShowTimesResponseDTO> CreateMovieWithShowTimesAsync(MovieWithShowTimesDTO movieWithShows);
        Task<MovieWithShowTimesResponseDTO> GetMovieWithShowTimesAsync(int movieId);
        Task<MovieWithShowTimesResponseDTO> UpdateMovieWithShowTimesAsync(int movieId, MovieWithShowTimesUpdateDTO updateDto);
    }
}