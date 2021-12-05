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
            Director director = _context.directors.Where(s => s.Movie_id == movie_id).FirstOrDefault<Director>();
            return director;
        }

        public Director GetDirectorByPersonID(int person_id)
        {
            Director director = _context.directors.Where(s => s.Person_id == person_id).FirstOrDefault<Director>();
            return director;
        }

        public Person GetPersonByID(int id)
        {
            return _context.people.Find(id);
        }

        public Person GetPersonByName(string name)
        {
            Person person = _context.people.Where(s => s.Name == name).FirstOrDefault<Person>();
            return person;
        }

        public Ratings GetRatingByMovieID(int id)
        {
            Ratings rating = _context.ratings.Where(s => s.Movie_id == id).FirstOrDefault<Ratings>();
            return rating;
        }

        public Star GetStarByMovieID(int movie_id)
        {
            Star star = _context.stars.Where(s => s.Movie_id == movie_id).FirstOrDefault<Star>();
            return star;
        }

        public Star GetStarByPersonID(int person_id)
        {
            Star star = _context.stars.Where(s => s.Person_id == person_id).FirstOrDefault<Star>();
            return star;
        }
    }
}
