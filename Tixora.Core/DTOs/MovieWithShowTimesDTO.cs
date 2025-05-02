using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.DTOs
{
    public class MovieWithShowTimesDTO
    {
        public MovieCreateDTO Movie { get; set; }
        public List<ShowTimeCreateDTO> Shows { get; set; }
    }

    // For updating movie with showtimes
    public class MovieWithShowTimesUpdateDTO
    {
        [Required]
        public MovieCreateDTO Movie { get; set; }

        public List<ShowTimeUpdateDTO> Shows { get; set; }
    }

    public class MovieWithShowTimesResponseDTO
    {
        public MovieResponseDTO Movie { get; set; }

        //[MaxLength(4, ErrorMessage = "Cannot have more than 4 showtimes per day")]
        public List<ShowTimeResponseDTO> Shows { get; set; }
    }
    // Showtime DTO for updates (extends create DTO with ID)
    public class ShowTimeUpdateDTO : ShowTimeCreateDTO
    {
        [Required]
        public int ShowtimeId { get; set; }
    }
}
