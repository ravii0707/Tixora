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
        public TimeOnly ShowTime { get; set; }

        [Range(1, 500, ErrorMessage = "Available seats must be between 1 and 500")]
        public int AvailableSeats { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ShowTimeResponseDTO
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public required string MovieTitle { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly ShowTime { get; set; }
        public bool IsActive { get; set; }
        public int AvailableSeats { get; set; }
    }
}
