

using AutoMapper;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Role;
using ChatFPT.Domain.Entities;

namespace ChatFPT.Service.Insfracstructure
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile() {
            CreateMap<Category, CreateCategoryModel>().ReverseMap();
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<ApplicationRole,ResponseRoleModel>().ReverseMap();
            CreateMap<ApplicationRole,CreateRoleModel>().ReverseMap();
            CreateMap<ApplicationRole,UpdateRoleModel>().ReverseMap();
        }
    }
}
