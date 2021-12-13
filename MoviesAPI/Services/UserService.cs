using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Models;
using Newtonsoft.Json;

namespace MoviesAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DBContext _context;

        public UserService(DBContext context)
        {
            _context = context;
        }

        public MoviesDetailed GetAMovieAccordingToID(int id)
        {
            OMDBRequiredInfo omdbStuff = new OMDBRequiredInfo();
            var getMovieFromDb = _context.movies.Find(id);
            var getStars = GetStarByMovieID(id);
            var getDirectors = GetDirectorByMovieID(id);
            var getRatings = GetRatingByMovieID(id);
            

            HttpClient hclient = new HttpClient();
            var resultFromAPI = hclient.GetAsync("https://omdbapi.com/?i=tt" + id + "&apikey=2a831622").Result;
            if (resultFromAPI.IsSuccessStatusCode)
            {
                var JsonContent = resultFromAPI.Content.ReadAsStringAsync().Result;
                omdbStuff = JsonConvert.DeserializeObject<OMDBRequiredInfo>(JsonContent);
            }
            return new MoviesDetailed {
                Title = getMovieFromDb.Title,
                Year = getMovieFromDb.Year,
                Rating = getRatings.Rating,
                Votes = getRatings.Votes,
                Stars = getStars,
                Directors = getDirectors,
                Plot = omdbStuff.Plot,
                Poster = omdbStuff.Poster
            };
        }

        public List<Person> GetDirectorByMovieID(int movie_id)
        {
            var person = (from directors in _context.directors
                          join people in _context.people
                          on directors.Person_id equals people.Id
                          where directors.Movie_id == movie_id
                          select new Person
                          {
                              Id = directors.Person_id,
                              Name = people.Name,
                              Birth = people.Birth
                          }).ToList();
            return person;
        }

        public Ratings GetRatingByMovieID(int id)
        {
            Ratings rating = _context.ratings.Where(s => s.Movie_id == id).FirstOrDefault<Ratings>();
            return rating;
        }

        public List<Person> GetStarByMovieID(int movie_id)
        {
            var retStars = (from stars in _context.stars
                            join people in _context.people
                            on stars.Person_id equals people.Id
                            where stars.Movie_id == movie_id
                            select new Person
                            {
                                Id = stars.Person_id,
                                Name = people.Name,
                                Birth = people.Birth
                            }).ToList();

            return retStars;
        }

        public User CreateAccount(User user, string password)
        {
            //Validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required!");

            if (_context.users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken!");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.users.Add(user);
            _context.SaveChanges();

            return user;
        }
        public User GetById(int id)
        {
            var lookingforuser = _context.users.Find(id);

            return lookingforuser;
        }
        public User Login(string username, string password)
        {
            // Checking if the username or password is not empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;
            try { 
            //Getting user from the database
                var user = _context.users.SingleOrDefault(x => x.Username == username);
                // Checking if user exists
                if (user == null)
                    return null;
                // Check if password is correct
                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    return null;
                // authentication was successful, return user obj
                return user;
            }
            catch (MySqlConnector.MySqlException)
            {
                return null;
            }
        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
                throw new AppException("Password is empty");
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Value cannot be empty or whitespace only string.", "password");

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new AppException("password");
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64)
                throw new AppException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128)
                throw new AppException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public List<Movie> SearchByName(string title)
        {
            var movie = _context.movies.Where(p => p.Title.Contains(title)).ToList();

            return movie;
        }
        public List<Movie> GetTopTierList(int userId)
        {
            var retMovies = (from toplists in _context.toplists
                             join movies in _context.movies
                             on toplists.movie_id equals movies.Id
                             where toplists.user_id == userId
                             select new Movie
                             {
                                 Id = movies.Id,
                                 Title = movies.Title,
                                 Year = movies.Year
                             }).ToList();

            return retMovies;
        }

        public int AddMovieToTierlist(int movie_id, int userId)
        {
            Console.WriteLine(movie_id + " " + userId);
            var addMovie = _context.movies.FirstOrDefault(e => e.Id == movie_id);

            if (addMovie == null)
            {
                return 0;
            }

            var getTopLists = _context.toplists.FirstOrDefault(g => g.user_id == userId && g.movie_id == movie_id);
            if (getTopLists == null)
            {
                return _context.Database.ExecuteSqlRaw("INSERT INTO toplists VALUES ({0} ,{1})", addMovie.Id, userId);
            }

            return 403;
        }

        public int RemoveMovieFromTierlist(int movie_id, int userId)
        {
            var getTopLists = _context.toplists.FirstOrDefault(g => g.user_id == userId && g.movie_id == movie_id);
            if (getTopLists == null)
            {
                return 403;
            }
            return _context.Database.ExecuteSqlRaw("DELETE FROM toplists WHERE movie_id = {0} AND user_id = {1}", getTopLists.movie_id, userId);
        }

        public async Task<object> StarRatingOfAllMoviesAsync(int star_id)
        {
            var ratingOverall = from ratings in _context.ratings
                                join stars in _context.stars on ratings.Movie_id equals stars.Movie_id
                                where stars.Person_id == star_id
                                group new { stars, ratings } by new { stars.Person_id } into ratingsGrp
                                select new
                                {
                                    RatingAverage = ratingsGrp.Average(x => x.ratings.Rating),
                                    MoviesActedIn = ratingsGrp.Count(),
                                    VotesSum = ratingsGrp.Sum(e => e.ratings.Votes)
                                };

            return await Task.FromResult(ratingOverall);
        }

        public async Task<object> CompareMoviesByDecades(int year)
        {
            var ratingsOfDecades = from movies in _context.movies
                                   join ratings in _context.ratings
                                   on movies.Id equals ratings.Movie_id
                                   where movies.Year >= year && movies.Year <= (year + 10)
                                   group ratings by 1 into ratingsGroup
                                   select new
                                   {
                                       NumberOfMovies = ratingsGroup.Count(),
                                       AverageRating = ratingsGroup.Average(e => e.Rating),
                                       NumberOfVotes = ratingsGroup.Sum(e => e.Votes)
                                   };
            return await Task.FromResult(ratingsOfDecades);
        }
        public List<Movie> ReturnTopNumberOfMovies(int numOfMovies)
        {
            var listOfMovies = (from movies in _context.movies
                                   join ratings in _context.ratings
                                   on movies.Id equals ratings.Movie_id
                                   orderby ratings.Rating descending
                                   select new Movie
                                   {
                                       Id = movies.Id,
                                       Title = movies.Title,
                                       Year = movies.Year
                                   }).Take(numOfMovies).ToList();
            return listOfMovies;
        }

        public List<Person> ReturnNumberOfStars(int numOfStars)
        {
            var returnStars = (from stars in _context.stars
                join people in _context.people on stars.Person_id equals people.Id
                orderby stars.Person_id descending
                select new Person
                {
                    Id = stars.Person_id,
                    Name = people.Name,
                    Birth = people.Birth
                }).Take(numOfStars).ToList<Person>();

            return returnStars;
        }

        public List<Person> SearchStarsByName(string name)
        {
            var persons = _context.people.Where(p => p.Name.Contains(name)).ToList();

            return persons;
        }
    }
}
