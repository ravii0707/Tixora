using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Tixora.Service.Exceptions;
using Tixora.Service.Interfaces;

namespace Tixora.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> RegisterAsync(UserRegisterDTO userDto)
        {
            if (await _userRepository.EmailExistsAsync(userDto.Email))
            {
                throw new BadRequestException("Email already exists");
            }

            var user = _mapper.Map<TbUser>(userDto);
            user.RoleName = "user"; // Force role to be user

            var createdUser = await _userRepository.AddAsync(user);
            return _mapper.Map<UserResponseDTO>(createdUser);
        }

        public async Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || user.Password != loginDto.Password) // In real app, use hashed passwords
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }
    }
}
