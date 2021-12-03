using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public interface IUserService
    {
        List<Movie> GetAllMovies();
        Movie GetAMovieAccordingToID(int id);

        List<Star> getAllStars();
        Star GetStarByMovieID(int movie_id);
        Star GetStarByPersonID(int person_id);

        List<Director> GetAllDirectors();
        Director GetDirectorByMovieID(int id);
        Director GetDirectorByPersonID(int id);

        List<Ratings> GetAllRatings();
        Ratings GetRatingByMovieID(int id);

        List<Person> GetAllPeople();
        Person GetPersonByID(int id);
        Person GetPersonByName(string name);
        
    }
}
