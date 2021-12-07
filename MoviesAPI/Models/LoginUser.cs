using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Models
{
    public class LoginUser
    {
        [Required]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "Username contains ':', '.' ';', '*', '/' and '\' which are not allowed!")]
        public String Username { get; set; }

        [Required]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "Password contains ':', '.' ';', '*', '/' and '\' which are not allowed!")]
        public String Password { get; set; }
    }
}
