using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.DTOs
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.(com|org|net|edu|gov|co\.uk|in|ca|us|us\.org)$",
       ErrorMessage = "Email must be in a valid format and contain a known domain.")]
        public required string Email { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain only digits.")]
        public required string Phone { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        public required string Password { get; set; }

    // RoleName is not included here as it should default to "user"
    }
public class UserLoginDTO
{
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }

    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public required string RoleName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        // Password is not included in response
    }
}
