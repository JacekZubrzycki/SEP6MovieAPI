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
    [Route("movies")]
    public class MoviesController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IUserService _userService;

        public MoviesController(IUserService userService, DBContext context)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public List<Movie> GetAllMovies()
        {
            var fishers = _userService.GetAllMovies();
            return fishers;
        }
        [HttpGet("{id}")]
        public IActionResult GetAMovieAccordingToID(int id)
        {
            var movie = _userService.GetAMovieAccordingToID(id);
            return Ok(movie);
        }

        [HttpGet("test/test")]
        public IActionResult TestingThisMF()
        {
            return Ok(new String("IS THIS WORKING?"));
        }
    }
}
