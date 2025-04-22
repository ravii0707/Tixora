using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.DTOs
{
    public class BookingCreateDTO
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Showtime ID is required")]
        public int ShowtimeId { get; set; }

        [Required(ErrorMessage = "Movie ID is required")]
        public int MovieId { get; set; }

        [Range(1, 20, ErrorMessage = "Ticket count must be between 1 and 20")]
        public int TicketCount { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Total amount must be between 0.01 and 10,000")]
        public decimal TotalAmount { get; set; }
    }

    public class BookingResponseDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public int ShowtimeId { get; set; }
        public required string Showtime { get; set; }
        public int MovieId { get; set; }
        public required string MovieTitle { get; set; }
        public int TicketCount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
