using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Models
{
    [Keyless]
    public class Star
    {
        public int Movie_id { get; set; }
        public int Person_id { get; set; }
    }
}


