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
    public class ShowTimeService : IShowTimeService
    {
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public ShowTimeService(
            IShowTimeRepository showTimeRepository,
            IMovieRepository movieRepository,
            IMapper mapper)
        {
            _showTimeRepository = showTimeRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        //public async Task<ShowTimeResponseDTO> CreateAsync(ShowTimeCreateDTO showTimeDto)
        //{
        //    //var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId);
        //    //if (movie == null)
        //    //{
        //    //    throw new NotFoundException("Movie not found");
        //    //}

        //    var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId);
        //    if (movie == null)
        //    {
        //        throw new NotFoundException("Movie not found");
        //    }

        //    var showTime = _mapper.Map<TbShowTime>(showTimeDto);
        //    var createdShowTime = await _showTimeRepository.AddAsync(showTime);

        //    return _mapper.Map<ShowTimeResponseDTO>(createdShowTime);
        //}

        public async Task<ShowTimeResponseDTO> CreateAsync(ShowTimeCreateDTO showTimeDto)
        {
            // Validate movie exists
            var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId);
            if (movie == null)
            {
                throw new NotFoundException("Movie not found");
            }

            // Mapping will handle the string format
            var showTime = _mapper.Map<TbShowTime>(showTimeDto);
            var createdShowTime = await _showTimeRepository.AddAsync(showTime);

            return _mapper.Map<ShowTimeResponseDTO>(createdShowTime);
        }

        public async Task<ShowTimeResponseDTO> GetByIdAsync(int id)
        {
            var showTime = await _showTimeRepository.GetByIdAsync(id);
            if (showTime == null)
            {
                throw new NotFoundException("Showtime not found");
            }

            return _mapper.Map<ShowTimeResponseDTO>(showTime);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetAllAsync()
        {
            var showTimes = await _showTimeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetByMovieIdAsync(int movieId)
        {
            var showTimes = await _showTimeRepository.GetByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<ShowTimeResponseDTO> UpdateAsync(int id, ShowTimeCreateDTO showTimeDto)
        {
            var existingShowTime = await _showTimeRepository.GetByIdAsync(id);
            if (existingShowTime == null)
            {
                throw new NotFoundException("Showtime not found");
            }

            _mapper.Map(showTimeDto, existingShowTime);
            var updatedShowTime = await _showTimeRepository.UpdateAsync(existingShowTime);
            return _mapper.Map<ShowTimeResponseDTO>(updatedShowTime);
        }
    }
}