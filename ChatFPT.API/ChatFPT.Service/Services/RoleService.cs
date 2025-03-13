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

namespace ChatFPT.Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateRole(CreateRoleModel model)
        {
            model.ValidateFields();
            ApplicationRole role = _mapper.Map<ApplicationRole>(model);
            role.NormalizedName = model.Name!.ToUpper();
            await _unitOfWork.GetRepository<ApplicationRole>().AddAsync(role);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteRole(Guid roleId)
        {
            ApplicationRole role = await _unitOfWork.GetRepository<ApplicationRole>().Entities.FirstOrDefaultAsync(r => r.Id == roleId && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy ID");

            role.DeletedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<ApplicationRole>().UpdateAsync(role);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseRoleModel>> GetAllRole(string? searchName, int index = 1, int PageSize = 10)
        {
            IQueryable<ResponseRoleModel> query = from role in _unitOfWork.GetRepository<ApplicationRole>().Entities
                                                  where !role.DeletedTime.HasValue
                                                  select new ResponseRoleModel
                                                  {
                                                      Id = role.Id,
                                                      Name = role.Name,
                                                      FullName = role.FullName,
                                                      CreatedTime = role.CreatedTime
                                                  };

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(s => s.Name!.Contains(searchName));
            }

            PaginatedList<ResponseRoleModel> paginatedRole = await _unitOfWork.GetRepository<ResponseRoleModel>().GetPagingAsync(query, index, PageSize);
            return paginatedRole;

        }

        public async Task<ResponseRoleModel> GetRoleById(Guid roleId)
        {
            ApplicationUserRoles role = await _unitOfWork.GetRepository<ApplicationUserRoles>().GetByIdAsync(roleId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstaints.NOT_FOUND, "Không tìm thấy Role");

            if (role.DeletedTime.HasValue)
            {
                throw new ErrorException(StatusCodes.Status410Gone, ResponseCodeConstaints.GONE,
                    $"Role đã bị xóa. Deleted time:{role.DeletedTime}");
            }
            return _mapper.Map<ResponseRoleModel>(role);
        }

        public async Task UpdateRole(UpdateRoleModel model)
        {
            model.ValidateFields();
            ApplicationRole role = await _unitOfWork.GetRepository<ApplicationRole>().Entities.FirstOrDefaultAsync(r => r.Id == model.Id && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy RoleId");

            _mapper.Map(model, role);
            role.LastUpdatedTime = DateTime.UtcNow;
            role.NormalizedName = model.Name!.ToUpper();
            await _unitOfWork.GetRepository<ApplicationRole>().UpdateAsync(role);
            await _unitOfWork.SaveAsync();
        }
    }
}
