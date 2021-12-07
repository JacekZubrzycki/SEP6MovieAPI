using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Models
{
    public class RegisterModel
    {
        [Required]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "Username contains ':', '.' ';', '*', '/' or '\' which are not allowed!")]
        [StringLength(20, ErrorMessage = "Max characters for username are 20")]
        public string Username { get; set; }
        [Required]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "Password contains ':', '.' ';', '*', '/' or '\' which are not allowed!")]

        [StringLength(20, ErrorMessage = "Max characters for password are 20")]
        public string Password { get; set; }
    }
}
