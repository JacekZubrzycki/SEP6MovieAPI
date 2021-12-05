using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Models
{
    [Keyless]
    public class Ratings
    {
        public int Movie_id { get; set; }
        public double Rating { get; set; }
        public int Votes { get; set; }
    }
}


