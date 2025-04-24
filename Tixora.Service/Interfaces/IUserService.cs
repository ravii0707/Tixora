using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.DTOs;

namespace Tixora.Service.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterAsync(UserRegisterDTO userDto);
        Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDto);
        Task<UserResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
    }
}