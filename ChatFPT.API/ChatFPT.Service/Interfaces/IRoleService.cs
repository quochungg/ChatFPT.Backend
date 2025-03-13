using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface IRoleService
    {
        Task<PaginatedList<ResponseRoleModel>> GetAllRole(string? searchName, int index, int PageSize);

        Task CreateRole(CreateRoleModel model);

        Task UpdateRole(UpdateRoleModel model);

        Task DeleteRole(Guid roleId);

        Task <ResponseRoleModel> GetRoleById(Guid roleId);
    }
}
