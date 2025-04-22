using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.DTOs
{
    public class MovieCreateDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public required string Title { get; set; }

        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
        public string? Genre { get; set; }

        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int? Duration { get; set; }

        [StringLength(50, ErrorMessage = "Language cannot exceed 50 characters")]
        public string? Language { get; set; }

        [StringLength(50, ErrorMessage = "Format cannot exceed 50 characters")]
        public string? Format { get; set; }

        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(1255, ErrorMessage = "Image URL cannot exceed 1255 characters")]
        public string? ImageUrl { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(1255, ErrorMessage = "Trailer URL cannot exceed 1255 characters")]
        public string? TrailerUrl { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(1255, ErrorMessage = "Poster URL cannot exceed 1255 characters")]
        public string? PosterUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class MovieResponseDTO
    {
        public int MovieId { get; set; }
        public required string Title { get; set; }
        public string? Genre { get; set; }
        public int? Duration { get; set; }
        public string? Language { get; set; }
        public string? Format { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public string? PosterUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
