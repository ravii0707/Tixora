﻿using System.ComponentModel.DataAnnotations;

public class BookingCreateDTO
{
    [Required(ErrorMessage = "User ID is required to identify who is making the booking.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Please select a showtime for your booking.")]
    public int ShowtimeId { get; set; }

    [Required(ErrorMessage = "Please select a movie for your booking.")]
    public int MovieId { get; set; }

    [Range(1, 10, ErrorMessage = "You can book between 1 to 10 tickets at a time.")]
    public int TicketCount { get; set; }

}

public class BookingResponseDTO
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public required string UserName { get; set; }
    public int ShowtimeId { get; set; }
    public required string ShowTime { get; set; }
    public DateOnly ShowDate { get; set; }
    public int MovieId { get; set; }
    public required string MovieTitle { get; set; }
    public int TicketCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = "Confirmed";
}
