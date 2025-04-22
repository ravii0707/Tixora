using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tixora.Core.Entities;

[Table("tb_BookingHistory")]
[Index("UserId", "ShowtimeId", "MovieId", Name = "UQ_BookingHistory_UserShowMovie", IsUnique = true)]
public partial class TbBookingHistory
{
    [Key]
    [Column("BookingID")]
    public int BookingId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("ShowtimeID")]
    public int ShowtimeId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    public int TicketCount { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? BookingDate { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("TbBookingHistories")]
    public virtual TbMovie Movie { get; set; } = null!;

    [ForeignKey("ShowtimeId")]
    [InverseProperty("TbBookingHistories")]
    public virtual TbShowTime Showtime { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TbBookingHistories")]
    public virtual TbUser User { get; set; } = null!;
}
