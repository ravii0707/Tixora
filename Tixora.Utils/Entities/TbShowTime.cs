using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tixora.Core.Entities;

[Table("tb_ShowTime")]
public partial class TbShowTime
{
    [Key]
    [Column("ShowtimeID")]
    public int ShowtimeId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly ShowTime { get; set; }

    public bool? IsActive { get; set; }

    public int AvailableSeats { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("TbShowTimes")]
    public virtual TbMovie Movie { get; set; } = null!;

    [InverseProperty("Showtime")]
    public virtual ICollection<TbBookingHistory> TbBookingHistories { get; set; } = new List<TbBookingHistory>();
}
