

using AutoMapper;
using ChatFPT.Core.Models.Answer;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Feedback;
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
            CreateMap<ApplicationUser,UserInfoModel>().ReverseMap();

            CreateMap<Question,RequestQuestionModel>().ReverseMap();
            CreateMap<Question,ResponseQuestionModel>().ReverseMap();
            CreateMap<Question,UpdateQuestionModel>().ReverseMap();

            CreateMap<Feedback, CreateFeedbackModel>().ReverseMap();
            CreateMap<Feedback, ResponseFeedbackModel>().ReverseMap();
            CreateMap<Feedback, UpdateFeedbackModel>().ReverseMap();

            CreateMap<Answer, CreateAnswerModel>().ReverseMap();
            CreateMap<Answer, UpdateAnswerModel>().ReverseMap();
            CreateMap<Answer, ResponseAnswerModel>().ReverseMap();

            CreateMap<ApplicationRoleClaims,CreateRoleClaim>().ReverseMap();
            CreateMap<ApplicationRoleClaims,UpdateRoleClaim>().ReverseMap();
            CreateMap<ApplicationRoleClaims,ResponseRoleClaimModel>().ReverseMap();
        }
    }
}
