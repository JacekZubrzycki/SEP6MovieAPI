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

        public Movie GetAMovieAccordingToID(int id)
        {
            return _context.movies.Find(id);
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
            user.Role = "User";

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
    }
}
