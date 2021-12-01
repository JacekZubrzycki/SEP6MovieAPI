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

        public List<Movie> GetAllMovies()
        {
            return _context.movies.ToList();
        }

        public Movie GetAMovieAccordingToID(int id)
        {
            return _context.movies.Find(id);
        }
    }
}
