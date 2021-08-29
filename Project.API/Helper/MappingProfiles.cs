using AutoMapper;
using Project.Model.Core;
using Project.Model.Model.News;
using Project.Model.Model.User;

namespace Project.API.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserInput>();
            CreateMap<UserInput, User>();

            CreateMap<UserList, User>();
            CreateMap<User, UserList>();

            CreateMap<News, NewsJson>();
            CreateMap<NewsJson, News>();
        }
    }
}
