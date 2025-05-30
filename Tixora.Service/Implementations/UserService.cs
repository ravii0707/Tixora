﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;
using Tixora.Core.Helpers;
using Tixora.Repository.Interfaces;
using Tixora.Service.Exceptions;
using Tixora.Service.Interfaces;

namespace Tixora.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public UserService(IUserRepository userRepository, IMapper mapper,IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserResponseDTO> RegisterAsync(UserRegisterDTO userDto)
        {
            if (await _userRepository.EmailExistsAsync(userDto.Email))
            {
                throw new BadRequestException("Email already exists");
            }
            if (await _userRepository.PhoneExistsAsync(userDto.Phone))
            {
                throw new BadRequestException("Phone number already in use");
            }

            var user = _mapper.Map<TbUser>(userDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            

            var createdUser = await _userRepository.AddAsync(user);
            //return _mapper.Map<UserResponseDTO>(createdUser);
            var response = _mapper.Map<UserResponseDTO>(createdUser);
            response.Token = JwtHelper.GenerateToken(user.Email, user.RoleName, user.UserId, _configuration);
        
            return response;
        }
        public async Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDto)
        {
            var normalizedEmail = loginDto.Email.Trim().ToLower();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user == null)
                throw new UnauthorizedException("Invalid credentials");

            if (user.RoleName == "admin" && loginDto.Password == user.Password)
            {
                //// Admin with plain text password
                //return _mapper.Map<UserResponseDTO>(user);

                var response = _mapper.Map<UserResponseDTO>(user);
                response.Token = JwtHelper.GenerateToken(user.Email,user.RoleName, user.UserId, _configuration);
                return response;

            }
            else if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                // Regular user with hashed password
                throw new UnauthorizedException("Invalid credentials");
            }


            //return _mapper.Map<UserResponseDTO>(user);
            var userresponse = _mapper.Map<UserResponseDTO>(user);
            userresponse.Token = JwtHelper.GenerateToken(user.Email, user.RoleName, user.UserId, _configuration);

            return userresponse;
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