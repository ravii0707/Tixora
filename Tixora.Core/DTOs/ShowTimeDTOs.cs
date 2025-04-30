using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.DTOs
{
    public class ShowTimeCreateDTO
    {

        [Required(ErrorMessage = "Movie ID is required")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Show date is required")]
        public DateOnly ShowDate { get; set; }

        [Required(ErrorMessage = "Show time is required")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$",
        ErrorMessage = "Time must be in HH:mm 24-hour format (e.g., '13:30')")]
        public string ShowTime { get; set; } = null!;

        [Range(1, 250, ErrorMessage = "Available seats must be between 1 and 250")]
        public int AvailableSeats { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ShowTimeResponseDTO
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }

        // Movie Details
        public required string MovieTitle { get; set; }
        public string? Genre { get; set; }
        public string? Language { get; set; }
        public string? Format { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsMovieActive { get; set; }

        // ShowTime Details
        public DateOnly ShowDate { get; set; }
        public string ShowTime { get; set; } = null!;
        public bool IsActive { get; set; }
        public int AvailableSeats { get; set; }


    }
}