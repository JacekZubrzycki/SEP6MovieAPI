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
using MoviesAPI.Helpers;
using MoviesAPI.Models;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
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
        [Authorize(Roles = "User")]
        [HttpGet("GetAMovieAccordingToID/{id}")]
        public Movie GetAMovieAccordingToID(int id)
        {
            return _userService.GetAMovieAccordingToID(id);
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetStarByMovieID/{movie_id}")]
        public List<Person> GetStarByMovieID(int movie_id)
        {
            return _userService.GetStarByMovieID(movie_id);
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetDirectorByMovieID/{id}")]
        public List<Person> GetDirectorByMovieID(int id)
        {
            return _userService.GetDirectorByMovieID(id);
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetRatingByMovieID/{id}")]
        public Ratings GetRatingByMovieID(int id)
        {
            return _userService.GetRatingByMovieID(id);
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
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
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
