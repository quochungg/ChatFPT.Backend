

using AutoMapper;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Question;
using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Models.User;
using ChatFPT.Domain.Entities;

namespace ChatFPT.Service.Insfracstructure
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile() {
            CreateMap<Category, CreateCategoryModel>().ReverseMap();
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<Category, UpdateCategoryModel>().ReverseMap();
            CreateMap<Category, ResponseCategoryModel>().ReverseMap();
            CreateMap<ApplicationRole,ResponseRoleModel>().ReverseMap();
            CreateMap<ApplicationRole,CreateRoleModel>().ReverseMap();
            CreateMap<ApplicationRole,UpdateRoleModel>().ReverseMap();
            CreateMap<ApplicationUser,RegisterRequestModel>().ReverseMap();

            CreateMap<Question,RequestQuestionModel>().ReverseMap();
            CreateMap<Question,ResponseQuestionModel>().ReverseMap();
            CreateMap<Question,UpdateQuestionModel>().ReverseMap();
        }
    }
}
