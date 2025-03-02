

using AutoMapper;
using ChatFPT.Core.Models.Category;
using ChatFPT.Domain.Entities;

namespace ChatFPT.Application.Common
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile() {
            CreateMap<Category, CreateCategoryModel>().ReverseMap();
            CreateMap<Category, CategoryModel>().ReverseMap();
        }
    }
}
