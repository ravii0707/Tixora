using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tixora.Core.Entities;

[Table("tb_Movies")]
public partial class TbMovie
{
    [Key]
    [Column("MovieID")]
    public int MovieId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Title { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Genre { get; set; }

    public int? Duration { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Language { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Format { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Column("ImageURL")]
    [StringLength(1255)]
    public string? ImageUrl { get; set; }

    [Column("TrailerURL")]
    [StringLength(1255)]
    public string? TrailerUrl { get; set; }

    [Column("PosterURL")]
    [StringLength(1255)]
    public string? PosterUrl { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<TbBookingHistory> TbBookingHistories { get; set; } = new List<TbBookingHistory>();

    [InverseProperty("Movie")]
    public virtual ICollection<TbShowTime> TbShowTimes { get; set; } = new List<TbShowTime>();
}
