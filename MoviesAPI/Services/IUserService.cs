using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public interface IUserService
    {
        //List<Movie> GetAllMovies();
        Movie GetAMovieAccordingToID(int id);

        //List<Star> getAllStars();
        List<Person> GetStarByMovieID(int movie_id);
        //Star GetStarByPersonID(int person_id);

        //List<Director> GetAllDirectors();
        List<Person> GetDirectorByMovieID(int id);
        //Director GetDirectorByPersonID(int id);

        //List<Ratings> GetAllRatings();
        Ratings GetRatingByMovieID(int id);

        //List<Person> GetAllPeople();
        //Person GetPersonByID(int id);
        //Person GetPersonByName(string name);
        User Login(string username, string password);
        User CreateAccount(User user, string password);
        User GetById(int id);
    }
}
