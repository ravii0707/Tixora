using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.DTOs;

namespace Tixora.Service.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponseDTO> CreateAsync(MovieCreateDTO movieDto);
        Task<MovieResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<MovieResponseDTO>> GetAllAsync();
        Task<IEnumerable<MovieResponseDTO>> GetAllActiveAsync();
        Task<MovieResponseDTO> UpdateAsync(int id, MovieCreateDTO movieDto);
        Task<bool> DeleteAsync(int id);
    }
}
