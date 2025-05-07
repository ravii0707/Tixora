using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.Constants
{
    public static class ValidGenres
    {
        public static readonly HashSet<string> Genres = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Action",
            "Adventure",
            "Animation",
            "Comedy",
            "Crime",
            "Documentary",
            "Drama",
            "Fantasy",
            "Horror",
            "Mystery",
            "Romance",
            "Sci-Fi",
            "Thriller",
            "Western",
            "Family",
            "Musical",
            "Biography",
            "History",
            "War",
            "Sport"
        };
        public static bool IsValidGenre(string genre)
        {
            if(string.IsNullOrWhiteSpace(genre)) return false;
            return Genres.Contains(genre.Trim());
        }
    }
}
