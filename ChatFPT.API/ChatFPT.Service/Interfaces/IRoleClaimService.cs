using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface IRoleClaimService
    {
         Task<PaginatedList<ResponseRoleClaimModel>> GetAllRoleClaims(string? searchValue , int index = 1, int pageSize = 10);

        Task CreateRoleClaim(CreateRoleClaim model);

        Task UpdateRoleClaim(UpdateRoleClaim model);

        Task DeleteRoleClaim (string? claimId);
    }
}
