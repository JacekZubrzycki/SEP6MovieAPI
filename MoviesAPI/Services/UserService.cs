using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Helpers;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DBContext _context;

        public UserService(DBContext context)
        {
            _context = context;
        }

        public List<Director> GetAllDirectors()
        {
            return _context.directors.ToList();
        }

        public List<Movie> GetAllMovies()
        {
            return _context.movies.ToList();
        }

        public List<Person> GetAllPeople()
        {
            return _context.people.ToList();
        }

        public List<Ratings> GetAllRatings()
        {
            return _context.ratings.ToList();
        }

        public List<Star> getAllStars()
        {
            return _context.stars.ToList();
        }

        public Movie GetAMovieAccordingToID(int id)
        {
            return _context.movies.Find(id);
        }

        public Director GetDirectorByMovieID(int movie_id)
        {
            return _context.directors.Find(movie_id);
        }

        public Director GetDirectorByPersonID(int person_id)
        {
            return _context.directors.Find(person_id);
        }

        public Person GetPersonByID(int id)
        {
            return _context.people.Find(id);
        }

        public Person GetPersonByName(string name)
        {
            return _context.people.Find(name);
        }

        public Ratings GetRatingByMovieID(int id)
        {
            return _context.ratings.Find(id);
        }

        public Star GetStarByMovieID(int movie_id)
        {
            return _context.stars.Find(movie_id);
        }

        public Star GetStarByPersonID(int person_id)
        {
            return _context.stars.Find(person_id);
        }
    }
}
