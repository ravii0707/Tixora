using System.Collections.Generic;
using System.Threading.Tasks;
using Tixora.Core.DTOs;

namespace Tixora.Service.Interfaces
{
    public interface IShowTimeService
    {
        // Basic ShowTime CRUD
        Task<ShowTimeResponseDTO> CreateAsync(ShowTimeCreateDTO showTimeDto);
        Task<ShowTimeResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<ShowTimeResponseDTO>> GetAllAsync();
        Task<IEnumerable<ShowTimeResponseDTO>> GetByMovieIdAsync(int movieId);
        Task<ShowTimeResponseDTO> UpdateAsync(int id, ShowTimeCreateDTO showTimeDto);

        // Batch operations
        Task<IEnumerable<ShowTimeResponseDTO>> CreateMultipleShowTimesAsync(int movieId, IEnumerable<ShowTimeCreateDTO> showTimeDtos);

        // Validation methods
        Task ValidateShowTimeDifference(int movieId, List<ShowTimeCreateDTO> shows);
        Task ValidateShowTimes(List<ShowTimeUpdateDTO> shows);
        Task ValidateShowTimeDifferences(List<ShowTimeUpdateDTO> shows);
    }
}