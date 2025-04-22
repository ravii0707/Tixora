using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
