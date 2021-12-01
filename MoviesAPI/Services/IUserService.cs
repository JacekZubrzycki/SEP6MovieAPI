using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public interface IUserService
    {
        List<Movie> GetAllMovies();
        Movie GetAMovieAccordingToID(int id);
    }
}
