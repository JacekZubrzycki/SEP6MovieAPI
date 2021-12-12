using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Entities
{
    public class MoviesDetailed
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }
        public int Votes { get; set; }
        public List<Person> Stars { get; set; }
        public List<Person> Directors { get; set; }
        public string Plot { get; set; }
        public string Poster { get; set; }
    }
}
