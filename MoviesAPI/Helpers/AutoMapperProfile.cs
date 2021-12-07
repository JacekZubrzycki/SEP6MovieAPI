using MoviesAPI.Models;
using AutoMapper;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterModel,User>();
        }
    }
}
