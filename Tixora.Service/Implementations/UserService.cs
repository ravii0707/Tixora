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

        // Constructor: Initializes dependencies
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> RegisterAsync(UserRegisterDTO userDto)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(userDto.Email))
            {
                throw new BadRequestException("Email already exists");
            }

            // Check if phone number already exists
            if (await _userRepository.PhoneExistsAsync(userDto.Phone))
            {
                throw new BadRequestException("Phone number already in use");
            }

            // Map DTO to User entity
            var user = _mapper.Map<TbUser>(userDto);

            // CHANGED: Hash the password before storing
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // Set default role
            user.RoleName = "user";

            // Add user to database
            var createdUser = await _userRepository.AddAsync(user);

            // Map and return the response DTO
            return _mapper.Map<UserResponseDTO>(createdUser);
        }

        public async Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDto)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            // CHANGED: Added null check first
            if (user == null)
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            // CHANGED: Proper password verification using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            // Map and return user data
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> GetByIdAsync(int id)
        {
            // Get user by ID
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            // Get all users
            var users = await _userRepository.GetAllAsync();

            // Map to response DTOs
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }
    }
}