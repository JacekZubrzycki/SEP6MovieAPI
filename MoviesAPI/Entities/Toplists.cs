using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Helpers
{
    [Keyless]
    public class Toplists
    {
        public int movie_id { get; set; }
        public int user_id { get; set; }
    }
}