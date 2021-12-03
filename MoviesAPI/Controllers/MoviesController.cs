using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Helpers;
using MoviesAPI.Models;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("ctrn")]
    public class MoviesController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IUserService _userService;
        
        public MoviesController(IUserService userService, DBContext context)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet("GetAllMovies")]
        public List<Movie> GetAllMovies()
        {
            return _userService.GetAllMovies();
        }

        [HttpGet("GetAMovieAccordingToID/{id}")]
        public Movie GetAMovieAccordingToID(int id)
        {
            return _userService.GetAMovieAccordingToID(id);
        }

        [HttpGet("GetAllStars")]
        public List<Star> getAllStars()
        {
            return _userService.getAllStars();
        }

        [HttpGet("GetStarByMovieID/{movie_id}")]
        public Star GetStarByMovieID(int movie_id)
        {
            return _userService.GetStarByMovieID(movie_id);
        }

        [HttpGet("GetStarByPersonID/{id}")]
        public Star GetStarByPersonID(int id)
        {
            return _userService.GetStarByPersonID(id);
        }

        [HttpGet("GetAllDirectors")]
        public List<Director> GetAllDirectors()
        {
            return _userService.GetAllDirectors();
        }

        [HttpGet("GetDirectorByMovieID/{id}")]
        public Director GetDirectorByMovieID(int id)
        {
            return _userService.GetDirectorByMovieID(id);
        }

        [HttpGet("GetDirectorByPersonID/{id}")]
        public Director GetDirectorByPersonID(int id)
        {
            return _userService.GetDirectorByPersonID(id);
        }

        [HttpGet("GetAllRatings")]
        public List<Ratings> GetAllRatings()
        {
            return _userService.GetAllRatings();
        }

        [HttpGet("GetRatingByMovieID/{id}")]
        public Ratings GetRatingByMovieID(int id)
        {
            return _userService.GetRatingByMovieID(id);
        }

        [HttpGet("GetAllPeople")]
        public List<Person> GetAllPeople()
        {
            return _userService.GetAllPeople();
        }

        [HttpGet("GetPersonByID/{id}")]
        public Person GetPersonByID(int id)
        {
            return _userService.GetPersonByID(id);
        }

        [HttpGet("GetPersonByName/{name}")]
        public Person GetPersonByName(string name)
        {
            return _userService.GetPersonByName(name);
        }
    }
}
