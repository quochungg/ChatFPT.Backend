using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Security.Claims;

namespace ChatFPT.Service.Services
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleClaimService(IHttpContextAccessor httpContextAccessor, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateRoleClaim(CreateRoleClaim model)
        {
            if (await _unitOfWork.GetRepository<ApplicationRole>().Entities.FirstOrDefaultAsync(r => r.Id.ToString() == model.RoleId && !r.DeletedTime.HasValue) == null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "RoleId không tồn tại");
            }

            ApplicationRoleClaims applicationRoleClaims = _mapper.Map<ApplicationRoleClaims>(model);

            applicationRoleClaims.CreatedTime = DateTime.Now;

            await _unitOfWork.GetRepository<ApplicationRoleClaims>().AddAsync(applicationRoleClaims);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteRoleClaim(string? claimId)
        {
            ApplicationRoleClaims applicationRoleClaims = await _unitOfWork.GetRepository<ApplicationRoleClaims>().Entities
                .FirstOrDefaultAsync(r => r.Id.ToString().Equals(claimId) && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "KHông tìm thấy roleclaims");

            applicationRoleClaims.DeletedTime = DateTime.Now;
            await _unitOfWork.GetRepository<ApplicationRoleClaims>().UpdateAsync(applicationRoleClaims);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseRoleClaimModel>> GetAllRoleClaims(string? searchValue, int index = 1, int pageSize = 10)
        {
            IQueryable<ResponseRoleClaimModel> query = from rc in _unitOfWork.GetRepository<ApplicationRoleClaims>().Entities
                                                       join r in _unitOfWork.GetRepository<ApplicationRole>().Entities on rc.RoleId equals r.Id
                                                       where !rc.DeletedTime.HasValue
                                                       select new ResponseRoleClaimModel()
                                                       {
                                                           Id = rc.Id.ToString(),
                                                           ClaimType = rc.ClaimType,
                                                           ClaimValue = rc.ClaimValue,
                                                           RoleName = r.Name,
                                                           CreatedTime = rc.CreatedTime,
                                                       };

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(r => r.ClaimValue!.Contains(searchValue));
            }

            PaginatedList<ResponseRoleClaimModel> paginatedRoleClaims = await _unitOfWork.GetRepository<ResponseRoleClaimModel>().GetPagingAsync(query, index, pageSize);
            return paginatedRoleClaims;
        }

        public async Task<ResponseRoleClaimModel> GetRoleClaimsById(string id)
        {
            ApplicationRoleClaims roleClaims = await _unitOfWork.GetRepository<ApplicationRoleClaims>().GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstaints.NOT_FOUND, "Không tìm thấy RoleClaimId");

            //Check role is deleted or not
            ApplicationRole role = await _unitOfWork.GetRepository<ApplicationRole>().GetByIdAsync(roleClaims.RoleId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstaints.NOT_FOUND, "Không tìm thấy RoleId");

            if (role.DeletedTime.HasValue)
            {
                throw new ErrorException(StatusCodes.Status410Gone, ResponseCodeConstaints.GONE, "Role đã bị xóa");
            }

            else if (roleClaims.DeletedTime.HasValue) { 
                throw new ErrorException(StatusCodes.Status410Gone, ResponseCodeConstaints.GONE,
                    $"RoleClaim đã bị xóa. Deleted time:{roleClaims.DeletedTime}");
            }

            return _mapper.Map<ResponseRoleClaimModel>(roleClaims);
        }

        public async Task UpdateRoleClaim(UpdateRoleClaim model)
        {
            ApplicationRoleClaims applicationRoleClaims = await _unitOfWork.GetRepository<ApplicationRoleClaims>().Entities
                .FirstOrDefaultAsync(r => r.Id.ToString().Equals(model.RoleClaimId) && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy roleclaims");

            if (await _unitOfWork.GetRepository<ApplicationRoleClaims>().Entities.FirstOrDefaultAsync(r => r.Id.ToString() == model.RoleId && !r.DeletedTime.HasValue) == null)
            {
                throw new ErrorException(StatusCodes.Status410Gone, ResponseCodeConstaints.GONE, "RoleId khong ton tai");
            }

            _mapper.Map(model, applicationRoleClaims);

            applicationRoleClaims.LastUpdatedTime = DateTime.Now;
            await _unitOfWork.GetRepository<ApplicationRoleClaims>().UpdateAsync(applicationRoleClaims);
            await _unitOfWork.SaveAsync();
        }
    }
}
