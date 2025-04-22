using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.DTOs;

namespace Tixora.Service.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDTO> CreateAsync(BookingCreateDTO bookingDto);
        Task<BookingResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<BookingResponseDTO>> GetAllAsync();
        Task<IEnumerable<BookingResponseDTO>> GetByUserIdAsync(int userId);
    }
}
