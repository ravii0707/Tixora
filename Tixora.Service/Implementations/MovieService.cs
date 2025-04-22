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
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public MovieService(IMovieRepository movieRepository, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        public async Task<MovieResponseDTO> CreateAsync(MovieCreateDTO movieDto)
        {
            var movie = _mapper.Map<TbMovie>(movieDto);
            var createdMovie = await _movieRepository.AddAsync(movie);
            return _mapper.Map<MovieResponseDTO>(createdMovie);
        }

        public async Task<MovieResponseDTO> GetByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                throw new NotFoundException("Movie not found");
            }

            return _mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllAsync()
        {
            var movies = await _movieRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MovieResponseDTO>>(movies);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllActiveAsync()
        {
            var movies = await _movieRepository.GetAllActiveAsync();
            return _mapper.Map<IEnumerable<MovieResponseDTO>>(movies);
        }

        public async Task<MovieResponseDTO> UpdateAsync(int id, MovieCreateDTO movieDto)
        {
            var existingMovie = await _movieRepository.GetByIdAsync(id);
            if (existingMovie == null)
            {
                throw new NotFoundException("Movie not found");
            }

            _mapper.Map(movieDto, existingMovie);
            var updatedMovie = await _movieRepository.UpdateAsync(existingMovie);
            return _mapper.Map<MovieResponseDTO>(updatedMovie);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _movieRepository.DeleteAsync(id);
        }
    }
}
