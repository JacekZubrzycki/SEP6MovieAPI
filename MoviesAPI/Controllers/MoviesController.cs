using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Models;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public MoviesController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _mapper = mapper;

        }
        [HttpGet("GetAMovieAccordingToID")]
        public MoviesDetailed GetAMovieAccordingToID(int id)
        {
            return _userService.GetAMovieAccordingToID(id);
        }
        [HttpGet("GetStarByMovieID")]
        public List<Person> GetStarByMovieID(int movie_id)
        {
            return _userService.GetStarByMovieID(movie_id);
        }
        [HttpGet("GetDirectorByMovieID")]
        public List<Person> GetDirectorByMovieID(int id)
        {
            return _userService.GetDirectorByMovieID(id);
        }
        [HttpGet("GetRatingByMovieID")]
        public Ratings GetRatingByMovieID(int id)
        {
            return _userService.GetRatingByMovieID(id);
        }

        [HttpGet("GetMoviesByTitle")]
        public List<Movie> GetMoviesByTitle(string title)
        {
            return _userService.SearchByName(title);
        }

        [HttpGet("GetToptierList")]
        public ActionResult GetTopTierList()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            try
            {
                var userId = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
                Console.WriteLine(userId);
                if (userId != null)
                {
                    var listOfMovies = _userService.GetTopTierList(int.Parse(userId));
                    return Ok(listOfMovies);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return BadRequest();
        }

        [HttpPost("AddMovieToTierlist")]
        public IActionResult AddMovieToTierlist([FromBody] Movie movie)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            try
            {
                var userId = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
                if (userId == null) return BadRequest(new { message = "User is not logged in or something else went wrong."});

                var returnNum = _userService.AddMovieToTierlist(movie.Id, int.Parse(userId));
                switch (returnNum)
                {
                    case 0:
                        return BadRequest(new { message = "Movie does not exist!" });
                    case 403:
                        return BadRequest(new { message = " Movie is already added to the toplist." });
                    default:
                        return Ok();
                }
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("RemoveMovieFromTierlist")]
        public IActionResult RemoveMovieFromTierlist([FromBody] Movie movie)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
            if (userId == null) return BadRequest(new { message = "User is not logged in or something else went wrong." });
            int responseCode = _userService.RemoveMovieFromTierlist(movie.Id, int.Parse(userId));
            switch (responseCode)
            {
                case 0:
                    return BadRequest(new { message = " Nothing was removed." });
                case 403:
                    return BadRequest(new { message = " Movie does not exist in toplist." });
                default:
                    return Ok();
            }
        }

        [HttpGet("StarRatingOfAllMovies")]
        public Task<object> StarRatingOfAllMovies(int star_id)
        {
            return _userService.StarRatingOfAllMoviesAsync(star_id);
        }
        [HttpGet("CompareMoviesByDecades")]
        public Task<object> CompareMoviesByDecades(int year)
        {
            return _userService.CompareMoviesByDecades(year);
        }
        [HttpGet("ReturnTopNumberOfMovies")]
        public List<Movie> ReturnTopNumberOfMovies(int numOfMovies)
        {
            return _userService.ReturnTopNumberOfMovies(numOfMovies);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var user = _mapper.Map<User>(model);
            try
            {
                //create user
                _userService.CreateAccount(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error if something went wrong with the registration
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginUser model)
        {
            var user = _userService.Login(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                userID = user.Id,
                username = user.Username,
                Token = tokenString
            });
        }

    }
}
