using System.Collections.Generic;
using System.Threading.Tasks;
using Tixora.Core.DTOs;

namespace Tixora.Service.Interfaces
{
    public interface IShowTimeService
    {
        Task<ShowTimeResponseDTO> CreateAsync(ShowTimeCreateDTO showTimeDto);
        Task<ShowTimeResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<ShowTimeResponseDTO>> GetAllAsync();
        Task<IEnumerable<ShowTimeResponseDTO>> GetByMovieIdAsync(int movieId);
        Task<ShowTimeResponseDTO> UpdateAsync(int id, ShowTimeCreateDTO showTimeDto);
        //Task<IEnumerable<string>> GetAvailableTimeSlots(DateOnly date);
        Task<IEnumerable<ShowTimeResponseDTO>> CreateMultipleShowTimesAsync(int movieId, IEnumerable<ShowTimeCreateDTO> showTimeDtos);

    }
}