using AutoMapper;
using BlogDotNet.DataServices.Abstractions.Models;
using Db = BlogDotNet.Database.Models;

namespace BlogDotNet.DataServices.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile() : base()
    {
        CreateMap<BlogPost, Db.BlogPost>()
            .ReverseMap();

        CreateMap<Db.RankedBlogPost, BlogPost>();
    }
}
