using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.Constants;

namespace Tixora.Core.Validation
{
    public class ValidGenreAttribute :ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var genre = value.ToString();
            if(!ValidGenres.IsValidGenre(genre))
            {
                var validGenres = string.Join(",",ValidGenres.Genres.OrderBy(g => g));
                return new ValidationResult($"Invalid Genre please Add valid genre as like :{validGenres}");
            }
            return ValidationResult.Success;
        }
    }
}
