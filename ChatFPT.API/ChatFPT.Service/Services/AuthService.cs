

using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.User;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatFPT.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        public async Task Delete(string id)
        {
            ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync(u => u.Id == Guid.Parse(id) && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy User");

            user.DeletedTime = DateTime.Now;
            await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();
        }

        public Task<UserInfoModel> GetUserInfo()
        {
            throw new NotImplementedException();
        }

        public async Task Login(LoginRequestModel model)
        {
            ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync(u => u.UserName == model.UserName && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Sai tên đăng nhập hoặc mật khẩu");

            if(!_passwordHasher.Verify(user.PasswordHash!, model.Password))
            {
                 throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Sai tên đăng nhập hoặc mật khẩu");
            }
        }

        public async Task LoginGoogle(string token)
        {
            
        }

        public async Task Register(RegisterRequestModel model)
        {
            model.CheckValid();
            if( await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync(u => u.UserName == model.UserName && !u.DeletedTime.HasValue) != null)
            {
                 throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "UserName bị trùng");
            }
               
            _passwordHasher.Hash(model.PasswordHash);
            ApplicationUser user = _mapper.Map<ApplicationUser>(model);
            user.CreatedTime = DateTime.Now;
            
            await _unitOfWork.GetRepository<ApplicationUser>().AddAsync(user);
            await _unitOfWork.SaveAsync();
            ApplicationUserRoles userRole = new ApplicationUserRoles
            {
                UserId = user.Id,
                RoleId = Guid.Parse(model.RoleId!)
            };
            await _unitOfWork.GetRepository<ApplicationUserRoles>().AddAsync(userRole);
            await _unitOfWork.SaveAsync();
        }
    }      
}   

